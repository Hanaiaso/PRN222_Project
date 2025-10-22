using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class Teacher
    {
        [Key]
        public int TId { get; set; }

        [Required]
        public int AId { get; set; }
        public Account? Account { get; set; }

        [Required, MaxLength(100)]
        public string TName { get; set; } = null!;

        [MaxLength(255)]
        public string? Qualification { get; set; }

        // Navigation
        public ICollection<TeacherSubject>? TeacherSubjects { get; set; }
    }
}
