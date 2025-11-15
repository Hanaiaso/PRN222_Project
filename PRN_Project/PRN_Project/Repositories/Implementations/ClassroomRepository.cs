using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace PRN_Project.Repositories.Implementations
{
    public class ClassroomRepository : IClassroomRepository
    {
        private readonly LmsDbContext _context; // Hoặc tên DbContext của bạn

        public ClassroomRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Classroom>> GetClassesByTeacherIdAsync(int teacherId)
        {
            return await _context.Classrooms
                .Where(c => c.Tid == teacherId)
                .Include(c => c.Teacher) // Lấy luôn thông tin giáo viên
                .OrderByDescending(c => c.CreateTime)
            .ToListAsync();
        }

        public async Task<IEnumerable<Classroom>> GetClassesByStudentIdAsync(int studentId)
        {
            var classroomIds = await _context.ClassroomMembers
                .Where(cm => cm.Sid == studentId)
                .Select(cm => cm.ClassroomId)
                .ToListAsync();

            return await _context.Classrooms
                .Where(c => classroomIds.Contains(c.ClassroomId))
                .Include(c => c.Teacher)
                .OrderByDescending(c => c.CreateTime)
                .ToListAsync();
        }

        public async Task<Classroom> GetClassByCodeAsync(string classCode)
        {
            return await _context.Classrooms
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.ClassCode == classCode);
        }

        public async Task<bool> ClassCodeExistsAsync(string classCode)
        {
            return await _context.Classrooms.AnyAsync(c => c.ClassCode == classCode);
        }

        public async Task AddClassAsync(Classroom newClass)
        {
            _context.Classrooms.Add(newClass);
            await _context.SaveChangesAsync();
        }

        public async Task AddMemberAsync(ClassroomMember newMember)
        {
            _context.ClassroomMembers.Add(newMember);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsStudentInClassAsync(int studentId, int classroomId)
        {
            return await _context.ClassroomMembers
                .AnyAsync(cm => cm.Sid == studentId && cm.ClassroomId == classroomId);
        }

        public async Task<Classroom> GetClassByIdAsync(int classroomId) // lấy 
        {
            return await _context.Classrooms
                .Include(c => c.Teacher)
                .Include(c => c.Members)
                    .ThenInclude(m => m.Student)
                .Include(c => c.Posts)
                    .ThenInclude(p => p.Account)
                .Include(c => c.Posts)
                    .ThenInclude(p => p.Submissions)
                        .ThenInclude(s => s.Student)
                .FirstOrDefaultAsync(c => c.ClassroomId == classroomId);
        }
    }
}
