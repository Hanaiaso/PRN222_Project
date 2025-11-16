using System.Collections.Generic;

namespace PRN_Project.ViewModels
{
    // ViewModel này đại diện cho kết quả của MỘT môn học
    public class SubjectProgressViewModel
    {
        public string SubjectName { get; set; }
        public double? SubjectAverageScore { get; set; }
        public int SubjectExamsTaken { get; set; }

        // Danh sách các bài nộp chỉ thuộc môn học này
        public List<StudentSubmissionViewModel> Submissions { get; set; }
    }
}