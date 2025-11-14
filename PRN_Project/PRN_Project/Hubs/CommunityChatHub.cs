using Microsoft.AspNetCore.SignalR;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Hubs
{
    public class CommunityChatHub : Hub
    {
        private readonly IChatService _chatService;
        private const int CommunityGroupId = 3; // ID nhóm cố định

        public CommunityChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public override async Task OnConnectedAsync()
        {
            // TỰ ĐỘNG THAM GIA nhóm chat cộng đồng khi kết nối
            await Groups.AddToGroupAsync(Context.ConnectionId, CommunityGroupId.ToString());
            await base.OnConnectedAsync();
        }

        public async Task SendCommunityMessage(string senderName, int senderId, string message)
        {
            // 1. Lưu tin nhắn vào Database
            bool success = await _chatService.SaveCommunityMessageAsync(senderId, message, CommunityGroupId);

            if (success)
            {
                // 2. Phát sóng tin nhắn đến tất cả client trong nhóm
                await Clients.Group(CommunityGroupId.ToString()).SendAsync("ReceiveCommunityMessage", senderName, message, DateTime.Now);

                // 3. Gửi thông báo đến những người không phải là người gửi
                await Clients.OthersInGroup(CommunityGroupId.ToString()).SendAsync("ReceiveNotification", senderName, message, DateTime.Now);
            }
        }

        public async Task<List<object>> LoadHistory()
        {
            var history = await _chatService.LoadCommunityChatHistoryAsync(CommunityGroupId);

            return history.Select(m => new {
                senderName = m.Sender?.Email ?? "Anonymous",
                content = m.Content,
                timestamp = m.Timestamp
            }).Cast<object>().ToList();
        }
    }
}