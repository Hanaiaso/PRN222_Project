"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

// -------------------------------------------------------------
// CHỨC NĂNG CHAT VÀ THÔNG BÁO
// -------------------------------------------------------------

// 1. Client lắng nghe sự kiện "ReceiveMessage" từ Hub
connection.on("ReceiveMessage", (user, message) => {
    // Tìm <ul> có id là messagesList
    const messagesList = document.getElementById("messagesList");

    // Tạo phần tử <li> mới để hiển thị tin nhắn
    const li = document.createElement("li");
    li.textContent = `${user}: ${message}`;

    // Thêm tin nhắn vào danh sách
    messagesList.appendChild(li);

    // Xóa nội dung ô message sau khi gửi
    document.getElementById("messageInput").value = "";
});

// 2. Client lắng nghe sự kiện "ReceiveNotification" để hiển thị thông báo
// THAY ĐỔI: Thêm event listener mới để nhận thông báo khi có tin nhắn từ người khác
connection.on("ReceiveNotification", (user, message) => {
    // Hiển thị popup thông báo nổi
    showPopup(`${user}: ${message}`);

    // Phát âm thanh thông báo
    playSound();

    // Hiển thị thông báo trình duyệt (nếu được phép)
    showBrowserNotification(`${user}: ${message}`);
});

// 3. Gắn sự kiện cho nút "Send Message"
document.getElementById("sendButton").addEventListener("click", (event) => {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;

    // Kiểm tra message không rỗng (user có thể để trống - sẽ dùng anonymous)
    if (!message) {
        console.warn("Message cannot be empty.");
        return;
    }

    // Gọi phương thức SendMessage trên Hub Server
    connection.invoke("SendMessage", user, message).catch((err) => {
        return console.error(err.toString());
    });

    // Ngăn chặn hành vi mặc định của form/button
    event.preventDefault();
});

// -------------------------------------------------------------
// CÁC HÀM HỖ TRỢ THÔNG BÁO
// -------------------------------------------------------------

// 4. Hàm hiển thị popup thông báo nổi trên màn hình
// THÊM MỚI: Tạo popup notification dạng toast
function showPopup(message) {
    const notif = document.createElement("div");
    notif.className = "notification-popup";
    notif.textContent = message;

    document.body.appendChild(notif);

    // Tự động xóa popup sau 4 giây
    setTimeout(() => notif.remove(), 4000);
}

// 5. Hàm phát âm thanh thông báo
// THÊM MỚI: Phát âm thanh khi có tin nhắn mới
function playSound() {
    const audio = new Audio("/sounds/newmessage.mp3");
    audio.play().catch((e) => {
        console.log("Cannot play sound:", e);
    });
}

// 6. Hàm hiển thị thông báo của trình duyệt (Browser Notification API)
// THÊM MỚI: Sử dụng Notification API của browser
function showBrowserNotification(content) {
    // Nếu đã được cấp quyền, hiển thị thông báo ngay
    if (Notification.permission === "granted") {
        new Notification("Tin nhắn mới", {
            body: content,
            icon: "/img/chat.png"
        });
    }
    // Nếu chưa từ chối, yêu cầu quyền
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
// THAY ĐỔI: Thêm logic yêu cầu quyền notification khi kết nối thành công
connection.start()
    .then(() => {
        console.log("SignalR connected");

        // Yêu cầu quyền thông báo trình duyệt nếu chưa có
        if (Notification.permission !== "granted" && Notification.permission !== "denied") {
            Notification.requestPermission();
        }
    })
    .catch((err) => {
        console.error("SignalR connection error:", err.toString());
    });