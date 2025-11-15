namespace PRN_Project.ViewModels
{
    public class StudentSubmissionViewModel
    {
        public string ExamName { get; set; }
        public string SubjectName { get; set; }
        public double? Score { get; set; }
        public DateTime SubmitTime { get; set; }

        public int? NumberOfQuestions { get; set; }

        public int CorrectAnswers { get; set; }
        public TimeSpan? ExamDuration { get; set; }

        public string? TeacherComment { get; set; }
    }
}
