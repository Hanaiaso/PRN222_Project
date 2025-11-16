using PRN_Project.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PRN_Project.Repositories.Interfaces
{
    public interface ITeacherRepository
    {
        Task<List<Teacher>> GetTeachersWithAccountAsync();
        Task AddTeacherAsync(Teacher teacher);
        void RemoveTeacher(Teacher teacher);

        Task<Teacher> GetTeacherWithAccountByIdAsync(int teacherId);

        // THÊM HÀM NÀY (để lấy chi tiết cho Edit)
        Task<Teacher> GetByIdWithIncludesAsync(int teacherId);
        // SaveChangesAsync sẽ được gọi từ AccountRepository
        // hoặc một UnitOfWork, giả sử AccountRepo có
    }
}