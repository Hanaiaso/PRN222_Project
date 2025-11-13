using PRN_Project.Models;
using PRN_Project.Models.JsonModels;

namespace PRN_Project.Services.Interfaces
{
    public interface IAccountService
    {
        (bool Success, string ErrorMessage) Register(Account account);
        (bool Success, string ErrorMessage, Account Account, string Token) Login(string email, string password);
        (bool Success, string ErrorMessage, string OTP) ForgotPassword(string email);
        (bool Success, string ErrorMessage) VerifyOTP(string otp, string sessionOtp, string sessionExpire);
        string ResendOTP(string email);
        (bool Success, string ErrorMessage) ResetPassword(string email, string newPassword);
        (bool Success, string ErrorMessage) ChangePassword(string email, string oldPassword, string newPassword);
        string GenerateJwtToken(Account account);
    }
}