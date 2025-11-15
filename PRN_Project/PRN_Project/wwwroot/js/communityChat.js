// TRONG FILE /js/communityChat.js

"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/communityChatHub") // KẾT NỐI ĐẾN HUB CHAT CỘNG ĐỒNG
    .build();

const currentAccountId = parseInt(document.getElementById("studentIdHidden").value);
const currentUserName = document.getElementById("userInput").value;

// -------------------------------------------------------------
// CHỨC NĂNG CHAT CỘNG ĐỒNG
// -------------------------------------------------------------


// --- HÀM HỖ TRỢ HIỂN THỊ THỜI GIAN (MỚI) ---
function formatTime(timestamp) {
    // Chuyển đổi chuỗi ISO (từ C#) thành đối tượng Date
    const date = new Date(timestamp);
    // Format thành giờ:phút:giây Ngày/Tháng/Năm
    return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', second: '2-digit' })
        + ' ' + date.toLocaleDateString('vi-VN');
}

// 1. Lắng nghe tin nhắn từ cộng đồng
connection.on("ReceiveCommunityMessage", (user, message, timestamp) => {
    const messagesList = document.getElementById("messagesList");
    const li = document.createElement("li");

    // THÊM THỜI GIAN VÀO HIỂN THỊ
    const timeDisplay = timestamp ? ` (${formatTime(timestamp)})` : '';
    li.textContent = `[${user}]${timeDisplay}: ${message}`;

    messagesList.appendChild(li);
    messagesList.scrollTop = messagesList.scrollHeight;
});

// 2. Lắng nghe thông báo (reuse hàm từ chat.js)
connection.on("ReceiveNotification", (user, message) => {
    showPopup(`${user}: ${message}`);
    playSound();
    showBrowserNotification(`${user}: ${message}`);
});


// 3. Gắn sự kiện cho nút "Send Message"
document.getElementById("sendButton").addEventListener("click", (event) => {
    const message = document.getElementById("messageInput").value;

    if (!message) return;

    // Gọi phương thức SendCommunityMessage trên Hub Server
    connection.invoke("SendCommunityMessage", currentUserName, currentAccountId, message)
        .then(() => {
            document.getElementById("messageInput").value = "";

        })
        .catch((err) => {
            console.error("Send Message Error:", err.toString());
        });

    event.preventDefault();
});


// -------------------------------------------------------------
// KẾT NỐI VÀ TẢI LỊCH SỬ
// -------------------------------------------------------------

connection.start()
    .then(() => {
        console.log("Community Chat Hub connected");
        document.getElementById("sendButton").disabled = false;

        // Tải lịch sử chat khi kết nối thành công (CẬP NHẬT để hiển thị thời gian)
        connection.invoke("LoadHistory")
            .then(history => {
                const messagesList = document.getElementById("messagesList");
                messagesList.innerHTML = ''; // Xóa lịch sử cũ (nếu có)
                history.forEach(msg => {
                    const li = document.createElement("li");

                    // LẤY THỜI GIAN TỪ DỮ LIỆU LỊCH SỬ
                    const timeDisplay = msg.timestamp ? ` (${formatTime(msg.timestamp)})` : '';
                    li.textContent = `[${msg.senderName}]${timeDisplay}: ${msg.content}`;

                    messagesList.appendChild(li);
                });
                messagesList.scrollTop = messagesList.scrollHeight;
            })
            .catch(err => console.error("Load History Error:", err.toString()));

        if (Notification.permission !== "granted" && Notification.permission !== "denied") {
            Notification.requestPermission();
        }
    })
    .catch((err) => {
        console.error("SignalR connection error:", err.toString());
    });

// ... (Đặt các hàm showPopup, playSound, showBrowserNotification ở đây hoặc đảm bảo chúng được nhúng)
// Lưu ý: Bạn cần đảm bảo các hàm showPopup, playSound, showBrowserNotification được định nghĩa.