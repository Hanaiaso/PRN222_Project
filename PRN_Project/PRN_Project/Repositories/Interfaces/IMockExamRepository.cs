using PRN_Project.Models;
using PRN_Project.Models.JsonModels;

namespace PRN_Project.Repositories.Interfaces
{
    public interface IMockExamRepository
    {
        // CRUD/Query cho Subject
        Task<List<Subject>> GetAllSubjectsAsync();

        // CRUD/Query cho Exam
        Task<Exam?> GetExamByIdAsync(int examId);
        Task<List<Exam>> GetExamsBySubjectIdAsync(int subjectId);
        Task<bool> IsExamSubmittedAsync(int studentId, int examId);

        // CRUD/Query cho Submit
        Task<List<int>> GetDoneExamIdsByStudentIdAsync(int studentId);
        Task<Submit?> GetSubmitByStudentAndExamIdAsync(int studentId, int examId);
        Task<Submit?> GetSubmitWithDetailsAsync(int submitId);
        Task AddSubmitAsync(Submit submit);
        Task SaveChangesAsync();
    }
}
