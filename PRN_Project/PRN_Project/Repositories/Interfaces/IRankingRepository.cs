using PRN_Project.Models;

namespace PRN_Project.Repositories.Interfaces
{
    public interface IRankingRepository
    {
        Task<List<Subject>> GetSubjectsForTeacherAsync(int accountId);
        Task<List<Subject>> GetAllSubjectsAsync();

        Task<List<Exam>> GetExamsBySubjectAsync(int subjectId);
        Task<List<Submit>> GetSubmitsByExamAsync(int examId);
        Task<Student?> GetStudentByAccountIdAsync(int accountId);
        Task<List<Submit>> GetSubmitsByStudentIdAsync(int studentId);
    }
}
