using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;

namespace PRN_Project.Controllers
{

    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IDashboardRepository _dashboardRepo;
        public IActionResult Index()
        {
            return RedirectToAction("Dashboard");
        }
        public async Task<IActionResult> Dashboard()
        {
            // Lấy số liệu thống kê
            var model = await _dashboardRepo.GetAdminDashboardStatsAsync();
            return View(model);
        }

        private readonly LmsDbContext _context;

        public AdminController(LmsDbContext context, IDashboardRepository dashboardRepo)
        {
            _context = context;
            _dashboardRepo = dashboardRepo;
        }

        // GET: Admin/TeacherAccounts
        public async Task<IActionResult> TeacherAccount()
        {
            var teachers = await _context.Teachers
                .Include(t => t.Account)
                .ToListAsync();

            return View("AccountTeacher", teachers);
        }

        // GET: Admin/TeacherAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers
                .Include(t => t.Account)
                .FirstOrDefaultAsync(m => m.TId == id);

            if (teacher == null) return NotFound();

            return View(teacher);
        }

        // GET: Admin/TeacherAccounts/Create
        public IActionResult Create()
        {
            return View("CreateAccount");
        }

        // POST: Admin/TeacherAccounts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Teacher teacher, string email, string password)
        {
            if (ModelState.IsValid)
            {
                var account = new Account
                {
                    Email = email,
                    Password = BCrypt.Net.BCrypt.HashPassword(password),
                    Role = RoleType.Teacher,
                    Status = true
                };

                _context.Accounts.Add(account);
                await _context.SaveChangesAsync();

                teacher.AId = account.AId;
                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(TeacherAccount));
            }
            return RedirectToAction(nameof(TeacherAccount));
        }

        // GET: Admin/TeacherAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.TId == id);

            if (teacher == null) return NotFound();

            return View("EditAccount", teacher);
        }

        // POST: Admin/TeacherAccounts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher teacher, string email, string password, bool status)
        {
            if (id != teacher.TId) return NotFound();

            if (ModelState.IsValid)
            {
                var existingTeacher = await _context.Teachers
                    .Include(t => t.Account)
                    .FirstOrDefaultAsync(t => t.TId == id);

                if (existingTeacher == null) return NotFound();

                existingTeacher.TName = teacher.TName;
                existingTeacher.Qualification = teacher.Qualification;

                existingTeacher.Account.Email = email;
                existingTeacher.Account.Password = BCrypt.Net.BCrypt.HashPassword(password); ;
                existingTeacher.Account.Status = status;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(TeacherAccount));
            }

            return RedirectToAction(nameof(TeacherAccount));
        }

        // GET: Admin/TeacherAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers
                .Include(t => t.Account)
                .FirstOrDefaultAsync(m => m.TId == id);

            if (teacher == null) return NotFound();

            return View("DeleteAccount", teacher);
        }

        // POST: Admin/TeacherAccounts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers
                .Include(t => t.Account)
                .FirstOrDefaultAsync(t => t.TId == id);

            if (teacher != null)
            {
                _context.Accounts.Remove(teacher.Account);
                _context.Teachers.Remove(teacher);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(TeacherAccount));
        }
    }
}
