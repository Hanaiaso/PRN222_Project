using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendOTPEmail(string toEmail, string otp)
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
    }
}