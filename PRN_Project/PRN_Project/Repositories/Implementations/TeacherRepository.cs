using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Project.Repositories.Implementations
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly LmsDbContext _context;

        public TeacherRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Teacher>> GetTeachersWithAccountAsync()
        {
            return await _context.Teachers
                .Include(t => t.Account)
                .AsNoTracking()
                .OrderBy(t => t.TName)
                .ToListAsync();
        }

        public async Task<Teacher> GetTeacherWithAccountByIdAsync(int teacherId)
        {
            // Không dùng AsNoTracking() vì chúng ta cần cập nhật/xóa
            return await _context.Teachers
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.TId == teacherId);
        }

        public async Task AddTeacherAsync(Teacher teacher)
        {
            await _context.Teachers.AddAsync(teacher);
        }

        public void RemoveTeacher(Teacher teacher)
        {
            _context.Teachers.Remove(teacher);
        }


        // === THÊM HÀM MỚI NÀY ===
        public async Task<Teacher> GetByIdWithIncludesAsync(int teacherId)
        {
            // Lấy 1 giáo viên, kèm tài khoản và các môn học họ đang dạy
            return await _context.Teachers
                .Include(t => t.Account)
                .Include(t => t.TeacherSubjects) // Rất quan trọng
                .FirstOrDefaultAsync(t => t.TId == teacherId);
        }
    }
}