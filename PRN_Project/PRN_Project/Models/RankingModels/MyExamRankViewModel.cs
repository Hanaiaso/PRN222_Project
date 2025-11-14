namespace PRN_Project.Models.RankingModels
{
    public class MyExamRankViewModel
    {
        public string ExamName { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public double Score { get; set; }
        public int RankInExam { get; set; }
        public int TotalParticipants { get; set; }
    }
}