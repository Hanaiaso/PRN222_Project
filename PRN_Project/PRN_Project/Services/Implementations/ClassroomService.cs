using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PRN_Project.Services.Implementations
{
    public class ClassroomService : IClassroomService
    {
        private readonly IClassroomRepository _classroomRepo;
        private readonly LmsDbContext _context;
        private static readonly Random _random = new Random();

        public ClassroomService(IClassroomRepository classroomRepo, LmsDbContext context)
        {
            _classroomRepo = classroomRepo;
            _context = context;
        }

        public async Task<IEnumerable<Classroom>> GetMyClassesAsync(int accountId, string userRole)
        {
            if (userRole.Equals("Teacher", StringComparison.OrdinalIgnoreCase))
            {
                // Chuyển đổi AccountId sang TeacherId
                var teacher = await _context.Teachers
                    .FirstOrDefaultAsync(t => t.AId == accountId);
                
                if (teacher == null)
                {
                    return new List<Classroom>();
                }

                return await _classroomRepo.GetClassesByTeacherIdAsync(teacher.TId);
            }
            else if (userRole.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                // Chuyển đổi AccountId sang StudentId
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.AId == accountId);
                
                if (student == null)
                {
                    return new List<Classroom>();
                }

                return await _classroomRepo.GetClassesByStudentIdAsync(student.SId);
            }
            return new List<Classroom>(); // Trả về danh sách rỗng nếu vai trò không hợp lệ
        }

        public async Task<bool> JoinClassAsync(int accountId, string classCode)
        {
            // Chuyển đổi AccountId sang StudentId
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.AId == accountId);
            
            if (student == null) return false;

            var classroom = await _classroomRepo.GetClassByCodeAsync(classCode);

            // Kiểm tra lớp có tồn tại không
            if (classroom == null) return false;

            // Kiểm tra xem đã tham gia lớp này chưa
            if (await _classroomRepo.IsStudentInClassAsync(student.SId, classroom.ClassroomId))
            {
                return false; // Đã ở trong lớp
            }

            var newMember = new ClassroomMember
            {
                ClassroomId = classroom.ClassroomId,
                Sid = student.SId,
                JoinDate = DateTime.Now
            };

            await _classroomRepo.AddMemberAsync(newMember);
            return true;
        }

        public async Task<Classroom> CreateClassAsync(string className, string description, int accountId)
        {
            // Chuyển đổi AccountId sang TeacherId
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.AId == accountId);
            
            if (teacher == null)
            {
                throw new InvalidOperationException("Không tìm thấy giáo viên với AccountId này.");
            }

            // 1. Tạo mã code ngẫu nhiên
            string classCode;
            do
            {
                classCode = GenerateRandomCode();
            }
            while (await _classroomRepo.ClassCodeExistsAsync(classCode)); // 2. Kiểm tra trùng lặp

            // 3. Tạo lớp học mới
            var newClass = new Classroom
            {
                Tid = teacher.TId,
                ClassName = className,
                ClassDescription = description,
                ClassCode = classCode, // 4. Lưu mã code
                CreateTime = DateTime.Now
            };

            await _classroomRepo.AddClassAsync(newClass);
            return newClass;
        }

        public async Task<Classroom> GetClassByIdAsync(int classroomId)
        {
            return await _classroomRepo.GetClassByIdAsync(classroomId);
        }

        // === HÀM RANDOM MÃ LỚP ===
        private string GenerateRandomCode(int length = 6)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}