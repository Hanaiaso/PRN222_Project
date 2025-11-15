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
    }


}
