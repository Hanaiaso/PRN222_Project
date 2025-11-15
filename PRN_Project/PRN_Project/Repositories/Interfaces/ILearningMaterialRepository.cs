using PRN_Project.Models;

namespace PRN_Project.Repositories.Interfaces
{
    public interface ILearningMaterialRepository
    {
        Task<List<LearningMaterial>> GetBySubjectIdAsync(int subjectId);
        Task<LearningMaterial?> GetByIdAsync(int id);
        Task AddAsync(LearningMaterial material);
        Task UpdateAsync(LearningMaterial material);
        Task DeleteAsync(int id);
        Task<List<LearningMaterial>> GetMaterialsBySubjectAsync(int subjectId);
    }
}
