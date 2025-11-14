using PRN_Project.Models;
using PRN_Project.Models.JsonModels;

namespace PRN_Project.Services.Interfaces
{
    public interface IMockExamService
    {
        Task<List<Subject>> GetSubjectsAsync();

        Task<(List<Exam> Exams, Subject? Subject, List<int> DoneExamIds)> GetExamsForStudentAsync(int subjectId, int studentId);

        Task<(Exam? Exam, string? ErrorRedirectAction)> CanTakeExamAsync(int examId, int studentId);

        Task<(Submit? Submit, string? RedirectAction)> SubmitExamAsync(int examId, int studentId, List<StudentAnswerModel> answers);

        Task<Submit?> AutoSubmitAsync(int studentId, int examId);

        Task<Submit?> GetExamResultAsync(int submitId);
    }
}
