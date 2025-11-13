using Microsoft.AspNetCore.SignalR;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Hubs
{
    public class PrivateChatHub : Hub
    {
        private readonly IChatService _chatService;
        // Dictionary tĩnh để lưu trữ ánh xạ AccountId -> ConnectionId (có thể dùng Redis trong thực tế)
        private static readonly Dictionary<int, string> ConnectedUsers = new Dictionary<int, string>();

        public PrivateChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        // Phương thức gọi từ Client khi người dùng kết nối, gửi AccountId của họ
        public void Register(int accountId)
        {
            lock (ConnectedUsers)
            {
                // Xóa kết nối cũ nếu có
                if (ConnectedUsers.ContainsKey(accountId))
                {
                    ConnectedUsers.Remove(accountId);
                }
                ConnectedUsers.Add(accountId, Context.ConnectionId);
            }
        }

        // Phương thức gửi tin nhắn
        public async Task SendPrivateMessage(int senderId, string senderName, int receiverId, string message)
        {
            // 1. Lưu tin nhắn vào Database
            await _chatService.SavePrivateMessageAsync(senderId, message, receiverId);

            // 2. Phát sóng tin nhắn (tới người gửi và người nhận)

            // Tìm ConnectionId của người nhận
            string? receiverConnectionId = null;
            lock (ConnectedUsers)
            {
                ConnectedUsers.TryGetValue(receiverId, out receiverConnectionId);
            }

            // Gửi tin nhắn đến người gửi (để hiển thị tin nhắn vừa gửi)
            await Clients.Caller.SendAsync("ReceivePrivateMessage", senderName, message);

            // Gửi tin nhắn đến người nhận (nếu họ đang online)
            if (receiverConnectionId != null)
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", senderName, message);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            // Xóa người dùng khỏi danh sách khi ngắt kết nối
            lock (ConnectedUsers)
            {
                var userToRemove = ConnectedUsers.FirstOrDefault(x => x.Value == Context.ConnectionId);
                if (userToRemove.Key != 0)
                {
                    ConnectedUsers.Remove(userToRemove.Key);
                }
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}