using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN_Project.Models;
using PRN_Project.Models.JsonModels;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
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

            var result = _accountService.Register(account);

            if (!result.Success)
            {
                ViewBag.Error = result.ErrorMessage;
                return View(account);
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

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var result = _accountService.Login(email, password);

            if (!result.Success)
            {
                ViewBag.Error = result.ErrorMessage;
                return View();
            }

            // Lưu JWT vào Cookie
            Response.Cookies.Append("jwt", result.Token, new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(60)
            });

            // Lưu session
            HttpContext.Session.SetInt32("accountId", result.Account.AId);
            HttpContext.Session.SetString("userEmail", result.Account.Email);
            HttpContext.Session.SetString("role", result.Account.Role.ToString());

            if (result.Account.Role.ToString() == "Teacher")
                return RedirectToAction("Dashboard", "Teacher");
            else if (result.Account.Role.ToString() == "Student")
                return RedirectToAction("Dashboard", "Student");
            else if (result.Account.Role.ToString() == "Admin")
                return RedirectToAction("Dashboard", "Admin");

            return View();
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
            var result = _accountService.ForgotPassword(email);

            if (!result.Success)
            {
                ViewBag.Error = result.ErrorMessage;
                return View();
            }

            // Lưu OTP vào session
            HttpContext.Session.SetString("otpCode", result.OTP);
            HttpContext.Session.SetString("otpEmail", email);
            HttpContext.Session.SetString("otpExpire", DateTime.Now.AddMinutes(5).ToString());

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

                string newOtp = _accountService.ResendOTP(sessionEmail);
                HttpContext.Session.SetString("otpCode", newOtp);
                HttpContext.Session.SetString("otpExpire", DateTime.Now.AddMinutes(5).ToString());

                ViewBag.Message = "Mã OTP mới đã được gửi đến email của bạn.";
                return View();
            }

            // bấm xác minh
            var result = _accountService.VerifyOTP(otp, sessionOtp, sessionExpire);

            if (!result.Success)
            {
                ViewBag.Error = result.ErrorMessage;
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

            // Tạm thời loại bỏ validation của Email và Role
            ModelState.Remove("Email");
            ModelState.Remove("Role");

            // Kiểm tra ModelState (Password và RePassword)
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng kiểm tra lại thông tin mật khẩu!";
                return View(model);
            }

            // Kiểm tra null
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ViewBag.Error = "Mật khẩu mới không được để trống!";
                return View(model);
            }

            var result = _accountService.ResetPassword(email, model.Password);

            if (!result.Success)
            {
                ViewBag.Error = result.ErrorMessage;
                return View(model);
            }

            // Xóa session OTP
            HttpContext.Session.Remove("otpCode");
            HttpContext.Session.Remove("otpEmail");
            HttpContext.Session.Remove("otpExpire");

            TempData["Success"] = "Đặt lại mật khẩu thành công! Hãy đăng nhập với mật khẩu mới.";
            return RedirectToAction("Login");
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

            var result = _accountService.ChangePassword(email, model.OldPassword, model.NewPassword);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage);
                return View(model);
            }

            ViewBag.SuccessMessage = "Đổi mật khẩu thành công!";
            return View();
        }

        // ========== ĐĂNG XUẤT ==========
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }
    }
}