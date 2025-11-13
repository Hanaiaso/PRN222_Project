using System.ComponentModel.DataAnnotations;

namespace PRN_Project.ViewModels
{
    public class ProfileViewModel
    {
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Giới tính")]
        public string? Gender { get; set; }

        [Display(Name = "Ngày sinh")]
        [DataType(DataType.Date)]
        public DateTime? Dob { get; set; }

        [Display(Name = "Chuyên môn")]
        public string? Qualification { get; set; }
    }
}
