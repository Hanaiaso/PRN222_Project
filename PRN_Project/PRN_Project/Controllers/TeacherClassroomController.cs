using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PRN_Project.Controllers
{
    public class TeacherClassroomController : Controller
    {
        private readonly LmsDbContext _context;

        public TeacherClassroomController(LmsDbContext context)
        {
            _context = context;
        }

        // --- 1. HIỂN THỊ LỚP CỦA GIÁO VIÊN ---
        public async Task<IActionResult> Class()
        {

            int? aid = HttpContext.Session.GetInt32("accountId"); 
            var teacher = _context.Teachers.FirstOrDefault(s => s.AId == aid);

            var classrooms = await _context.Classrooms
                .Where(c => c.Tid == teacher.TId)
                .AsNoTracking()
                .ToListAsync();

            return View(classrooms);
        }

        // --- 2. XEM DANH SÁCH HỌC SINH TRONG LỚP ---
        public async Task<IActionResult> ClassDetails(int classroomId)
        {
            // Lấy thông tin lớp (để hiển thị tên)
            var classroom = await _context.Classrooms
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClassroomId == classroomId);

            if (classroom == null) return NotFound();
            ViewBag.ClassroomName = classroom.ClassName; // Giả sử tên là ClassName
            ViewBag.ClassroomId = classroomId;

            // Lấy danh sách thành viên (học sinh) của lớp
            var members = await _context.ClassroomMembers
                .Where(m => m.ClassroomId == classroomId)
                .Include(m => m.Student) // Nối bảng Student để lấy tên
                .AsNoTracking()
                .ToListAsync();

            return View(members);
        }

        // --- 3. XEM TẤT CẢ BÀI NỘP CỦA 1 HỌC SINH ---
        public async Task<IActionResult> StudentSubmissions(int studentId, int classroomId)
        {
            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SId == studentId);

            if (student == null) return NotFound();
            ViewBag.StudentName = student.SName;
            ViewBag.StudentId = studentId;
            ViewBag.ClassroomId = classroomId; // Để cho nút "Quay lại"

            var submissions = await _context.Submits
                .Where(s => s.SId == studentId)
                .Include(s => s.Exam) // Nối bảng Exam để lấy tên bài thi
                .OrderByDescending(s => s.SubmitTime)
                .AsNoTracking()
                .ToListAsync();

            return View(submissions);
        }

        // --- 4. (GET) HIỂN THỊ FORM NHẬN XÉT ---
        [HttpGet]
        public async Task<IActionResult> EditComment(int submitId, int classroomId)
        {
            var submission = await _context.Submits
                .Include(s => s.Student)
                .Include(s => s.Exam)
                .FirstOrDefaultAsync(s => s.SbId == submitId);

            if (submission == null) return NotFound();

            ViewBag.ClassroomId = classroomId; // Cần giữ lại classroomId
            return View(submission);
        }

        // --- 5. (POST) LƯU NHẬN XÉT ---
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditComment(int submitId, string comment, int classroomId)
        {
            var submission = await _context.Submits.FindAsync(submitId);
            if (submission == null) return NotFound();

            // Cập nhật comment
            submission.Comment = comment;
            _context.Update(submission);
            await _context.SaveChangesAsync();

            // Chuyển hướng quay lại trang DS bài nộp của học sinh
            return RedirectToAction("StudentSubmissions",
                new { studentId = submission.SId, classroomId = classroomId });
        }
    }
}