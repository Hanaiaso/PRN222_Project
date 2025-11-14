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

// 1. Lắng nghe tin nhắn từ cộng đồng
connection.on("ReceiveCommunityMessage", (user, message) => {
    const messagesList = document.getElementById("messagesList");
    const li = document.createElement("li");
    li.textContent = `[${user}]: ${message}`;
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

        // Tải lịch sử chat khi kết nối thành công
        connection.invoke("LoadHistory")
            .then(history => {
                const messagesList = document.getElementById("messagesList");
                messagesList.innerHTML = ''; // Xóa lịch sử cũ (nếu có)
                history.forEach(msg => {
                    const li = document.createElement("li");
                    li.textContent = `[${msg.senderName}]: ${msg.content}`;
                    messagesList.appendChild(li);
                });
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