using PRN_Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using PRN_Project.Repositories.Interfaces;

namespace PRN_Project.Repositories.Implementations
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly LmsDbContext _context;
        public SubjectRepository(LmsDbContext context) => _context = context;

        public async Task<List<Subject>> GetAllAsync() => await _context.Subjects.ToListAsync();
        public async Task<Subject?> GetByIdAsync(int id) => await _context.Subjects.FindAsync(id);
        public async Task AddAsync(Subject subject)
        {
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Subject subject)
        {
            _context.Subjects.Update(subject);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null)
                return;

            _context.Subjects.Remove(subject);

            await _context.SaveChangesAsync();
        }
    }
}
