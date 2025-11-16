using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Project.Repositories.Implementations
{
    public class TeacherClassroomRepository : ITeacherClassroomRepository
    {
        private readonly LmsDbContext _context;

        public TeacherClassroomRepository(LmsDbContext context)
        {
            _context = context;
        }

        // --- Sao chép logic DbContext từ Controller cũ vào đây ---

        public async Task<Teacher> GetTeacherByAccountIdAsync(int accountId)
        {
            // Đã đổi sang Async để tối ưu
            return await _context.Teachers.FirstOrDefaultAsync(s => s.AId == accountId);
        }

        public async Task<List<Classroom>> GetClassroomsByTeacherIdAsync(int teacherId)
        {
            return await _context.Classrooms
                .Where(c => c.Tid == teacherId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Classroom> GetClassroomByIdAsync(int classroomId)
        {
            return await _context.Classrooms
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClassroomId == classroomId);
        }

        public async Task<List<ClassroomMember>> GetMembersByClassroomIdAsync(int classroomId)
        {
            return await _context.ClassroomMembers
                .Where(m => m.ClassroomId == classroomId)
                .Include(m => m.Student)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Student> GetStudentByIdAsync(int studentId)
        {
            return await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SId == studentId);
        }

        public async Task<List<Submit>> GetSubmissionsByStudentIdAsync(int studentId)
        {
            return await _context.Submits
                .Where(s => s.SId == studentId)
                .Include(s => s.Exam)
                .OrderByDescending(s => s.SubmitTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Submit> GetSubmissionForEditAsync(int submitId)
        {
            // Hàm Get (đọc) dùng AsNoTracking
            return await _context.Submits
                .AsNoTracking()
                .Include(s => s.Student)
                .Include(s => s.Exam)
                .FirstOrDefaultAsync(s => s.SbId == submitId);
        }

        public async Task<Submit> GetSubmissionByIdAsync(int submitId)
        {
            // Hàm Post (ghi) không dùng AsNoTracking
            return await _context.Submits.FindAsync(submitId);
        }

        public void UpdateSubmission(Submit submission)
        {
            _context.Update(submission);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<Submit> GetSubmissionWithDetailsAsync(int submitId)
        {
            // Lấy 1 bài nộp, kèm theo Student và Exam (để lấy nội dung câu hỏi)
            return await _context.Submits
                .Include(s => s.Student)
                .Include(s => s.Exam)
                .AsNoTracking() // Dùng AsNoTracking vì chỉ để đọc
                .FirstOrDefaultAsync(s => s.SbId == submitId);
        }
    }
}