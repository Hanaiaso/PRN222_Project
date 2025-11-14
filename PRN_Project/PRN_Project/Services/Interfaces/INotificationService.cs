using PRN_Project.Models;

namespace PRN_Project.Services.Interfaces
{
    public interface INotificationService
    {
        Task<List<Notification>> GetAllAsync();
        Task<Notification?> GetDetailsAsync(int id);
        Task CreateAsync(Notification notification, int[] receiverIds, int senderId);
        Task UpdateAsync(Notification notification);
        Task DeleteAsync(int notificationId);
        Task<List<NotificationReceiver>> GetMyNotificationsAsync(int accountId);
        Task MarkAsReadAsync(int receiverId);
    }
}
