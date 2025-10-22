using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class Admin
    {
        [Key]
        public int AdId { get; set; }

        [Required]
        public int AId { get; set; }
        public Account? Account { get; set; }

        [Required, MaxLength(100)]
        public string AdName { get; set; } = null!;
    }
}
