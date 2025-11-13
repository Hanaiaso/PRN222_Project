using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN_Project.Models
{
    public class LearningMaterial
    {
        [Key]
        public int MaterialID { get; set; }

        [Required]
        public int SubjectID { get; set; }

        [ForeignKey(nameof(SubjectID))]
        public Subject? Subject { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? FilePath { get; set; }

        public DateTime UploadDate { get; set; }
    }
}
