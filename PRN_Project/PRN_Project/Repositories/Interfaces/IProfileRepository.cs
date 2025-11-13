using PRN_Project.Models;
using System.Threading.Tasks;

namespace PRN_Project.Repositories.Interfaces
{
    public interface IProfileRepository
    {
        // Hàm lấy Account VÀ dữ liệu Student/Teacher đi kèm
        Task<Account?> GetAccountByIdAsync(int accountId);

        // Hàm lưu thay đổi (Update)
        Task<bool> SaveChangesAsync();
    }
}