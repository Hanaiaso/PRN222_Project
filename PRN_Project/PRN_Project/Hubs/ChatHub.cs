using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PRN_Project.Hubs
{
    public class ChatHub : Hub
    {// Gửi tin nhắn tới người nhận cụ thể
     // PHƯƠNG THỨC MỚI CHO CHỨC NĂNG CHAT
        public async Task SendMessage(string user, string message)
        {
            // Phát sóng tin nhắn đến tất cả các client
            await Clients.All.SendAsync("ReceiveMessage", user, message);

            // Gửi thông báo đến tất cả người khác (ngoại trừ người gửi)
            await Clients.Others.SendAsync("ReceiveNotification", user, message);
        }


        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"Client connected: {Context.ConnectionId}");
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
            await base.OnDisconnectedAsync(exception);
        }

    }
}
