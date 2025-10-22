using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class Subject
    {
        [Key]
        public int SuId { get; set; }

        [Required, MaxLength(100)]
        public string SuName { get; set; } = null!;

        // Navigation
        public ICollection<TeacherSubject>? TeacherSubjects { get; set; }
        public ICollection<Exam>? Exams { get; set; }
    }
}
