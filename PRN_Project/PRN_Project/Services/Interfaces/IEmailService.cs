namespace PRN_Project.Services.Interfaces
{
    public interface IEmailService
    {
        void SendOTPEmail(string toEmail, string otp);
    }
}