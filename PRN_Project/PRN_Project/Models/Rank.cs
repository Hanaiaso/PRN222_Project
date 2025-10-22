using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class Rank
    {
        [Key]
        public int RaId { get; set; }

        [Required]
        public int SId { get; set; }
        public Student? Student { get; set; }

        [Required]
        public int EId { get; set; }
        public Exam? Exam { get; set; }

        [MaxLength(50)]
        public string? RankName { get; set; }

        public int? RankOrder { get; set; }
    }
}
