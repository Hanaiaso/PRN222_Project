using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces; // Quan trọng
using PRN_Project.ViewModels;
using System.Threading.Tasks;

namespace PRN_Project.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IDashboardRepository _dashboardRepo;
        private readonly ITeacherManagementService _teacherMgmtService;
        private readonly IStudentManagementService _studentMgmtService;
        private readonly ISubjectService _subjectService;

        public AdminController(
            IDashboardRepository dashboardRepo,
            ITeacherManagementService teacherMgmtService,
            IStudentManagementService studentMgmtService,
            ISubjectService subjectService)
        {
            _dashboardRepo = dashboardRepo;
            _teacherMgmtService = teacherMgmtService;
            _studentMgmtService = studentMgmtService;
            _subjectService = subjectService;
        }

        public IActionResult Index() => RedirectToAction("Dashboard");

        public async Task<IActionResult> Dashboard()
        {
            var model = await _dashboardRepo.GetAdminDashboardStatsAsync();
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> TeacherAccount()
        {
            var teachers = await _teacherMgmtService.GetTeacherAccountsAsync();
            return View("AccountTeacher", teachers);
        }

        [HttpGet]
        public async Task<IActionResult> TeacherDetails(int id)
        {
            var teacher = await _teacherMgmtService.GetTeacherDetailsAsync(id);
            if (teacher == null) return NotFound();
            return View(teacher);
        }

        // === SỬA HÀM CREATE ===
        [HttpGet]
        public async Task<IActionResult> CreateTeacher()
        {
            var viewModel = await _teacherMgmtService.GetTeacherForCreateAsync();
            return View("CreateTeacher", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTeacher(TeacherEditViewModel viewModel)
        {
            // Bỏ [Bind], dùng ViewModel
            if (ModelState.IsValid) // Chỉ validate DataAnnotations (TName, Email)
            {
                var result = await _teacherMgmtService.CreateTeacherAccountAsync(viewModel);
                if (result.Success)
                {
                    return RedirectToAction(nameof(TeacherAccount));
                }
                TempData["Error"] = result.ErrorMessage;
            }

            // Nếu lỗi, tải lại danh sách môn học
            var allSubjects = await _teacherMgmtService.GetTeacherForCreateAsync();
            viewModel.AllSubjects = allSubjects.AllSubjects;
            return View("CreateTeacher", viewModel);
        }

        // === SỬA HÀM EDIT ===
        [HttpGet]
        public async Task<IActionResult> EditTeacher(int id)
        {
            var viewModel = await _teacherMgmtService.GetTeacherForEditAsync(id);
            if (viewModel == null) return NotFound();
            return View("EditTeacher", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTeacher(int id, TeacherEditViewModel viewModel)
        {
            if (id != viewModel.TeacherId) return NotFound();

            if (ModelState.IsValid)
            {
                // Code của bạn ở đây là đúng
                var result = await _teacherMgmtService.UpdateTeacherAccountAsync(viewModel);
                if (result.Success)
                {
                    return RedirectToAction(nameof(TeacherAccount));
                }
                TempData["Error"] = result.ErrorMessage;
            }
            else
            {

                var allErrors = ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage);

                // Nối các lỗi lại và đưa ra TempData
                string errorString = string.Join(" | ", allErrors);

                // và bất kỳ lỗi nào khác (như TName, Email)
                TempData["Error"] = "Lỗi validation: " + errorString;
                // === KẾT THÚC DEBUG ===
            }

            // Nếu lỗi, tải lại danh sách môn học
            var allSubjects = await _teacherMgmtService.GetTeacherForCreateAsync();
            viewModel.AllSubjects = allSubjects.AllSubjects;
            return View("EditTeacher", viewModel);
        }

        // === HÀM DELETE (Giữ nguyên) ===
        [HttpGet]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleTeacherStatus(int accountId)
        {
            var success = await _teacherMgmtService.ToggleTeacherStatusAsync(accountId);
            if (!success)
            {
                TempData["Error"] = "Không tìm thấy tài khoản giáo viên.";
            }
            return RedirectToAction(nameof(TeacherAccount));
        }


        // ===========================================
        // === QUẢN LÝ TÀI KHOẢN HỌC SINH (Async) ===
        // ===========================================

        [HttpGet]
        public async Task<IActionResult> StudentAccounts()
        {
            var accounts = await _studentMgmtService.GetStudentAccountsAsync();
            return View(accounts);
        }

        [HttpGet]
        public async Task<IActionResult> StudentDetails(int studentId)
        {
            var student = await _studentMgmtService.GetStudentDetailsAsync(studentId);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAccountStatus(int accountId)
        {
            var success = await _studentMgmtService.ToggleAccountStatusAsync(accountId);
            if (!success)
            {
                TempData["Error"] = "Không tìm thấy tài khoản.";
            }
            return RedirectToAction("StudentAccounts");
        }
        [HttpGet]
        public async Task<IActionResult> EditStudent(int studentId)
        {
            var student = await _studentMgmtService.GetStudentForEditAsync(studentId);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: /Admin/EditStudent/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(
            int studentId,
            [Bind("SId, SName, Gender, Dob")] Student studentUpdates,
            string email,
            string password,
            bool status)
        {
            if (studentId != studentUpdates.SId) return NotFound();

            // Gọi service để cập nhật
            var result = await _studentMgmtService.UpdateStudentAccountAsync(
                studentId, studentUpdates, email, password, status);

            if (!result.Success)
            {
                // Nếu thất bại (vd: do validation), hiển thị lỗi và tải lại form
                TempData["Error"] = result.ErrorMessage;
                // Tải lại dữ liệu gốc để hiển thị
                var studentToEdit = await _studentMgmtService.GetStudentForEditAsync(studentId);
                return View(studentToEdit);
            }

            return RedirectToAction(nameof(StudentAccounts));
        }
        public IActionResult CreateStudent()
        {
            // Trả về view CreateStudent.cshtml
            return View(new Student()); // Truyền model rỗng để binding
        }

        // POST: /Admin/CreateStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(
            [Bind("SName, Gender, Dob")] Student student,
            string email,
            string password)
        {
            // Chỉ kiểm tra Model của Student (SName, Gender, Dob)
            if (ModelState.IsValid)
            {
                var result = await _studentMgmtService.CreateStudentAccountAsync(student, email, password);

                if (result.Success)
                {
                    return RedirectToAction(nameof(StudentAccounts));
                }

                // Nếu thất bại (do validation service), hiển thị lỗi
                TempData["Error"] = result.ErrorMessage;
            }

            // Nếu ModelState không hợp lệ hoặc service thất bại,
            // trả lại form với dữ liệu Student đã nhập
            return View(student);
        }
    }
}