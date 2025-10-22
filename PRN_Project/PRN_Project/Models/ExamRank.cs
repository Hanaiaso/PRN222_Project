using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class ExamRank
    {
        [Key]
        public int ErId { get; set; }

        [Required]
        public int SId { get; set; }
        public Student? Student { get; set; }

        [Required]
        public int EId { get; set; }
        public Exam? Exam { get; set; }

        [Required]
        public int RaId { get; set; }
        public Rank? Rank { get; set; }
    }
}
