using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public enum RoleType
    {
        Admin,
        Teacher,
        Student
    }
    public class Account
    {
        [Key]
        public int AId { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required, MaxLength(255)]
        public string Password { get; set; } = null!;

        [Required]
        public RoleType Role { get; set; }

        public bool Status { get; set; } = true;

        // Navigation props
        public Student? Student { get; set; }
        public Teacher? Teacher { get; set; }
        public Admin? Admin { get; set; }
        public ICollection<Notification>? SentNotifications { get; set; }
        public ICollection<NotificationReceiver>? NotificationReceivers { get; set; }
    }
}
