using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Required(ErrorMessage = "Email không được bỏ trống")]
        [MaxLength(100)]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            ErrorMessage = "Email phải có định dạng hợp lệ với @ và đuôi domain")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password không được bỏ trống")]
        [MaxLength(255)]
        [MinLength(8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@#$%^&+=!]{8,}$",
            ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm cả chữ và số")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        // Thêm RePassword nhưng KHÔNG lưu vào database
        [NotMapped] // dòng này rất quan trọng
        [Compare("Password", ErrorMessage = "Password và Repassword không trùng khớp")]
        [Required(ErrorMessage = "Repassword không được bỏ trống")]
        [MaxLength(255)]
        [DataType(DataType.Password)]
        public string RePassword { get; set; } = null!;

        [Required]
        public RoleType Role { get; set; }

        public bool Status { get; set; } = true;

        // Navigation props
        public Student? Student { get; set; }
        public Teacher? Teacher { get; set; }
        public Admin? Admin { get; set; }
        public ICollection<Notification>? SentNotifications { get; set; }
        public ICollection<NotificationReceiver>? NotificationReceivers { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Post>? Posts { get; set; }
    }
}