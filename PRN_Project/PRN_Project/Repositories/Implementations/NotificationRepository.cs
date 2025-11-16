using PRN_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN_Project.Services.Interfaces;
using System.Security.Claims;
using PRN_Project.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace PRN_Project.Repositories.Implementations
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly LmsDbContext _context;
        public NotificationRepository(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> GetAllAsync()
        {
            return await _context.Notifications
                .Include(n => n.Sender)
                .Include(n => n.Receivers)
                    .ThenInclude(r => r.Receiver)
                .OrderByDescending(n => n.SentTime)
                .ToListAsync();
        }

        public async Task<Notification?> GetByIdAsync(int id)
        {
            return await _context.Notifications
                .Include(n => n.Sender)
                .Include(n => n.Receivers)
                    .ThenInclude(r => r.Receiver)
                .FirstOrDefaultAsync(n => n.NtId == id);
        }

        public async Task AddAsync(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Notification notification)
        {
            if (notification.Receivers != null)
                _context.NotificationReceivers.RemoveRange(notification.Receivers);

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationReceiver>> GetReceiversByAccountIdAsync(int accountId)
        {
            return await _context.NotificationReceivers
                .Include(nr => nr.Notification)
                    .ThenInclude(n => n.Sender)
                .Where(nr => nr.ReceiverId == accountId)
                .OrderByDescending(nr => nr.Notification.SentTime)
                .ToListAsync();
        }

        public async Task<NotificationReceiver?> GetReceiverByIdAsync(int nrId)
        {
            return await _context.NotificationReceivers.FindAsync(nrId);
        }

        public async Task<List<Account>> GetAllAccountsAsync()
        {
            return await _context.Accounts
                .Where(a => a.Status) // chỉ lấy account đang active
                .ToListAsync();
        }

        public async Task AddReceiversAsync(List<NotificationReceiver> receivers)
        {
            if (receivers != null && receivers.Any())
            {
                _context.NotificationReceivers.AddRange(receivers);
                await _context.SaveChangesAsync();
            }
        }
    }
}
