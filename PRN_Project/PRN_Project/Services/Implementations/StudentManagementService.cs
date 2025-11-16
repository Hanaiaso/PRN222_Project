using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PRN_Project.Services.Implementations
{
    public class StudentManagementService : IStudentManagementService
    {
        private readonly IAccountRepository _accountRepo;
        private readonly IStudentRepository _studentRepo;

        public StudentManagementService(IAccountRepository accountRepo, IStudentRepository studentRepo)
        {
            _accountRepo = accountRepo;
            _studentRepo = studentRepo;
        }

        public async Task<List<Account>> GetStudentAccountsAsync()
        {
            // Gọi hàm Async từ IAccountRepository
            return await _accountRepo.GetStudentAccountsWithDetailsAsync();
        }

        public async Task<Student> GetStudentDetailsAsync(int studentId)
        {
            return await _studentRepo.GetStudentWithAccountDetailsAsync(studentId);
        }

        public async Task<bool> ToggleAccountStatusAsync(int accountId)
        {
            // Gọi hàm Async từ IAccountRepository
            var account = await _accountRepo.GetByIdAsync(accountId);
            if (account == null) return false;

            account.Status = !account.Status;

            // Gọi hàm Update (Sync) từ IAccountRepository
            _accountRepo.Update(account);

            // Gọi hàm Save (Async) từ IAccountRepository
            await _accountRepo.SaveChangesAsync();
            return true;
        }
        public async Task<Student> GetStudentForEditAsync(int studentId)
        {
            // Gọi hàm mới từ StudentRepository
            return await _studentRepo.GetStudentWithAccountForEditAsync(studentId);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateStudentAccountAsync(
            int studentId, Student studentUpdates, string email, string password, bool status)
        {
            // 1. Lấy dữ liệu gốc
            // (Chúng ta cần GetByIdAsync từ AccountRepo để kiểm tra email cũ)
            var student = await _studentRepo.GetStudentWithAccountForEditAsync(studentId);
            if (student == null)
            {
                return (false, "Không tìm thấy học sinh.");
            }

            // 2. Validation: Mật khẩu (Chỉ validate nếu admin nhập mật khẩu mới)
            if (!string.IsNullOrEmpty(password) && password.Length < 8)
            {
                return (false, "Mật khẩu mới phải có ít nhất 8 ký tự.");
            }

            // 3. Validation: Định dạng Email
            if (string.IsNullOrEmpty(email))
            {
                return (false, "Email không được để trống.");
            }
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                return (false, "Định dạng email không hợp lệ.");
            }

            // 4. Validation: Email đã tồn tại (Chỉ kiểm tra nếu email bị THAY ĐỔI)
            if (student.Account.Email.ToLower() != email.ToLower())
            {
                // Nếu email đã thay đổi, kiểm tra xem email mới đã tồn tại chưa
                if (await _accountRepo.EmailExistsAsync(email))
                {
                    return (false, "Email này đã tồn tại trong hệ thống.");
                }
            }
            // === KẾT THÚC VALIDATION ===

            // 5. Cập nhật thông tin Student
            student.SName = studentUpdates.SName;
            student.Gender = studentUpdates.Gender;
            student.Dob = studentUpdates.Dob;

            // 6. Cập nhật thông tin Account
            student.Account.Email = email;
            student.Account.Status = status;

            // 7. Cập nhật mật khẩu (nếu có)
            if (!string.IsNullOrEmpty(password))
            {
                student.Account.Password = BCrypt.Net.BCrypt.HashPassword(password);
            }

            // 8. Lưu thay đổi
            // (Không cần gọi _accountRepo.Update() vì EF Core đang theo dõi 'student')
            await _accountRepo.SaveChangesAsync(); // Gọi hàm async
            return (true, null);
        }
        public async Task<(bool Success, string ErrorMessage)> CreateStudentAccountAsync(
            Student student, string email, string password)
        {
            // 1. Validation: Mật khẩu
            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                return (false, "Mật khẩu phải có ít nhất 8 ký tự.");
            }

            // 2. Validation: Định dạng Email
            if (string.IsNullOrEmpty(email))
            {
                return (false, "Email không được để trống.");
            }

            // Biểu thức Regex đơn giản để kiểm tra định dạng email
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(email, emailPattern))
            {
                return (false, "Định dạng email không hợp lệ.");
            }
            // === KẾT THÚC THÊM MỚI ===

            // 3. Validation: Email đã tồn tại (Dùng hàm async từ repo của bạn)
            if (await _accountRepo.EmailExistsAsync(email))
            {
                return (false, "Email này đã tồn tại trong hệ thống.");
            }

            // 4. Tạo Account
            var account = new Account
            {
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(password),
                Role = RoleType.Student,
                Status = true // Mặc định là active
            };

            // 5. Lưu Account (Dùng hàm async từ repo của bạn)
            await _accountRepo.AddAsync(account);
            await _accountRepo.SaveChangesAsync(); // Lưu để lấy AId

            // 6. Liên kết và tạo Student
            student.AId = account.AId;
            await _studentRepo.AddStudentAsync(student);
            await _accountRepo.SaveChangesAsync(); // Lưu Student

            return (true, null);
        }
    }
}