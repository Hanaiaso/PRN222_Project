// communityChat.js
"use strict";

const communityConnection = new signalR.HubConnectionBuilder()
    .withUrl("/communityChatHub")
    .build();

const currentAccountId = parseInt(document.getElementById("studentIdHidden").value);
const currentUserName = document.getElementById("userInput").value;

function formatTime(timestamp) {
    const date = new Date(timestamp);
    return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', second: '2-digit' })
        + ' ' + date.toLocaleDateString('vi-VN');
}

// Hiển thị tin nhắn trong chat
communityConnection.on("ReceiveCommunityMessage", (user, message, timestamp) => {
    const messagesList = document.getElementById("messagesList");
    const li = document.createElement("li");
    const timeDisplay = timestamp ? ` (${formatTime(timestamp)})` : '';
    li.textContent = `[${user}]${timeDisplay}: ${message}`;
    messagesList.appendChild(li);
    messagesList.scrollTop = messagesList.scrollHeight;
});

// Thông báo được xử lý bởi global-notification.js
// Không cần listener ở đây để tránh duplicate notifications

// Gửi tin nhắn
document.getElementById("sendButton")?.addEventListener("click", (event) => {
    const message = document.getElementById("messageInput").value;
    if (!message) return;

    communityConnection.invoke("SendCommunityMessage", currentUserName, currentAccountId, message)
        .then(() => { document.getElementById("messageInput").value = ""; })
        .catch(err => console.error("Send Message Error:", err.toString()));

    event.preventDefault();
});

// Kết nối và load lịch sử
communityConnection.start()
    .then(() => {
        console.log("Community Chat Hub connected");
        document.getElementById("sendButton").disabled = false;

        // Load history
        communityConnection.invoke("LoadHistory")
            .then(history => {
                const messagesList = document.getElementById("messagesList");
                messagesList.innerHTML = '';
                history.forEach(msg => {
                    const li = document.createElement("li");
                    const timeDisplay = msg.timestamp ? ` (${formatTime(msg.timestamp)})` : '';
                    li.textContent = `[${msg.senderName}]${timeDisplay}: ${msg.content}`;
                    messagesList.appendChild(li);
                });
                messagesList.scrollTop = messagesList.scrollHeight;
            })
            .catch(err => console.error("Load History Error:", err.toString()));

        // Request notification permission
        if (Notification.permission !== "granted" && Notification.permission !== "denied") {
            Notification.requestPermission();
        }
    })
    .catch(err => console.error("SignalR connection error:", err.toString()));