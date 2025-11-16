namespace PRN_Project.ViewModels
{
    public class StudentProgressViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }

        public int TotalExamsTaken { get; set; }
        public double? AverageScore { get; set; }

        // Danh sách các bài đã nộp
        public List<StudentSubmissionViewModel> Submissions { get; set; }

        // Dữ liệu cho biểu đồ (JSON strings)
        public string ChartLabels { get; set; }
        public string ChartData { get; set; }

        public List<SubjectProgressViewModel> SubjectProgressList { get; set; }


        // === THÊM 2 DÒNG NÀY ===
        // Dữ liệu cho biểu đồ cột (so sánh)
        public string BarChartLabels { get; set; } // Tên các môn học
        public string BarChartData { get; set; }
        public string LineChartDataJson { get; set; }
    }


}
