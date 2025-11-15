using PRN_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Project.Repositories.Interfaces
{
    public interface ITeacherRepository
    {
        Task<List<Teacher>> GetTeachersWithAccountAsync();
        Task<Teacher> GetTeacherWithAccountByIdAsync(int teacherId);
        Task AddTeacherAsync(Teacher teacher);
        void RemoveTeacher(Teacher teacher);
        // SaveChangesAsync sẽ được gọi từ AccountRepository
        // hoặc một UnitOfWork, giả sử AccountRepo có
    }
}