"use strict";

// 1. THAY ĐỔI: Kết nối đến Private Chat Hub
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/privateChatHub")
    .build();

// Lấy Account ID của người dùng hiện tại từ input ẩn
const currentAccountId = parseInt(document.getElementById("studentIdHidden").value);
// Lấy Tên người dùng hiện tại từ input readonly
const currentUserName = document.getElementById("userInput").value;
let currentTargetUserId = null;

// -------------------------------------------------------------
// CHỨC NĂNG CHAT CÁ NHÂN VÀ THÔNG BÁO
// -------------------------------------------------------------

function formatTime(timestamp) {
    // Chuyển đổi chuỗi ISO (từ C#) thành đối tượng Date
    const date = new Date(timestamp);
    // Format thành giờ:phút:giây Ngày/Tháng/Năm
    return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', second: '2-digit' })
        + ' ' + date.toLocaleDateString('vi-VN');
}

// Hàm hỗ trợ render tin nhắn (cho cả lịch sử và tin nhắn mới)
function renderMessage(user, message, timestamp, messagesList, senderId = null) {
    const li = document.createElement("li");

    // Logic color: So sánh tên người gửi
    const isCurrentUser = user === currentUserName || senderId === currentAccountId; // Kiểm tra bằng tên hoặc ID
    const senderNameHtml = `<span class="${isCurrentUser ? 'current-user-name' : 'other-user-name'}">[${user}]</span>`;

    const timeDisplay = timestamp ? ` (${formatTime(timestamp)})` : '';

    // Sử dụng innerHTML để chèn thẻ span CSS
    li.innerHTML = `${senderNameHtml}${timeDisplay}: ${message}`;
    li.className = "list-group-item"; // Thêm class Bootstrap cho đẹp hơn

    messagesList.appendChild(li);
    messagesList.scrollTop = messagesList.scrollHeight;
}


// 1. Client lắng nghe sự kiện "ReceivePrivateMessage" từ Hub
connection.on("ReceivePrivateMessage", (senderName, message, timestamp) => {
    const messagesList = document.getElementById("messagesList");

    // ✅ GỌI HÀM RENDER TIN NHẮN MỚI
    renderMessage(senderName, message, timestamp, messagesList);

    // Kích hoạt thông báo chỉ khi tin nhắn đến TỪ NGƯỜI KHÁC
    if (senderName !== currentUserName) {
        showPopup(`${senderName}: ${message}`);
        playSound();
        showBrowserNotification(`${senderName}: ${message}`);
    }
});

// 2. Gắn sự kiện cho nút START CHAT (Tìm người dùng theo Email và tải lịch sử)
document.getElementById("startChatButton")?.addEventListener("click", (event) => {
    event.preventDefault();
    const targetEmail = document.getElementById("targetEmailInput").value;

    if (!targetEmail) {
        alert("Vui lòng nhập Email người muốn chat.");
        return;
    }

    const formData = new URLSearchParams();
    formData.append('targetEmail', targetEmail);

    document.getElementById("chatStatus").textContent = "Đang tìm người dùng...";

    fetch('/Chat/StartPrivateChat', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: formData.toString()
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                currentTargetUserId = data.targetUserId;
                document.getElementById("chatStatus").textContent = `Đang chat với: ${data.targetUserName}`;
                document.getElementById("chatStatus").style.color = "green";

                document.getElementById("sendButton").disabled = false;

                // Xóa lịch sử cũ và hiển thị lịch sử chat mới
                const messagesList = document.getElementById("messagesList");
                messagesList.innerHTML = '';

                // ✅ RENDER LỊCH SỬ CHAT
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


// 3. Gắn sự kiện cho nút "Send Message" (Gửi tin nhắn cá nhân)
document.getElementById("sendButton").addEventListener("click", (event) => {
    const message = document.getElementById("messageInput").value;

    if (!message || currentTargetUserId === null) {
        console.warn("Message or Target User is missing.");
        return;
    }

    connection.invoke("SendPrivateMessage", currentAccountId, currentUserName, currentTargetUserId, message)
        .then(() => {
            // Xóa nội dung ô message sau khi gửi thành công
            document.getElementById("messageInput").value = "";
            // KHÔNG TỰ ECHO: Tin nhắn sẽ được hiển thị qua ReceivePrivateMessage
        })
        .catch((err) => {
            return console.error("SendPrivateMessage Error:", err.toString());
        });

    event.preventDefault();
});


// -------------------------------------------------------------
// CÁC HÀM HỖ TRỢ THÔNG BÁO (GIỮ NGUYÊN)
// -------------------------------------------------------------

function showPopup(message) {
    const notif = document.createElement("div");
    notif.className = "notification-popup";
    notif.textContent = message;
    document.body.appendChild(notif);
    setTimeout(() => notif.remove(), 4000);
}

function playSound() {
    const audio = new Audio("/sounds/newmessage.mp3");
    audio.play().catch((e) => {
        console.log("Cannot play sound:", e);
    });
}

function showBrowserNotification(content) {
    if (Notification.permission === "granted") {
        new Notification("Tin nhắn mới", {
            body: content,
            icon: "/img/chat.png"
        });
    }
    else if (Notification.permission !== "denied") {
        Notification.requestPermission().then((permission) => {
            if (permission === "granted") {
                new Notification("Tin nhắn mới", {
                    body: content,
                    icon: "/img/chat.png"
                });
            }
        });
    }
}


// -------------------------------------------------------------
// KẾT NỐI SIGNALR
// -------------------------------------------------------------

connection.start()
    .then(() => {
        console.log("SignalR connected to Private Chat Hub");

        connection.invoke("Register", currentAccountId)
            .catch(err => console.error("Register Error:", err.toString()));

        if (Notification.permission !== "granted" && Notification.permission !== "denied") {
            Notification.requestPermission();
        }
    })
    .catch((err) => {
        console.error("SignalR connection error:", err.toString());
    });