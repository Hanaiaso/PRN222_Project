namespace PRN_Project.Services.Interfaces
{
    public interface IEmailService
    {
        void SendOTPEmail(string toEmail, string otp);
        void SendAssignmentReminderEmail(string toEmail, string studentName, string assignmentTitle, DateTime dueDate);
    }
}