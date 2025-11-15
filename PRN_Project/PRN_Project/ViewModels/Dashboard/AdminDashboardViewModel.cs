using PRN_Project.Models;

namespace PRN_Project.ViewModels.Dashboard
{
    public class AdminDashboardViewModel
    {
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalClasses { get; set; }
        public int TotalExams { get; set; }
        public List<ExamStatisticViewModel> RecentExamStats { get; set; } = new List<ExamStatisticViewModel>();
    }

    public class ExamStatisticViewModel
    {
        public string ExamName { get; set; }
        public string SubjectName { get; set; }
        public int TotalSubmissions { get; set; }
        public double? AverageScore { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}