namespace PRN_Project.Models.RankingModels
{
    public class ExamRankingRow
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public double Score { get; set; }
        public int RankPosition { get; set; }
    }
}
