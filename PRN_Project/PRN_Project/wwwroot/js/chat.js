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

// 1. Client lắng nghe sự kiện "ReceivePrivateMessage" từ Hub
connection.on("ReceivePrivateMessage", (senderName, message) => {
    const messagesList = document.getElementById("messagesList");
    const li = document.createElement("li");

    // Format tin nhắn nhận được
    li.textContent = `[${senderName}]: ${message}`;

    messagesList.appendChild(li);
    messagesList.scrollTop = messagesList.scrollHeight; // Tự động cuộn

    // Kích hoạt thông báo chỉ khi tin nhắn đến TỪ NGƯỜI KHÁC
    if (senderName !== currentUserName) {
        showPopup(`${senderName}: ${message}`);
        playSound();
        showBrowserNotification(`${senderName}: ${message}`);
    }
});

// 2. Gắn sự kiện cho nút START CHAT (Tìm người dùng theo Email và tạo/tìm phòng chat)
document.getElementById("startChatButton")?.addEventListener("click", (event) => {
    event.preventDefault();
    // Lấy Email từ ô nhập Email người nhận
    const targetEmail = document.getElementById("targetEmailInput").value;

    if (!targetEmail) {
        alert("Vui lòng nhập Email người muốn chat.");
        return;
    }

    // 🛠️ SỬA LỖI: Chuẩn bị dữ liệu dưới dạng URL-encoded
    const formData = new URLSearchParams();
    formData.append('targetEmail', targetEmail);

    document.getElementById("chatStatus").textContent = "Đang tìm người dùng...";

    // Gọi API Controller với dữ liệu Form Data
    fetch('/Chat/StartPrivateChat', {
        method: 'POST',
        headers: {
            // THAY ĐỔI: Sử dụng content type cho Form Data
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: formData.toString() // Gửi dữ liệu dưới dạng chuỗi URL-encoded
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                currentTargetUserId = data.targetUserId; // Lấy ID người nhận từ Controller
                // Cập nhật trạng thái
                document.getElementById("chatStatus").textContent = `Đang chat với: ${data.targetUserName}`;
                document.getElementById("chatStatus").style.color = "green"; // Thêm màu xanh cho dễ nhìn

                document.getElementById("sendButton").disabled = false;

                // Xóa lịch sử cũ và hiển thị lịch sử chat mới
                const messagesList = document.getElementById("messagesList");
                messagesList.innerHTML = '';
                data.history.forEach(msg => {
                    const senderDisplay = msg.senderName || (msg.senderId === currentAccountId ? currentUserName : "Người lạ");
                    const li = document.createElement("li");
                    li.textContent = `[${senderDisplay}]: ${msg.content}`;
                    messagesList.appendChild(li);
                });

            } else {
                alert(data.message);
                currentTargetUserId = null; // Reset
                document.getElementById("sendButton").disabled = true;
                document.getElementById("chatStatus").textContent = "Chưa chọn người chat.";
                document.getElementById("chatStatus").style.color = "#cc3333"; // Reset màu lỗi
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

    // Gọi phương thức SendPrivateMessage trên Hub Server
    connection.invoke("SendPrivateMessage", currentAccountId, currentUserName, currentTargetUserId, message)
        .then(() => {
            // Xóa nội dung ô message sau khi gửi thành công
            document.getElementById("messageInput").value = "";
        })
        .catch((err) => {
            return console.error("SendPrivateMessage Error:", err.toString());
        });

    event.preventDefault();
});


// -------------------------------------------------------------
// CÁC HÀM HỖ TRỢ THÔNG BÁO (GIỮ NGUYÊN)
// -------------------------------------------------------------

// 4. Hàm hiển thị popup thông báo nổi trên màn hình
function showPopup(message) {
    const notif = document.createElement("div");
    notif.className = "notification-popup";
    notif.textContent = message;

    document.body.appendChild(notif);

    // Tự động xóa popup sau 4 giây
    setTimeout(() => notif.remove(), 4000);
}

// 5. Hàm phát âm thanh thông báo
function playSound() {
    const audio = new Audio("/sounds/newmessage.mp3");
    audio.play().catch((e) => {
        console.log("Cannot play sound:", e);
    });
}

// 6. Hàm hiển thị thông báo của trình duyệt (Browser Notification API)
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

// 7. Khởi động kết nối SignalR
connection.start()
    .then(() => {
        console.log("SignalR connected to Private Chat Hub");

        // GỌI HÀM REGISTER ĐỂ ÁNH XẠ ACCOUNTID VÀO CONNECTIONID TRÊN SERVER
        connection.invoke("Register", currentAccountId)
            .catch(err => console.error("Register Error:", err.toString()));

        // Yêu cầu quyền thông báo trình duyệt nếu chưa có
        if (Notification.permission !== "granted" && Notification.permission !== "denied") {
            Notification.requestPermission();
        }
    })
    .catch((err) => {
        console.error("SignalR connection error:", err.toString());
    });