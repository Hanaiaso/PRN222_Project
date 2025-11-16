using Microsoft.AspNetCore.Mvc.Rendering;
using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;
using PRN_Project.ViewModels;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PRN_Project.Services.Implementations
{
    public class TeacherManagementService : ITeacherManagementService
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly ISubjectRepository _subjectRepo; // Thêm
        private readonly ITeacherSubjectRepository _teacherSubjectRepo; // Thêm

        public TeacherManagementService(ITeacherRepository teacherRepo, IAccountRepository accountRepo, ISubjectRepository subjectRepo)
        {
            _teacherRepo = teacherRepo;
            _accountRepo = accountRepo;
            _subjectRepo = subjectRepo;
        }

        public async Task<List<Teacher>> GetTeacherAccountsAsync()
        {
            return await _teacherRepo.GetTeachersWithAccountAsync();
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


        public async Task<Teacher> GetTeacherDetailsAsync(int teacherId)
        {
            // Sửa hàm này để dùng hàm mới
            return await _teacherRepo.GetByIdWithIncludesAsync(teacherId);
        }

        public async Task<bool> DeleteTeacherAccountAsync(int teacherId)
        {
            // Sửa hàm này để dùng hàm cũ
            var teacher = await _teacherRepo.GetTeacherWithAccountByIdAsync(teacherId);
            if (teacher == null) return false;

            _teacherRepo.RemoveTeacher(teacher);
            _accountRepo.Remove(teacher.Account);
            await _accountRepo.SaveChangesAsync();

            return true;
        }

        // === CÁC HÀM MỚI ===

        // 1. Lấy dữ liệu cho form TẠO MỚI
        public async Task<TeacherEditViewModel> GetTeacherForCreateAsync()
        {
            var allSubjects = await _subjectRepo.GetAllAsync();
            var viewModel = new TeacherEditViewModel
            {
                AllSubjects = allSubjects.Select(s => new SelectListItem
                {
                    Value = s.SuId.ToString(),
                    Text = s.SuName
                }).ToList(),
                Status = true // Mặc định là Active
            };
            return viewModel;
        }

        // 2. Lấy dữ liệu cho form CHỈNH SỬA
        public async Task<TeacherEditViewModel> GetTeacherForEditAsync(int teacherId)
        {
            var teacher = await _teacherRepo.GetByIdWithIncludesAsync(teacherId);
            if (teacher == null) return null;

            var allSubjects = await _subjectRepo.GetAllAsync();

            var viewModel = new TeacherEditViewModel
            {
                TeacherId = teacher.TId,
                TName = teacher.TName,
                Qualification = teacher.Qualification,
                Email = teacher.Account.Email,
                Status = teacher.Account.Status,
                Password= teacher.Account.Password,
                AllSubjects = allSubjects.Select(s => new SelectListItem
                {
                    Value = s.SuId.ToString(),
                    Text = s.SuName
                }).ToList(),
                SelectedSubjectIds = teacher.TeacherSubjects.Select(ts => ts.SuId).ToList()
            };
            return viewModel;
        }

        public async Task<(bool Success, string ErrorMessage)> CreateTeacherAccountAsync(TeacherEditViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.Password) || viewModel.Password.Length < 8)
                return (false, "Mật khẩu phải có ít nhất 8 ký tự.");

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(viewModel.Email, emailPattern))
                return (false, "Định dạng email không hợp lệ.");

            if (await _accountRepo.EmailExistsAsync(viewModel.Email))
                return (false, "Email này đã tồn tại.");

            var account = new Account
            {
                Email = viewModel.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(viewModel.Password),
                Role = RoleType.Teacher,
                Status = viewModel.Status
            };
            await _accountRepo.AddAsync(account);

            var teacher = new Teacher
            {
                TName = viewModel.TName,
                Qualification = viewModel.Qualification,
                Account = account 
            };

            teacher.TeacherSubjects = new List<TeacherSubject>();
            foreach (var subjectId in viewModel.SelectedSubjectIds)
            {
                teacher.TeacherSubjects.Add(new TeacherSubject { SuId = subjectId });
            }

            await _teacherRepo.AddTeacherAsync(teacher);

            await _accountRepo.SaveChangesAsync();
            return (true, null);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateTeacherAccountAsync(TeacherEditViewModel viewModel)
        {
            var teacher = await _teacherRepo.GetByIdWithIncludesAsync(viewModel.TeacherId);
            if (teacher == null)
                return (false, "Không tìm thấy giáo viên.");

            if (!string.IsNullOrEmpty(viewModel.Password) && viewModel.Password.Length < 8)
                return (false, "Mật khẩu mới phải có ít nhất 8 ký tự.");

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(viewModel.Email, emailPattern))
                return (false, "Định dạng email không hợp lệ.");

            if (teacher.Account.Email.ToLower() != viewModel.Email.ToLower() &&
                await _accountRepo.EmailExistsAsync(viewModel.Email))
                return (false, "Email này đã tồn tại.");


            teacher.TName = viewModel.TName;
            teacher.Qualification = viewModel.Qualification;
            teacher.Account.Email = viewModel.Email;
            teacher.Account.Status = viewModel.Status;

            if (!string.IsNullOrEmpty(viewModel.Password))
                teacher.Account.Password = BCrypt.Net.BCrypt.HashPassword(viewModel.Password);

            var existingSubjectIds = teacher.TeacherSubjects.Select(ts => ts.SuId).ToHashSet();
            var selectedSubjectIds = viewModel.SelectedSubjectIds.ToHashSet();

            var subjectsToRemove = teacher.TeacherSubjects
                .Where(ts => !selectedSubjectIds.Contains(ts.SuId)).ToList();
            if (subjectsToRemove.Any())
                _teacherSubjectRepo.RemoveRange(subjectsToRemove);

            var subjectIdsToAdd = selectedSubjectIds
                .Where(id => !existingSubjectIds.Contains(id)).ToList();
            foreach (var id in subjectIdsToAdd)
            {
                await _teacherSubjectRepo.AddAsync(new TeacherSubject { TId = teacher.TId, SuId = id });
            }

            await _accountRepo.SaveChangesAsync();
            return (true, null);
        }
        public async Task<bool> ToggleTeacherStatusAsync(int accountId)
        {
            // 1. Lấy tài khoản
            var account = await _accountRepo.GetByIdAsync(accountId);

            // 2. Kiểm tra (phải tồn tại VÀ là giáo viên)
            if (account == null || account.Role != RoleType.Teacher)
            {
                return false;
            }

            // 3. Đảo ngược trạng thái
            account.Status = !account.Status;

            _accountRepo.Update(account);
            await _accountRepo.SaveChangesAsync();
            return true;
        }
    }
}