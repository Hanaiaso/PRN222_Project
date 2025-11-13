using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;
using PRN_Project.ViewModels;
using System.Threading.Tasks;

namespace PRN_Project.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepository _profileRepo;

        public ProfileService(IProfileRepository profileRepo)
        {
            _profileRepo = profileRepo;
        }

        // --- Nơi chứa logic map GET ---
        public async Task<ProfileViewModel?> GetProfileForViewAsync(int accountId, string role)
        {
            var account = await _profileRepo.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return null; // Không tìm thấy
            }

            var viewModel = new ProfileViewModel
            {
                Email = account.Email
            };

            if (role == "Student" && account.Student != null)
            {
                viewModel.FullName = account.Student.SName;
                viewModel.Gender = account.Student.Gender;
                viewModel.Dob = account.Student.Dob;
            }
            else if (role == "Teacher" && account.Teacher != null)
            {
                viewModel.FullName = account.Teacher.TName;
                viewModel.Qualification = account.Teacher.Qualification;
            }

            return viewModel;
        }

        // --- Nơi chứa logic map POST (Update) ---
        public async Task<bool> UpdateProfileAsync(int accountId, string role, ProfileViewModel model)
        {
            var account = await _profileRepo.GetAccountByIdAsync(accountId);
            if (account == null)
            {
                return false; 
            }


            account.Email = model.Email;

            if (role == "Student" && account.Student != null)
            {
                account.Student.SName = model.FullName;
                account.Student.Gender = model.Gender;
                account.Student.Dob = model.Dob;
            }
            else if (role == "Teacher" && account.Teacher != null)
            {
                account.Teacher.TName = model.FullName;
                account.Teacher.Qualification = model.Qualification;
            }


            return await _profileRepo.SaveChangesAsync();
        }
    }
}