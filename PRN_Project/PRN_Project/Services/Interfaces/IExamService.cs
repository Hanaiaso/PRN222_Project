using PRN_Project.Models;

namespace PRN_Project.Services.Interfaces
{
    public interface IExamService
    {
        Task<List<Exam>> GetAllExamsAsync();
        Task<Exam?> GetExamByIdAsync(int id);
        Task CreateExamAsync(Exam exam);
        Task UpdateExamAsync(Exam exam);
        Task DeleteExamAsync(Exam exam);
    }
}