using PRN_Project.Models;

namespace PRN_Project.Repositories.Interfaces
{
    public interface IExamRepository
    {
        Task<List<Exam>> GetAllAsync();
        Task<Exam?> GetByIdAsync(int id);
        Task AddAsync(Exam exam);
        Task UpdateAsync(Exam exam);
        Task DeleteAsync(Exam exam);
    }
}
