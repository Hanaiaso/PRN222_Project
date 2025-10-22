using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class Notification
    {
        [Key]
        public int NtId { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        public string? Content { get; set; }

        public DateTime SentTime { get; set; }

        public int? SenderId { get; set; }    // Account
        public Account? Sender { get; set; }

        public ICollection<NotificationReceiver>? Receivers { get; set; }
    }
}
