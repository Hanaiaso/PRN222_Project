using Microsoft.AspNetCore.SignalR;
using PRN_Project.Hubs;
using PRN_Project.Models;
using PRN_Project.Repositories.Interfaces;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repo;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(INotificationRepository repo, IHubContext<NotificationHub> hubContext)
        {
            _repo = repo;
            _hubContext = hubContext;
        }

        public async Task<List<Notification>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Notification?> GetDetailsAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task CreateAsync(Notification notification, int[] receiverIds, int senderId)
        {
            notification.SentTime = DateTime.Now;
            notification.SenderId = senderId;

            await _repo.AddAsync(notification);

            foreach (var receiverId in receiverIds)
            {
                var nr = new NotificationReceiver
                {
                    NtId = notification.NtId,
                    ReceiverId = receiverId,
                    IsRead = false
                };
                notification.Receivers ??= new List<NotificationReceiver>();
                notification.Receivers.Add(nr);
            }

            await _repo.UpdateAsync(notification);

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"📢 Thông báo mới: {notification.Title}");
        }

        public async Task UpdateAsync(Notification notification)
        {
            await _repo.UpdateAsync(notification);
        }

        public async Task DeleteAsync(int notificationId)
        {
            var notification = await _repo.GetByIdAsync(notificationId);
            if (notification != null)
                await _repo.DeleteAsync(notification);
        }

        public async Task<List<NotificationReceiver>> GetMyNotificationsAsync(int accountId)
        {
            return await _repo.GetReceiversByAccountIdAsync(accountId);
        }

        public async Task MarkAsReadAsync(int receiverId)
        {
            var receiver = await _repo.GetReceiverByIdAsync(receiverId);
            if (receiver != null && !receiver.IsRead)
            {
                receiver.IsRead = true;
                await _repo.UpdateAsync(receiver.Notification!);
            }
        }
    }
}
