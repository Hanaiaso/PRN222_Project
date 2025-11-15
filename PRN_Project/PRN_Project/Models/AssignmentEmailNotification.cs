using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    /// <summary>
    /// Model để track việc gửi email thông báo assignment
    /// Đảm bảo mỗi assignment chỉ gửi email 1 lần khi còn 1 ngày hạn
    /// </summary>
    public class AssignmentEmailNotification
    {
        [Key]
        public int NotificationId { get; set; }
        
        [Required]
        public int PostId { get; set; }  // FK -> Post (Assignment)
        
        [Required]
        public int StudentId { get; set; }  // FK -> Student
        
        /// <summary>
        /// Thời điểm gửi email thông báo
        /// </summary>
        public DateTime SentAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// Loại thông báo: "OneDayReminder" (còn 1 ngày hạn)
        /// Có thể mở rộng sau: "ThreeDayReminder", "Overdue", etc.
        /// </summary>
        public string NotificationType { get; set; } = "OneDayReminder";
        
        // Navigation
        public Post? Post { get; set; }
        public Student? Student { get; set; }
    }
}

