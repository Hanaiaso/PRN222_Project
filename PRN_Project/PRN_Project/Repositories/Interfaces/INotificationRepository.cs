using PRN_Project.Models;

namespace PRN_Project.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetAllAsync();
        Task<Notification?> GetByIdAsync(int id);
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task DeleteAsync(Notification notification);
        Task<List<NotificationReceiver>> GetReceiversByAccountIdAsync(int accountId);
        Task<NotificationReceiver?> GetReceiverByIdAsync(int nrId);
    }
}
