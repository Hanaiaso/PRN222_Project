using System.ComponentModel.DataAnnotations;

namespace PRN_Project.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Old password không được bỏ trống")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "New password không được bỏ trống")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Compare("NewPassword", ErrorMessage = "New password và Re-new password không trùng khớp")]
        [Required(ErrorMessage = "Re-new password không được bỏ trống")]
        [DataType(DataType.Password)]
        public string ReNewPassword { get; set; } = null!;
    }

}
