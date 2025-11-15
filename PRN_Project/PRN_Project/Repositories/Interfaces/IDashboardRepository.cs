using PRN_Project.Models;
using PRN_Project.ViewModels.Dashboard;

namespace PRN_Project.Repositories.Interfaces
{
    public interface IDashboardRepository
    {
        // Admin
        Task<AdminDashboardViewModel> GetAdminDashboardStatsAsync();

        // Teacher
        Task<List<ClassroomStatisticViewModel>> GetTeacherClassStatsAsync(int accountId);
    }
}