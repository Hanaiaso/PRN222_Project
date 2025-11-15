using PRN_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Project.Services.Interfaces
{
    public interface ITeacherManagementService
    {
        Task<List<Teacher>> GetTeacherAccountsAsync();
        Task<Teacher> GetTeacherDetailsAsync(int teacherId);
        Task<Teacher> CreateTeacherAccountAsync(Teacher teacher, string email, string password);
        Task<bool> UpdateTeacherAccountAsync(int teacherId, Teacher teacherUpdates, string email, string password, bool status);
        Task<bool> DeleteTeacherAccountAsync(int teacherId);
    }
}