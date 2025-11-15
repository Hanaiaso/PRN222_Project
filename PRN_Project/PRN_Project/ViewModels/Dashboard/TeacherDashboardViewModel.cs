namespace PRN_Project.ViewModels.Dashboard
{
    public class TeacherDashboardViewModel
    {
        public List<ClassroomStatisticViewModel> ClassStats { get; set; } = new List<ClassroomStatisticViewModel>();
    }

    public class ClassroomStatisticViewModel
    {
        public int ClassroomId { get; set; }
        public string ClassName { get; set; }
        public string ClassCode { get; set; }
        public int TotalStudents { get; set; }
        public int TotalSubmissions { get; set; } // Tổng số bài nộp của lớp này (cho tất cả bài thi)
        public double ParticipationRate { get; set; } // Tỉ lệ tham gia thi

        // Phân bố điểm số
        public int GradeA { get; set; } // Giỏi (ví dụ >= 8.5)
        public int GradeB { get; set; } // Khá
        public int GradeC { get; set; } // Trung bình
        public int GradeD { get; set; } // Yếu
    }
}