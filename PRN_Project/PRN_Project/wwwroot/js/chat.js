// chat.js
"use strict";

/*
  Private chat client. Connects to /privateChatHub and uses Register(accountId).
  Uses window.NotificationHelpers if available for popups/sound/notifications.
*/

const privateConnection = new signalR.HubConnectionBuilder()
    .withUrl("/privateChatHub")
    .build();

const currentAccountId = parseInt(document.getElementById("studentIdHidden").value);
const currentUserName = document.getElementById("userInput").value;
let currentTargetUserId = null;

// helper render
function formatTime(timestamp) {
    const date = new Date(timestamp);
    return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', second: '2-digit' })
        + ' ' + date.toLocaleDateString('vi-VN');
}
function renderMessage(user, message, timestamp, messagesList, senderId = null) {
    const li = document.createElement("li");
    const isCurrentUser = user === currentUserName || senderId === currentAccountId;
    const senderNameHtml = `<span class="${isCurrentUser ? 'current-user-name' : 'other-user-name'}">[${user}]</span>`;
    const timeDisplay = timestamp ? ` (${formatTime(timestamp)})` : '';
    li.innerHTML = `${senderNameHtml}${timeDisplay}: ${escapeHtml(message)}`;
    li.className = "list-group-item";
    messagesList.appendChild(li);
    messagesList.scrollTop = messagesList.scrollHeight;
}
function escapeHtml(s) {
    return String(s).replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": "&#39;" }[m]));
}

// ReceivePrivateMessage
privateConnection.on("ReceivePrivateMessage", (senderName, message, timestamp) => {
    const messagesList = document.getElementById("messagesList");
    renderMessage(senderName, message, timestamp, messagesList);

    if (senderName !== currentUserName) {
        if (window.NotificationHelpers) {
            window.NotificationHelpers.showPopup(`${senderName}: ${message}`);
            window.NotificationHelpers.playSound();
            window.NotificationHelpers.showBrowserNotification(senderName, message);
        } else {
            // fallback simple popup
            alert(`${senderName}: ${message}`);
        }
    }
});

// Start private chat (search by email)
document.getElementById("startChatButton")?.addEventListener("click", (event) => {
    event.preventDefault();
    const targetEmail = document.getElementById("targetEmailInput").value;
    if (!targetEmail) { alert("Vui lòng nhập Email người muốn chat."); return; }

    const formData = new URLSearchParams();
    formData.append('targetEmail', targetEmail);
    document.getElementById("chatStatus").textContent = "Đang tìm người dùng...";

    fetch('/Chat/StartPrivateChat', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: formData.toString()
    })
        .then(r => r.json())
        .then(data => {
            if (data.success) {
                currentTargetUserId = data.targetUserId;
                document.getElementById("chatStatus").textContent = `Đang chat với: ${data.targetUserName}`;
                document.getElementById("chatStatus").style.color = "green";
                document.getElementById("sendButton").disabled = false;

                const messagesList = document.getElementById("messagesList");
                messagesList.innerHTML = '';
                data.history.forEach(msg => {
                    const senderDisplay = msg.senderName || (msg.senderId === currentAccountId ? currentUserName : "Người lạ");
                    renderMessage(senderDisplay, msg.content, msg.timestamp, messagesList, msg.senderId);
                });
            } else {
                alert(data.message);
                currentTargetUserId = null;
                document.getElementById("sendButton").disabled = true;
                document.getElementById("chatStatus").textContent = "Chưa chọn người chat.";
                document.getElementById("chatStatus").style.color = "#cc3333";
            }
        })
        .catch(err => console.error("Start Chat Error:", err));
});

// Send message
document.getElementById("sendButton")?.addEventListener("click", (event) => {
    const message = document.getElementById("messageInput").value;
    if (!message || currentTargetUserId === null) {
        console.warn("Message or Target User is missing.");
        return;
    }

    privateConnection.invoke("SendPrivateMessage", currentAccountId, currentUserName, currentTargetUserId, message)
        .then(() => { document.getElementById("messageInput").value = ""; })
        .catch(err => console.error("SendPrivateMessage Error:", err.toString()));

    event.preventDefault();
});

// start connection & register
privateConnection.start()
    .then(() => {
        console.log("SignalR connected to Private Chat Hub");
        // call Register on server so PrivateChatHub stores mapping
        privateConnection.invoke("Register", currentAccountId).catch(e => console.error("Register Error:", e.toString()));
        // request browser notification perm if needed
        if (Notification.permission !== "granted" && Notification.permission !== "denied") {
            Notification.requestPermission();
        }
    })
    .catch(err => console.error("Private Chat SignalR connection error:", err.toString()));
