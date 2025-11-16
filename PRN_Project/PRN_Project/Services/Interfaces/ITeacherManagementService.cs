using PRN_Project.Models;
using PRN_Project.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Project.Services.Interfaces
{
    public interface ITeacherManagementService
    {
        Task<List<Teacher>> GetTeacherAccountsAsync();
        Task<Teacher> GetTeacherDetailsAsync(int teacherId);
        Task<TeacherEditViewModel> GetTeacherForCreateAsync();
        Task<TeacherEditViewModel> GetTeacherForEditAsync(int teacherId);

        Task<(bool Success, string ErrorMessage)> CreateTeacherAccountAsync(TeacherEditViewModel viewModel);
        Task<(bool Success, string ErrorMessage)> UpdateTeacherAccountAsync(TeacherEditViewModel viewModel);
        
        Task<bool> DeleteTeacherAccountAsync(int teacherId);

        Task<bool> ToggleTeacherStatusAsync(int accountId);
    }
}