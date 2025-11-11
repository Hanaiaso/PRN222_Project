using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace PRN_Project.Hubs
{
    public class ChatHub : Hub
    {// Gửi tin nhắn tới người nhận cụ thể
        public async Task SendPrivateMessage(string receiverId, string message)
        {
            string sender = Context.UserIdentifier; // hoặc lấy từ Context.User.Identity.Name

            await Clients.User(receiverId).SendAsync("ReceiveMessage", sender, message);
        }

        // Khi user kết nối, có thể lưu lại ConnectionId
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }
    }
}
