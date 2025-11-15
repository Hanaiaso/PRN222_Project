using PRN_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using PRN_Project.Repositories.Interfaces;

namespace PRN_Project.Repositories.Implementations
{
    public class LearningMaterialRepository : ILearningMaterialRepository
    {
        private readonly LmsDbContext _context;
        public LearningMaterialRepository(LmsDbContext context) => _context = context;

        public async Task<List<LearningMaterial>> GetBySubjectIdAsync(int subjectId) =>
            await _context.LearningMaterials
                .Where(m => m.SubjectID == subjectId)
                .ToListAsync();

        public async Task<LearningMaterial?> GetByIdAsync(int id) =>
            await _context.LearningMaterials.FindAsync(id);

        public async Task AddAsync(LearningMaterial material)
        {
            _context.LearningMaterials.Add(material);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(LearningMaterial material)
        {
            _context.LearningMaterials.Update(material);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var material = await _context.LearningMaterials.FindAsync(id);
            if (material != null)
            {
                _context.LearningMaterials.Remove(material);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<List<LearningMaterial>> GetMaterialsBySubjectAsync(int subjectId)
        {
            return await _context.LearningMaterials
                .Where(m => m.SubjectID == subjectId)
                .ToListAsync();
        }
    }
}
