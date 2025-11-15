using PRN_Project.Models;
using PRN_Project.Models.RankingModels;

namespace PRN_Project.Services.Interfaces
{
    public interface IRankingService
    {
        Task<List<Subject>> GetSubjectsForRankingAsync(int accountId, string role);

        Task<List<SubjectExamRankingViewModel>> GetSubjectRankingAsync(int subjectId);

        Task<List<MyExamRankViewModel>> GetStudentRankingAsync(int accountId);

        Task<string> ExportRankingCsvAsync(int examId);
    }
}
