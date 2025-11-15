using Microsoft.EntityFrameworkCore;
using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.ViewModels.Dashboard;

namespace PRN_Project.Repositories.Implementations
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly LmsDbContext _context;

        public DashboardRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<AdminDashboardViewModel> GetAdminDashboardStatsAsync()
        {
            var stats = new AdminDashboardViewModel
            {
                TotalStudents = await _context.Students.CountAsync(),
                TotalTeachers = await _context.Teachers.CountAsync(),
                TotalClasses = await _context.Classrooms.CountAsync(),
                TotalExams = await _context.Exams.CountAsync()
            };

            // Thống kê kết quả các đợt thi gần đây
            stats.RecentExamStats = await _context.Exams
                .Include(e => e.Subject) // Include Subject
                .Include(e => e.Submits)
                .OrderByDescending(e => e.CreatedAt)
                .Take(5) // Lấy 5 kỳ thi gần nhất
                .Select(e => new ExamStatisticViewModel
                {
                    ExamName = e.EName,
                    SubjectName = e.Subject.SuName,
                    CreatedAt = e.CreatedAt,
                    TotalSubmissions = e.Submits.Count(),
                    AverageScore = e.Submits.Any()
                        ? e.Submits.Average(s => s.Score ?? 0)
                        : (double?)null
                }).ToListAsync();

            return stats;
        }

        public async Task<List<ClassroomStatisticViewModel>> GetTeacherClassStatsAsync(int accountId)
        {
            // 1. Lấy TeacherId từ AccountId
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.AId == accountId);
            if (teacher == null) return new List<ClassroomStatisticViewModel>();

            // 2. Lấy danh sách lớp học của giáo viên
            var classrooms = await _context.Classrooms
                .Where(c => c.Tid == teacher.TId)
                .Include(c => c.Members)
                //.ThenInclude(cm => cm.Sid) 
                .ToListAsync();

            var result = new List<ClassroomStatisticViewModel>();

            // Lấy thang điểm (Rank) để so sánh
            var ranks = await _context.Ranks.ToListAsync();

            foreach (var cls in classrooms)
            {
                var studentIds = cls.Members.Select(cm => cm.Sid).ToList();
                var totalStudents = studentIds.Count;

                // Lấy tất cả bài submit của học sinh trong lớp này (đơn giản hóa: lấy tất cả môn)
                // *Nâng cao: Bạn có thể filter theo Subject của lớp học nếu logic yêu cầu chặt chẽ hơn*
                var submissions = await _context.Submits
                    .Where(s => studentIds.Contains(s.SId))
                    .ToListAsync();

                var totalSubmits = submissions.Count;

                // Tính toán thống kê điểm dựa trên bảng Ranks trong DB
                int countA = 0, countB = 0, countC = 0, countD = 0;

                foreach (var sub in submissions)
                {
                    if (sub.Score == null) continue;

                    // Logic map score -> rank dựa vào DB hoặc hardcode nếu muốn nhanh
                    // Dựa vào data insert rank của bạn: A(8.5-10), B(6.5-8.4), C(4.5-6.4), D(0-4.4)
                    double score = sub.Score.Value;
                    if (score >= 8.5) countA++;
                    else if (score >= 6.5) countB++;
                    else if (score >= 4.5) countC++;
                    else countD++;
                }

                // Tính tỉ lệ tham gia (giả định mỗi HS phải làm ít nhất 1 bài, hoặc tỉ lệ nộp bài trung bình)
                // Ở đây tôi tính: Số lượng bài nộp / (Số học sinh * Số bài thi đã diễn ra) - Logic này tương đối phức tạp
                // Nên tạm tính đơn giản: TotalSubmissions để hiển thị

                result.Add(new ClassroomStatisticViewModel
                {
                    ClassroomId = cls.ClassroomId,
                    ClassName = cls.ClassName,
                    ClassCode = cls.ClassCode,
                    TotalStudents = totalStudents,
                    TotalSubmissions = totalSubmits,
                    ParticipationRate = totalStudents > 0 ? Math.Round((double)submissions.Select(s => s.SId).Distinct().Count() / totalStudents * 100, 1) : 0, // % Học sinh đã từng làm bài
                    GradeA = countA,
                    GradeB = countB,
                    GradeC = countC,
                    GradeD = countD
                });
            }

            return result;
        }
    }
}