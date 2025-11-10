using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class ExamRank
    {
        [Key]
        public int ErId { get; set; }

        [Required]
        public int SuId { get; set; }
        public Submit? Submit { get; set; }

        [Required]
        public int RaId { get; set; }
        public Rank? Rank { get; set; }
    }
}
