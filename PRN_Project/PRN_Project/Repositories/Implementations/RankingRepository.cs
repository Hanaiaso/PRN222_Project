using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;

namespace PRN_Project.Repositories.Implementations
{
    public class RankingRepository : IRankingRepository
    {
        private readonly LmsDbContext _context;

        public RankingRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Subject>> GetSubjectsForTeacherAsync(int accountId)
        {
            int teacherId = await _context.Teachers
                .Where(t => t.AId == accountId)
                .Select(t => t.TId)
                .FirstOrDefaultAsync();

            return await _context.TeacherSubjects
                .Where(ts => ts.TId == teacherId)
                .Include(ts => ts.Subject)
                .Select(ts => ts.Subject)
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Subject>> GetAllSubjectsAsync()
        {
            return await _context.Subjects.ToListAsync();
        }

        public async Task<List<Exam>> GetExamsBySubjectAsync(int subjectId)
        {
            return await _context.Exams
                .Include(e => e.Subject)
                .Where(e => e.SuId == subjectId)
                .ToListAsync();
        }

        public async Task<List<Submit>> GetSubmitsByExamAsync(int examId)
        {
            return await _context.Submits
                .Include(s => s.Student)
                .Where(s => s.EId == examId)
                .OrderByDescending(s => s.Score)
                .ToListAsync();
        }

        public async Task<Student?> GetStudentByAccountIdAsync(int accountId)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.AId == accountId);
        }

        public async Task<List<Submit>> GetSubmitsByStudentIdAsync(int studentId)
        {
            return await _context.Submits
                .Include(s => s.Exam)
                    .ThenInclude(e => e.Subject)
                .Where(s => s.SId == studentId)
                .ToListAsync();
        }
        public async Task<Exam?> GetExamByIdAsync(int examId)
        {
            return await _context.Exams
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(e => e.EId == examId);
        }
    }
}
