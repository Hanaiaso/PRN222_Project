using Microsoft.AspNetCore.Mvc.Rendering;
using PRN_Project.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PRN_Project.ViewModels
{
    public class TeacherEditViewModel
    {
        // Thông tin Giáo viên & Tài khoản
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Tên không được để trống")]
        public string TName { get; set; }
        public string Qualification { get; set; }

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Định dạng email không hợp lệ")]
        public string Email { get; set; }

        public bool Status { get; set; }


        public string? Password { get; set; }

        public List<int> SelectedSubjectIds { get; set; } = new List<int>();

        public List<SelectListItem> AllSubjects { get; set; } = new List<SelectListItem>();
    }
}