using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using PRN_Project.Models.JsonModels;
using PRN_Project.Repositories.Interfaces;

namespace PRN_Project.Repositories.Implementations
{
    public class MockExamRepository : IMockExamRepository
    {
        private readonly LmsDbContext _context;

        public MockExamRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task<Exam?> GetExamByIdAsync(int examId)
        {
            return await _context.Exams.FindAsync(examId);
        }

        public async Task<List<Exam>> GetExamsBySubjectIdAsync(int subjectId)
        {
            return await _context.Exams
                                 .Where(e => e.SuId == subjectId)
                                 .ToListAsync();
        }

        public async Task<List<int>> GetDoneExamIdsByStudentIdAsync(int studentId)
        {
            return await _context.Submits
                                 .Where(r => r.SId == studentId)
                                 .Select(r => r.EId)
                                 .ToListAsync();
        }

        public async Task<Submit?> GetSubmitByStudentAndExamIdAsync(int studentId, int examId)
        {
            return await _context.Submits
                                 .FirstOrDefaultAsync(s => s.SId == studentId && s.EId == examId);
        }

        public async Task<bool> IsExamSubmittedAsync(int studentId, int examId)
        {
            return await _context.Submits.AnyAsync(s => s.SId == studentId && s.EId == examId);
        }

        public async Task<Submit?> GetSubmitWithDetailsAsync(int submitId)
        {
            return await _context.Submits
                                 .Include(s => s.Exam)
                                 .Include(s => s.Student)
                                 .FirstOrDefaultAsync(s => s.SbId == submitId);
        }

        public async Task AddSubmitAsync(Submit submit)
        {
            await _context.Submits.AddAsync(submit);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
