using PRN_Project.ViewModels;
using System.Threading.Tasks;

namespace PRN_Project.Services.Interfaces
{
    public interface IProfileService
    {
        // Hàm lấy ViewModel cho Controller
        Task<ProfileViewModel?> GetProfileForViewAsync(int accountId, string role);

        // Hàm cập nhật Profile từ ViewModel
        Task<bool> UpdateProfileAsync(int accountId, string role, ProfileViewModel model);
    }
}