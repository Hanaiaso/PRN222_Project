using PRN_Project.Models;

namespace PRN_Project.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> GetStudentByAccountIdAsync(int accountId);

        // Dùng cho action StudentProgress
        Task<Student> GetStudentByIdAsync(int studentId);
        Task<List<Submit>> GetSubmissionsWithDetailsAsync(int studentId);
        void Add(Student student);
        void SaveChanges();
    }
}