namespace PRN_Project.Models.RankingModels
{
    public class SubjectRankingViewModel
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public double AverageScore { get; set; }
        public int RankPosition { get; set; }
    }
}
