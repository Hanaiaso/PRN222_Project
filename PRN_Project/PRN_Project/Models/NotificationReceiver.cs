using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class NotificationReceiver
    {
        [Key]
        public int NrId { get; set; }

        [Required]
        public int NtId { get; set; }
        public Notification? Notification { get; set; }

        [Required]
        public int ReceiverId { get; set; }   // Account
        public Account? Receiver { get; set; }

        public bool IsRead { get; set; } = false;
    }
}
