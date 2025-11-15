using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;

namespace PRN_Project.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly LmsDbContext _context;

        public StudentRepository(LmsDbContext context)
        {
            _context = context;
        }

        public void Add(Student student)
        {
            _context.Students.Add(student);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public async Task<Student> GetStudentByAccountIdAsync(int accountId)
        {
            // Dùng để lấy Student (cho phép theo dõi)
            return await _context.Students.FirstOrDefaultAsync(s => s.AId == accountId);
        }

        public async Task<Student> GetStudentByIdAsync(int studentId)
        {
            // Dùng để đọc (chỉ hiển thị)
            return await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SId == studentId);
        }

        public async Task<List<Submit>> GetSubmissionsWithDetailsAsync(int studentId)
        {
            // Lấy tất cả bài nộp và dữ liệu liên quan
            return await _context.Submits
                .AsNoTracking()
                .Where(s => s.SId == studentId)
                .Include(s => s.Exam)
                    .ThenInclude(e => e.Subject)
                .OrderBy(s => s.SubmitTime)
                .ToListAsync();
        }
    }
}