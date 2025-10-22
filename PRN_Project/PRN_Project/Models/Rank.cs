using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class Rank
    {
        [Key]
        public int RaId { get; set; }
        [MaxLength(50)]
        public string? RankName { get; set; }
        public float? MinScore { get; set; }
        public float? MaxScore { get; set; }
        // Navigation
        public ICollection<ExamRank>? ExamRanks { get; set; }
    }
}
