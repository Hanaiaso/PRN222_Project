using PRN_Project.Models;

namespace PRN_Project.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> GetStudentByAccountIdAsync(int accountId);

        // Dùng cho action StudentProgress
        Task<Student> GetStudentWithAccountDetailsAsync(int studentId);
        Task<Student> GetStudentWithAccountForEditAsync(int studentId);
        Task<Student> GetStudentByIdAsync(int studentId);
        Task<List<Submit>> GetSubmissionsWithDetailsAsync(int studentId);

        Task AddStudentAsync(Student student);
        void Add(Student student);
        void SaveChanges();
    }
}