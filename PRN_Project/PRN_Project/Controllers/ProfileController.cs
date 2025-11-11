using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using PRN_Project.ViewModels;
using System.Threading.Tasks;

namespace PRN_Project.Controllers
{
    public class ProfileController : Controller
    {
        private readonly LmsDbContext _context;

        public ProfileController(LmsDbContext context)
        {
            _context = context;
        }

        // Hàm helper lấy AId từ Session
        private int? GetCurrentAccountId()
        {
            return HttpContext.Session.GetInt32("accountId");
        }

        // Hàm helper lấy Role từ Session
        private string? GetCurrentRole()
        {
            return HttpContext.Session.GetString("role");
        }

        // [GET] /Profile/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? accountId = GetCurrentAccountId();
            string? role = GetCurrentRole();

            if (accountId == null || role == null)
            {
                // Nếu chưa đăng nhập (Session không có), trả về trang đăng nhập
                // Hoặc trang lỗi
                return RedirectToAction("Login", "Account"); // Giả sử
            }

            var account = await _context.Accounts
                .Include(a => a.Student) // Lấy kèm data Student
                .Include(a => a.Teacher) // Lấy kèm data Teacher
                .FirstOrDefaultAsync(a => a.AId == accountId);

            if (account == null)
            {
                return NotFound("Không tìm thấy tài khoản");
            }

            var viewModel = new ProfileViewModel
            {
                Email = account.Email
            };

            if (role == "Student" && account.Student != null)
            {
                viewModel.FullName = account.Student.SName;
                viewModel.Gender = account.Student.Gender;
                viewModel.Dob = account.Student.Dob;
            }
            else if (role == "Teacher" && account.Teacher != null)
            {
                viewModel.FullName = account.Teacher.TName;
                viewModel.Qualification = account.Teacher.Qualification;
            }

            ViewBag.Role = role; // Truyền Role qua View để hiển thị đúng layout
            return View(viewModel);
        }

        // [POST] /Profile/Index
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            int? accountId = GetCurrentAccountId();
            string? role = GetCurrentRole();
            ViewBag.Role = role; // Cần set lại cho View nếu có lỗi

            if (accountId == null || role == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return View(model); // Có lỗi, trả về form
            }

            var account = await _context.Accounts
                .Include(a => a.Student)
                .Include(a => a.Teacher)
                .FirstOrDefaultAsync(a => a.AId == accountId);

            if (account == null)
            {
                return NotFound();
            }

            // Cập nhật thông tin
            account.Email = model.Email;

            if (role == "Student" && account.Student != null)
            {
                account.Student.SName = model.FullName;
                account.Student.Gender = model.Gender;
                account.Student.Dob = model.Dob;
            }
            else if (role == "Teacher" && account.Teacher != null)
            {
                account.Teacher.TName = model.FullName;
                account.Teacher.Qualification = model.Qualification;
            }

            try
            {
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Không thể lưu thay đổi. Hãy thử lại.");
                return View(model);
            }

            return RedirectToAction("Index");
        }

        // [GET] /Profile/ChangePassword
        [HttpGet]
        public IActionResult ChangePassword()
        {
            ViewBag.Role = GetCurrentRole(); // Để View chọn đúng Layout
            return View(new ChangePasswordViewModel());
        }

        // [POST] /Profile/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            ViewBag.Role = GetCurrentRole(); // Cần set lại cho View nếu có lỗi

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int? accountId = GetCurrentAccountId();
            if (accountId == null)
            {
                return Unauthorized();
            }

            var account = await _context.Accounts.FindAsync(accountId);
            if (account == null)
            {
                return NotFound();
            }

            // --- CẢNH BÁO BẢO MẬT ---
            // Hiện tại, bạn đang lưu mật khẩu dạng CHUỖI GỐC (plain text).
            // Đây là một lỗ hổng bảo mật RẤT LỚN.
            // Khi làm chức năng Login/Register, bạn PHẢI băm (HASH) mật khẩu
            // (dùng BCrypt hoặc PBKDF2).
            //
            // Vì hiện tại bạn đang lưu plain text, tôi sẽ so sánh plain text:
            if (account.Password != model.OldPassword)
            {
                ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng");
                return View(model);
            }

            // Cập nhật mật khẩu mới (cũng là plain text)
            account.Password = model.NewPassword;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("Index");
        }
    }
}
