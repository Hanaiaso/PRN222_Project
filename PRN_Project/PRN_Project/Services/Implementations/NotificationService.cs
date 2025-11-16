using Microsoft.AspNetCore.SignalR;
using Microsoft.Identity.Client;
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

        public async Task CreateForAllAsync(Notification notification, int senderId)
        {
            notification.SentTime = DateTime.Now;
            notification.SenderId = senderId;

            // Lấy danh sách tất cả tài khoản
            var allUsers = await _repo.GetAllAccountsAsync();  // Chưa có method này -> thêm trong repo

            await _repo.AddAsync(notification);

            notification.Receivers = new List<NotificationReceiver>();

            foreach (var user in allUsers)
            {
                notification.Receivers.Add(new NotificationReceiver
                {
                    NtId = notification.NtId,
                    ReceiverId = user.AId,
                    IsRead = false
                });
            }

            await _repo.UpdateAsync(notification);

            await _hubContext.Clients.All.SendAsync(
                "ReceiveNotification",
                new
                {
                    id = notification.NtId,
                    title = notification.Title,
                    sentTime = notification.SentTime.ToString("HH:mm dd/MM/yyyy")
                }
            );
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
        public async Task<List<Notification>> GetOtherNotificationsAsync(int excludeId, int currentAccountId)
        {
            var all = await _repo.GetReceiversByAccountIdAsync(currentAccountId);

            return all.Where(n => n.NtId != excludeId)
                      .OrderByDescending(n => n.Notification.SentTime)
                      .Take(5)
                      .Select(n => n.Notification)
                      .ToList();
        }
    }
}
