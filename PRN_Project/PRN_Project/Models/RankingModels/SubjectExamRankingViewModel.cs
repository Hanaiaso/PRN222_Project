namespace PRN_Project.Models.RankingModels
{
    public class SubjectExamRankingViewModel
    {
        public string SubjectName { get; set; } = null!;
        public int ExamId { get; set; }
        public string ExamName { get; set; } = null!;
        public List<ExamRankingRow> Rankings { get; set; } = new();
    }
}
