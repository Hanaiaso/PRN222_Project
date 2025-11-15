using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Project.Services.Implementations
{
    public class TeacherManagementService : ITeacherManagementService
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly IAccountRepository _accountRepo;

        public TeacherManagementService(ITeacherRepository teacherRepo, IAccountRepository accountRepo)
        {
            _teacherRepo = teacherRepo;
            _accountRepo = accountRepo;
        }

        public async Task<List<Teacher>> GetTeacherAccountsAsync()
        {
            return await _teacherRepo.GetTeachersWithAccountAsync();
        }

        public async Task<Teacher> GetTeacherDetailsAsync(int teacherId)
        {
            return await _teacherRepo.GetTeacherWithAccountByIdAsync(teacherId);
        }

        public async Task<Teacher> CreateTeacherAccountAsync(Teacher teacher, string email, string password)
        {
            var account = new Account
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Role = RoleType.Teacher,
                Status = true
            };

            // Thêm Account vào context
            await _accountRepo.AddAsync(account);

            // Gán Account mới tạo cho Teacher
            teacher.Account = account;

            // Thêm Teacher vào context
            await _teacherRepo.AddTeacherAsync(teacher);

            // Lưu cả hai thay đổi trong một giao dịch
            await _accountRepo.SaveChangesAsync();

            return teacher;
        }

        public async Task<bool> UpdateTeacherAccountAsync(int teacherId, Teacher teacherUpdates, string email, string password, bool status)
        {
            var existingTeacher = await _teacherRepo.GetTeacherWithAccountByIdAsync(teacherId);
            if (existingTeacher == null) return false;

            // Cập nhật thông tin Teacher
            existingTeacher.TName = teacherUpdates.TName;
            existingTeacher.Qualification = teacherUpdates.Qualification;

            // Cập nhật thông tin Account
            existingTeacher.Account.Email = email;
            existingTeacher.Account.Status = status;

            // Chỉ hash và cập nhật mật khẩu NẾU nó được cung cấp
            if (!string.IsNullOrEmpty(password))
            {
                existingTeacher.Account.Password = BCrypt.Net.BCrypt.HashPassword(password);
            }

            await _accountRepo.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteTeacherAccountAsync(int teacherId)
        {
            var teacher = await _teacherRepo.GetTeacherWithAccountByIdAsync(teacherId);
            if (teacher == null) return false;

            // Phải xóa cả Teacher và Account
            // Cấu hình OnDelete(DeleteBehavior.Cascade) trong DbContext
            // cũng sẽ tự động làm điều này.
            _teacherRepo.RemoveTeacher(teacher);
            _accountRepo.Remove(teacher.Account); // Xóa tài khoản liên quan

            await _accountRepo.SaveChangesAsync();
            return true;
        }
    }
}