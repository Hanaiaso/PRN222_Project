using PRN_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Project.Services.Interfaces
{
    public interface IStudentManagementService
    {
        Task<List<Account>> GetStudentAccountsAsync();
        Task<Student> GetStudentDetailsAsync(int studentId);
        Task<bool> ToggleAccountStatusAsync(int accountId);

        Task<Student> GetStudentForEditAsync(int studentId);

        // Trả về một tuple (bool Success, string ErrorMessage)
        Task<(bool Success, string ErrorMessage)> UpdateStudentAccountAsync(
            int studentId, Student studentUpdates, string email, string password, bool status);

        // === THÊM PHƯƠNG THỨC NÀY ===
        Task<(bool Success, string ErrorMessage)> CreateStudentAccountAsync(
            Student student, string email, string password);
    }
}