using System.ComponentModel.DataAnnotations;

namespace PRN_Project.ViewModels
{
    // ViewModel này chứa thông tin "chung" cho cả Student và Teacher
    public class ProfileViewModel
    {
        // Từ Account
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = null!;

        // Chung
        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string FullName { get; set; } = null!;

        // Thông tin riêng của Student
        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? Dob { get; set; }

        // Thông tin riêng của Teacher
        [Display(Name = "Chuyên môn")]
        public string? Qualification { get; set; }
    }
}
