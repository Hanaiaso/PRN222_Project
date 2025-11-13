using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PRN_Project.Models;
using PRN_Project.Models.JsonModels;
using Microsoft.EntityFrameworkCore;

namespace PRN_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly LmsDbContext _context;
        private readonly IConfiguration _config;

        public AccountController(LmsDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // ========== ĐĂNG KÝ ==========
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Account account)
        {
            if (!ModelState.IsValid)
                return View(account);

            if (_context.Accounts.Any(a => a.Email == account.Email))
            {
                ViewBag.Error = "Email đã được đăng ký!";
                return View(account);
            }

            // Hash mật khẩu trước khi lưu
            account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            account.Status = true;
            account.Role = RoleType.Student;

            // Lưu Account trước
            _context.Accounts.Add(account);
            _context.SaveChanges();

            // ========== TẠO BẢN GHI STUDENT TỰ ĐỘNG ==========
            if (account.Role == RoleType.Student)
            {
                var student = new Student
                {
                    AId = account.AId,  // Lấy ID vừa được tạo
                    SName = account.Email.Split('@')[0], // Lấy tên từ email, hoặc để trống
                    Gender = null,  // Để null, user sẽ cập nhật sau
                    Dob = null      // Để null, user sẽ cập nhật sau
                };

                _context.Students.Add(student);
                _context.SaveChanges();
            }

            TempData["Success"] = "Đăng ký thành công! Hãy đăng nhập.";
            return RedirectToAction("Login", "Account");
        }

        // ========== ĐĂNG NHẬP ==========
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ================= LOGIN (JWT) =================
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var acc = _context.Accounts.FirstOrDefault(a => a.Email == email && a.Status);

            if (acc == null)
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng!";
                return View();
            }

            bool isMatch = false;

            try
            {
                // Kiểm tra xem password trong DB có phải hash BCrypt không
                if (acc.Password.StartsWith("$2a$") || acc.Password.StartsWith("$2b$") || acc.Password.StartsWith("$2y$"))
                {
                    // Là hash → xác minh bằng BCrypt
                    isMatch = BCrypt.Net.BCrypt.Verify(password, acc.Password);
                }
                else
                {
                    // Chưa mã hóa → so sánh trực tiếp
                    isMatch = password == acc.Password;

                    // Nếu đúng thì hash lại để chuyển sang chuẩn BCrypt
                    if (isMatch)
                    {
                        acc.Password = BCrypt.Net.BCrypt.HashPassword(password);
                        _context.SaveChanges();
                    }
                }
            }
            catch
            {
                ViewBag.Error = "Đã xảy ra lỗi khi kiểm tra mật khẩu.";
                return View();
            }

            if (!isMatch)
            {
                ViewBag.Error = "Email hoặc mật khẩu không đúng!";
                return View();
            }

            // ==== Sinh JWT Token ====
            var token = GenerateJwtToken(acc);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(60)
            });

            // Lưu session
            HttpContext.Session.SetInt32("accountId", acc.AId);
            HttpContext.Session.SetString("userEmail", acc.Email);
            HttpContext.Session.SetString("role", acc.Role.ToString());

            if (acc.Role.ToString() == "Teacher")
                return RedirectToAction("Dashboard", "Teacher");
            else if (acc.Role.ToString() == "Student")
                return RedirectToAction("Dashboard", "Student");
            else if (acc.Role.ToString() == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            return View();
        }

        // ================= Sinh JWT Token =================
        private string GenerateJwtToken(Account acc)
        {
            var jwtSettings = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, acc.AId.ToString()),
        new Claim(ClaimTypes.Email, acc.Email),
        new Claim(ClaimTypes.Role, acc.Role.ToString())
    };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtSettings["ExpireMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        // ========== QUÊN MẬT KHẨU ==========
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            var acc = _context.Accounts.FirstOrDefault(a => a.Email == email);
            if (acc == null)
            {
                ViewBag.Error = "Email không tồn tại!";
                return View();
            }

            string otp = new Random().Next(100000, 999999).ToString();
            var otpData = new OTPModel
            {
                Email = email,
                OTPCode = otp,
                ExpiredAt = DateTime.Now.AddMinutes(5)
            };

            HttpContext.Session.SetString("otpCode", otp);
            HttpContext.Session.SetString("otpEmail", email);
            HttpContext.Session.SetString("otpExpire", otpData.ExpiredAt.ToString());

            SendOTPEmail(email, otp);

            TempData["Message"] = "Mã OTP đã được gửi đến email của bạn.";
            return RedirectToAction("VerifyOTP");
        }

        // ========== XÁC NHẬN OTP ==========
        [HttpGet]
        public IActionResult VerifyOTP()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyOTP(string otp, string actionType)
        {
            string sessionOtp = HttpContext.Session.GetString("otpCode");
            string sessionEmail = HttpContext.Session.GetString("otpEmail");
            string sessionExpire = HttpContext.Session.GetString("otpExpire");

            // gửi lại mã OTP
            if (actionType == "resend")
            {
                if (string.IsNullOrEmpty(sessionEmail))
                {
                    ViewBag.Error = "Không tìm thấy email. Vui lòng quay lại nhập lại email.";
                    return View();
                }

                string newOtp = new Random().Next(100000, 999999).ToString();
                HttpContext.Session.SetString("otpCode", newOtp);
                HttpContext.Session.SetString("otpExpire", DateTime.Now.AddMinutes(5).ToString());

                // Gửi OTP mới
                SendOTPEmail(sessionEmail, newOtp);

                ViewBag.Message = "Mã OTP mới đã được gửi đến email của bạn.";
                return View();
            }

            // bấm xác minh
            if (sessionOtp == null || DateTime.Now > DateTime.Parse(sessionExpire))
            {
                ViewBag.Error = "Mã OTP đã hết hạn. Vui lòng yêu cầu lại.";
                return View();
            }

            if (otp != sessionOtp)
            {
                ViewBag.Error = "Mã OTP không chính xác!";
                return View();
            }

            // OTP chính xác thì chuyển sang đặt lại mật khẩu
            return RedirectToAction("ResetPassword");
        }

        // ========== ĐẶT LẠI MẬT KHẨU ==========
        [HttpGet]
        public IActionResult ResetPassword()
        {
            // Kiểm tra xem có email trong session không
            string email = HttpContext.Session.GetString("otpEmail");
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Phiên làm việc đã hết hạn. Vui lòng thực hiện lại quy trình quên mật khẩu.";
                return RedirectToAction("ForgotPassword");
            }

            // Truyền một Account model rỗng xuống View
            return View(new Account());
        }

        [HttpPost]
        public IActionResult ResetPassword(Account model)
        {
            // Kiểm tra email trong session
            string email = HttpContext.Session.GetString("otpEmail");
            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Phiên làm việc đã hết hạn. Vui lòng thực hiện lại quy trình quên mật khẩu.";
                return RedirectToAction("ForgotPassword");
            }

            // Tạm thời loại bỏ validation của Email và Role (vì không cần thiết ở đây)
            ModelState.Remove("Email");
            ModelState.Remove("Role");

            // Kiểm tra ModelState (Password và RePassword)
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng kiểm tra lại thông tin mật khẩu!";
                return View(model);
            }

            // Kiểm tra null (phòng trường hợp)
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.Error = "Mật khẩu mới không được để trống!";
                return View(model);
            }

            // Tìm tài khoản
            var acc = _context.Accounts.FirstOrDefault(a => a.Email == email);
            if (acc == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản. Vui lòng thử lại!";
                return View(model);
            }

            try
            {
                // Hash mật khẩu mới
                acc.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                _context.SaveChanges();

                // Xóa session OTP
                HttpContext.Session.Remove("otpCode");
                HttpContext.Session.Remove("otpEmail");
                HttpContext.Session.Remove("otpExpire");

                TempData["Success"] = "Đặt lại mật khẩu thành công! Hãy đăng nhập với mật khẩu mới.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xảy ra: {ex.Message}";
                return View(model);
            }
        }       

        // ========== ĐĂNG XUẤT ==========
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }
        // ========== GỬI EMAIL OTP ==========
        private void SendOTPEmail(string toEmail, string otp)
        {
            string sender = _config["EmailSettings:SenderEmail"];
            string appPassword = _config["EmailSettings:AppPassword"];
            string host = _config["EmailSettings:Host"] ?? "smtp.gmail.com";
            int port = int.Parse(_config["EmailSettings:Port"] ?? "587");
            bool enableSSL = bool.Parse(_config["EmailSettings:EnableSSL"] ?? "true");

            // Kiểm tra dữ liệu trước khi gửi
            if (string.IsNullOrWhiteSpace(sender))
                throw new Exception("Email người gửi (SenderEmail) không được để trống!");
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new Exception("Email người nhận (toEmail) không được để trống!");

            //Tạo địa chỉ email gửi & nhận
            var from = new MailAddress(sender, "PRN Project");
            var to = new MailAddress(toEmail);

            //Tạo nội dung email
            string subject = "Mã OTP xác thực đổi mật khẩu";
            string body = $"Mã OTP của bạn là: {otp}\nMã này hết hạn sau 5 phút.";

            try
            {
                //Tạo và gửi email
                using (var message = new MailMessage())
                {
                    message.From = from;
                    message.To.Add(to);
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = false;

                    using (var smtp = new SmtpClient(host, port))
                    {
                        smtp.Credentials = new NetworkCredential(sender, appPassword);
                        smtp.EnableSsl = enableSSL;
                        smtp.Send(message);
                    }
                }
            }
            catch (SmtpException ex)
            {
                Console.WriteLine("SMTP ERROR: " + ex.Message);
                throw new Exception("Không gửi được email. Kiểm tra lại cấu hình SMTP hoặc App Password.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                throw;
            }
        }

        // ========== THAY ĐỔI PASSWORD ==========
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Lấy email người dùng hiện tại
            string email = HttpContext.Session.GetString("userEmail");
            if (email == null)
            {
                ModelState.AddModelError("", "Bạn cần đăng nhập trước khi đổi mật khẩu.");
                return View(model);
            }

            var account = _context.Accounts.FirstOrDefault(a => a.Email == email);
            if (account == null)
            {
                ModelState.AddModelError("", "Không tìm thấy tài khoản.");
                return View(model);
            }

            // Kiểm tra mật khẩu cũ
            if (!BCrypt.Net.BCrypt.Verify(model.OldPassword, account.Password))
            {
                ModelState.AddModelError("", "Mật khẩu cũ không đúng.");
                return View(model);
            }

            // Cập nhật mật khẩu mới
            account.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
            _context.SaveChanges();

            ViewBag.SuccessMessage = "Đổi mật khẩu thành công!";
            return View();
        }
    }
}
