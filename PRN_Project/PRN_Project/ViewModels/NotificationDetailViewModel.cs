using PRN_Project.Models;

namespace PRN_Project.ViewModels
{
    public class NotificationDetailViewModel
    {
        public Notification? CurrentNotification { get; set; }
        public List<Notification>? OtherNotifications { get; set; }
    }
}
