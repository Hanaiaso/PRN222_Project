using PRN_Project.Models;

namespace PRN_Project.Services.Interfaces
{
    public interface ILearningMaterialService
    {
        Task<List<LearningMaterial>> GetMaterialsBySubjectIdAsync(int subjectId);
        Task<LearningMaterial?> GetMaterialByIdAsync(int id);
        Task AddMaterialAsync(LearningMaterial material);
        Task UpdateMaterialAsync(LearningMaterial material);
        Task DeleteMaterialAsync(int id);
        Task<List<LearningMaterial>> GetMaterialsBySubjectAsync(int subjectId);
    }
}
