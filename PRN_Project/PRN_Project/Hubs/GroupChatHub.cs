using Microsoft.AspNetCore.SignalR;
using PRN_Project.Services.Interfaces;

namespace PRN_Project.Hubs
{
    public class GroupChatHub : Hub
    {
        private readonly IChatService _chatService;

        public GroupChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task JoinGroup(int groupId)
        {
            // Kiểm tra GroupId hợp lệ (tùy chọn)
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId.ToString());
        }

        public async Task SendGroupMessage(int groupId, int senderId, string senderName, string message)
        {
            // 1. Lưu tin nhắn vào Database
            bool success = await _chatService.SaveGroupMessageAsync(senderId, message, groupId);

            if (success)
            {
                // 2. Phát sóng tin nhắn đến tất cả thành viên trong nhóm
                await Clients.Group(groupId.ToString()).SendAsync("ReceiveGroupMessage", senderName, message);

                // 3. Gửi thông báo đến những người không phải là người gửi
                await Clients.OthersInGroup(groupId.ToString()).SendAsync("ReceiveNotification", senderName, message);
            }
        }

        public async Task<List<object>> LoadHistory(int groupId) // Thêm tham số groupId
        {
            // Kiểm tra tính hợp lệ của người dùng (Tùy chọn, nên làm)
            // Nếu bạn muốn kiểm tra quyền ở đây:
            // var currentAccountId = ... (lấy từ HttpContext.Session hoặc token)

            var history = await _chatService.LoadGroupChatHistoryAsync(groupId);

            // Trả về dữ liệu cần thiết cho client
            return history.Select(m => new {
                senderName = m.Sender?.Email ?? "Anonymous",
                content = m.Content,
                timestamp = m.Timestamp // Tùy chọn: có thể format ngày tháng ở đây
            }).Cast<object>().ToList();
        }
    }
}