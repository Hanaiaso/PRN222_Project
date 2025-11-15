using PRN_Project.Models;
using PRN_Project.ViewModels; // Cần dùng ViewModels

namespace PRN_Project.Services.Interfaces
{
    public interface IStudentService
    {
        // Cho action Dashboard
        Task<Student> GetStudentByAccountIdAsync(int accountId);

        // Cho action StudentProgress
        Task<StudentProgressViewModel> GetStudentProgressReportAsync(int studentId);
    }
}