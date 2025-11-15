using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Services.Implementations
{
    public class LearningMaterialService : ILearningMaterialService
    {
        private readonly ILearningMaterialRepository _repo;
        public LearningMaterialService(ILearningMaterialRepository repo) => _repo = repo;

        public Task AddMaterialAsync(LearningMaterial material) => _repo.AddAsync(material);
        public Task DeleteMaterialAsync(int id) => _repo.DeleteAsync(id);
        public Task<List<LearningMaterial>> GetMaterialsBySubjectIdAsync(int subjectId) => _repo.GetBySubjectIdAsync(subjectId);
        public Task<LearningMaterial?> GetMaterialByIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task UpdateMaterialAsync(LearningMaterial material) => _repo.UpdateAsync(material);
        public Task<List<LearningMaterial>> GetMaterialsBySubjectAsync(int subjectId)
        {
            return _repo.GetMaterialsBySubjectAsync(subjectId);
        }
    }
}
