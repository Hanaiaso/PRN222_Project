using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;

        public AccountService(
            IAccountRepository accountRepo,
            IStudentRepository studentRepo,
            IEmailService emailService,
            IConfiguration config)
        {
            _accountRepo = accountRepo;
            _studentRepo = studentRepo;
            _emailService = emailService;
            _config = config;
        }

        public (bool Success, string ErrorMessage) Register(Account account)
        {
            if (_accountRepo.EmailExists(account.Email))
            {
                return (false, "Email đã được đăng ký!");
            }

            // Hash mật khẩu trước khi lưu
            account.Password = BCrypt.Net.BCrypt.HashPassword(account.Password);
            account.Status = true;
            account.Role = RoleType.Student;

            // Lưu Account trước
            _accountRepo.Add(account);
            _accountRepo.SaveChanges();

            // ========== TẠO BẢN GHI STUDENT TỰ ĐỘNG ==========
            if (account.Role == RoleType.Student)
            {
                var student = new Student
                {
                    AId = account.AId,  // Lấy ID vừa được tạo
                    SName = account.Email.Split('@')[0], // Lấy tên từ email
                    Gender = null,  // Để null, user sẽ cập nhật sau
                    Dob = null      // Để null, user sẽ cập nhật sau
                };

                _studentRepo.Add(student);
                _studentRepo.SaveChanges();
            }

            return (true, null);
        }

        public (bool Success, string ErrorMessage, Account Account, string Token) Login(string email, string password)
        {
            var acc = _accountRepo.GetByEmail(email);

            if (acc == null || !acc.Status)
            {
                return (false, "Email hoặc mật khẩu không đúng!", null, null);
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
                        _accountRepo.Update(acc);
                        _accountRepo.SaveChanges();
                    }
                }
            }
            catch
            {
                return (false, "Đã xảy ra lỗi khi kiểm tra mật khẩu.", null, null);
            }

            if (!isMatch)
            {
                return (false, "Email hoặc mật khẩu không đúng!", null, null);
            }

            // ==== Sinh JWT Token ====
            var token = GenerateJwtToken(acc);

            return (true, null, acc, token);
        }

        public (bool Success, string ErrorMessage, string OTP) ForgotPassword(string email)
        {
            var acc = _accountRepo.GetByEmail(email);
            if (acc == null)
            {
                return (false, "Email không tồn tại!", null);
            }

            string otp = new Random().Next(100000, 999999).ToString();
            _emailService.SendOTPEmail(email, otp);

            return (true, null, otp);
        }

        public (bool Success, string ErrorMessage) VerifyOTP(string otp, string sessionOtp, string sessionExpire)
        {
            if (sessionOtp == null || DateTime.Now > DateTime.Parse(sessionExpire))
            {
                return (false, "Mã OTP đã hết hạn. Vui lòng yêu cầu lại.");
            }

            if (otp != sessionOtp)
            {
                return (false, "Mã OTP không chính xác!");
            }

            return (true, null);
        }

        public string ResendOTP(string email)
        {
            string newOtp = new Random().Next(100000, 999999).ToString();
            _emailService.SendOTPEmail(email, newOtp);
            return newOtp;
        }

        public (bool Success, string ErrorMessage) ResetPassword(string email, string newPassword)
        {
            var acc = _accountRepo.GetByEmail(email);
            if (acc == null)
            {
                return (false, "Không tìm thấy tài khoản. Vui lòng thử lại!");
            }

            try
            {
                // Hash mật khẩu mới
                acc.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _accountRepo.Update(acc);
                _accountRepo.SaveChanges();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Có lỗi xảy ra: {ex.Message}");
            }
        }

        public (bool Success, string ErrorMessage) ChangePassword(string email, string oldPassword, string newPassword)
        {
            var account = _accountRepo.GetByEmail(email);
            if (account == null)
            {
                return (false, "Không tìm thấy tài khoản.");
            }

            // Kiểm tra mật khẩu cũ
            if (!BCrypt.Net.BCrypt.Verify(oldPassword, account.Password))
            {
                return (false, "Mật khẩu cũ không đúng.");
            }

            // Cập nhật mật khẩu mới
            account.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _accountRepo.Update(account);
            _accountRepo.SaveChanges();

            return (true, null);
        }

        public string GenerateJwtToken(Account acc)
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
    }
}