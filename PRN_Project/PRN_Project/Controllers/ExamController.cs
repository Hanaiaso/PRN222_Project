using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN_Project.Models;
using System;
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

        // ===== Kiểm tra quyền =====
        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("Role");
            return role == "Teacher" || role == "Admin";
        }

        // ===== Danh sách bài thi =====
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var exams = await _context.Exams
                .Include(e => e.Subject)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return View(exams);
        }

        // ===== Chi tiết bài thi =====
        public async Task<IActionResult> Details(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id == null) return NotFound();

            var exam = await _context.Exams
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(e => e.EId == id);

            if (exam == null) return NotFound();

            return View(exam);
        }

        // ===== Tạo mới =====
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exam exam)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            if (ModelState.IsValid)
            {
                exam.CreatedAt = DateTime.Now;
                _context.Add(exam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName", exam.SuId);
            return View(exam);
        }

        // ===== Sửa bài thi =====
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var exam = await _context.Exams
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(e => e.EId == id);
            if (exam == null)
            {
                return NotFound();
            }
            var subjects = await _context.Subjects.ToListAsync();
            if (subjects == null || subjects.Count == 0)
            {
                subjects = new List<Subject>();
            }
            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName", exam.SuId);
            return View(exam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exam exam, string questionData)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
            if (id != exam.EId) return NotFound();

            try
            {
                if (!string.IsNullOrEmpty(questionData))
                {
                    exam.ExamContent = questionData;
                }

                _context.Update(exam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Exams.Any(e => e.EId == exam.EId))
                    return NotFound();
                else
                    throw;
            }

            ViewBag.Subjects = new SelectList(_context.Subjects, "SuId", "SubjectName", exam.SuId);
            return View(exam);
        }

        // ===== Xóa =====
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");
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
            if (!IsAdmin()) return RedirectToAction("AccessDenied", "Home");

            var exam = await _context.Exams.FindAsync(id);
            if (exam != null)
            {
                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
