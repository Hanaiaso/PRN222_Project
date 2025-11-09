using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Project.Controllers
{
    public class ExamController : Controller
    {
        private readonly LmsDbContext _context;

        public ExamController(LmsDbContext context)
        {
            _context = context;
        }

        // ===== Kiểm tra quyền truy cập =====
        private bool IsAdminOrTeacher()
        {
            var role = HttpContext.Session.GetString("role");
            return role == RoleType.Admin.ToString() || role == RoleType.Teacher.ToString();
        }

        private IActionResult CheckPermission()
        {
            if (!IsAdminOrTeacher())
                return RedirectToAction("Login", "Account");
            return null!;
        }

        // ===== Danh sách bài thi =====
        public async Task<IActionResult> Index()
        {
            var permission = CheckPermission();
            if (permission != null) return permission;

            var exams = await _context.Exams
                .Include(e => e.Subject)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return View(exams);
        }

        // ===== Xem chi tiết =====
        public async Task<IActionResult> Details(int? id)
        {
            var permission = CheckPermission();
            if (permission != null) return permission;
            if (id == null) return NotFound();

            var exam = await _context.Exams
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(e => e.EId == id);

            if (exam == null) return NotFound();
            return View(exam);
        }

        // ===== Tạo mới =====
        [HttpGet]
        public IActionResult Create()
        {
            var permission = CheckPermission();
            if (permission != null) return permission;

            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exam exam)
        {
            var permission = CheckPermission();
            if (permission != null) return permission;

            if (ModelState.IsValid)
            {
                exam.CreatedAt = DateTime.Now;
                _context.Add(exam);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tạo bài thi thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName", exam.SuId);
            return View(exam);
        }

        // ===== Sửa =====
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            var permission = CheckPermission();
            if (permission != null) return permission;
            if (id == null) return NotFound();

            var exam = await _context.Exams.FindAsync(id);
            if (exam == null) return NotFound();

            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName", exam.SuId);
            return View(exam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exam exam)
        {
            var permission = CheckPermission();
            if (permission != null) return permission;
            if (id != exam.EId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exam);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Cập nhật bài thi thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Exams.Any(e => e.EId == exam.EId))
                        return NotFound();
                    throw;
                }
            }

            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName", exam.SuId);
            return View(exam);
        }

        // ===== Xóa =====
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            var permission = CheckPermission();
            if (permission != null) return permission;
            if (id == null) return NotFound();

            var exam = await _context.Exams
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(e => e.EId == id);

            if (exam == null) return NotFound();
            return View(exam);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permission = CheckPermission();
            if (permission != null) return permission;

            var exam = await _context.Exams.FindAsync(id);
            if (exam != null)
            {
                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Xóa bài thi thành công!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
