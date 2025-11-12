using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using PRN_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PRN_Project.Controllers
{
    [Authorize] // Chỉ người đăng nhập có JWT hợp lệ mới truy cập được
    public class ExamController : Controller
    {
        private readonly LmsDbContext _context;

        public ExamController(LmsDbContext context)
        {
            _context = context;
        }

        // ===== Danh sách bài thi =====
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<IActionResult> Index()
        {
            var exams = await _context.Exams
                .Include(e => e.Subject)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();

            return View(exams);
        }

        // ===== Xem chi tiết =====
        [Authorize(Roles = "Admin,Teacher,Student")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _context.Exams
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(e => e.EId == id);

            if (exam == null) return NotFound();
            return View(exam);
        }

        // ===== Tạo mới =====
        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName");
            return View();
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Exam exam)
        {
            if (ModelState.IsValid)
            {
                //exam.CreatedAt = DateTime.Now; //khong can do da dat mac dinh trong model
                _context.Add(exam);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tạo bài thi thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName", exam.SuId);
            return View(exam);
        }

        // ===== Sửa =====
        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _context.Exams.FindAsync(id);
            if (exam == null) return NotFound();

            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName", exam.SuId);
            return View(exam);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Exam exam, string questionData)
        {
            if (id != exam.EId) return NotFound();

            if (ModelState.IsValid)
            {
                var dbExam = await _context.Exams.FirstOrDefaultAsync(e => e.EId == id);
                if (dbExam == null) return NotFound();

                // Giữ nguyên CreatedAt
                // Cập nhật các trường có thể sửa
                dbExam.EName = exam.EName;
                dbExam.SuId = exam.SuId;
                dbExam.StartTime = exam.StartTime;
                dbExam.EndTime = exam.EndTime;
                dbExam.Status = exam.Status;

                // Giữ lại nội dung câu hỏi nếu không thay đổi
                if (!string.IsNullOrEmpty(questionData))
                {
                    dbExam.ExamContent = questionData;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật bài thi thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.SubjectId = new SelectList(_context.Subjects, "SuId", "SuName", exam.SuId);
            return View(exam);
        }



        // ===== Xóa =====
        [Authorize(Roles = "Admin,Teacher")]
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var exam = await _context.Exams
                .Include(e => e.Subject)
                .FirstOrDefaultAsync(e => e.EId == id);

            if (exam == null) return NotFound();
            return View(exam);
        }

        [Authorize(Roles = "Admin,Teacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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
