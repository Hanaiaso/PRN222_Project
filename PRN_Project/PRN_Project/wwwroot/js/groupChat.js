"use strict";

// Định nghĩa Hub Connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/groupChatHub") // Kết nối đến Group Chat Hub
    .build();

// Lấy thông tin người dùng và nhóm từ View (Được đặt trong input ẩn)
const currentAccountId = parseInt(document.getElementById("studentIdHidden").value);
const currentUserName = document.getElementById("userInput").value;
const currentGroupId = document.getElementById("groupIdHidden").value;

// --- 1. LẮNG NGHE SỰ KIỆN TỪ HUB ---

// Lắng nghe tin nhắn từ nhóm
connection.on("ReceiveGroupMessage", (user, message) => {
    const messagesList = document.getElementById("messagesList");
    const li = document.createElement("li");

    // Format tin nhắn
    li.className = "list-group-item";
    li.textContent = `[${user}]: ${message}`;

    messagesList.appendChild(li);
    messagesList.scrollTop = messagesList.scrollHeight; // Tự động cuộn xuống
});

// Lắng nghe thông báo (Sử dụng lại các hàm notification đã có)
connection.on("ReceiveNotification", (user, message) => {
    // Kích hoạt các hàm thông báo đã được định nghĩa trong file khác hoặc trong View
    showPopup(`${user}: ${message}`);
    playSound();
    showBrowserNotification(`${user}: ${message}`);
});


// --- 2. XỬ LÝ GỬI TIN NHẮN ---

document.getElementById("sendButton").addEventListener("click", (event) => {
    const message = document.getElementById("messageInput").value;

    if (!message || !currentGroupId) {
        console.warn("Message or Group ID is missing.");
        return;
    }

    // Gọi phương thức SendGroupMessage trên Hub Server
    // Các tham số: ID nhóm, ID người gửi, Tên người gửi, Tin nhắn
    connection.invoke("SendGroupMessage", parseInt(currentGroupId), currentAccountId, currentUserName, message)
        .then(() => {
            // Xóa nội dung ô message sau khi gửi thành công
            document.getElementById("messageInput").value = "";
        })
        .catch((err) => {
            console.error("SendGroupMessage Error:", err.toString());
        });

    event.preventDefault();
});


// --- 3. KẾT NỐI VÀ THAM GIA NHÓM ---

connection.start()
    .then(() => {
        console.log(`SignalR connected to Group Hub. Attempting to join Group ${currentGroupId}`);
        const groupId = document.getElementById("groupIdHidden").value;
        // BẬT NÚT GỬI SAU KHI KẾT NỐI THÀNH CÔNG
        document.getElementById("sendButton").disabled = false;

        // Tham gia Group SignalR bằng Group ID (Đây là lý do bạn không ấn được nút gửi trước đó)
        connection.invoke("JoinGroup", parseInt(currentGroupId))
            .catch(err => console.error("Join Group Error:", err.toString()));

        // TẢI LỊCH SỬ CHAT (Chưa được triển khai trong Hub, nhưng để đây nếu bạn thêm vào)
        connection.invoke("LoadHistory", parseInt(groupId))
            .then(history => {
                const messagesList = document.getElementById("messagesList");
                messagesList.innerHTML = ''; // Xóa bất kỳ placeholder nào

                history.forEach(msg => {
                    const li = document.createElement("li");
                    // Sử dụng senderName từ dữ liệu trả về của Hub
                    li.textContent = `[${msg.senderName}]: ${msg.content}`;
                    messagesList.appendChild(li);
                });
                messagesList.scrollTop = messagesList.scrollHeight; // Cuộn xuống dưới
            })
            .catch(err => console.error("Load History Error:", err.toString()));

        // Yêu cầu quyền thông báo trình duyệt nếu cần
        if (Notification.permission !== "granted" && Notification.permission !== "denied") {
            Notification.requestPermission();
        }
    })
    .catch((err) => {
        console.error("SignalR connection error:", err.toString());
    });

// --- LƯU Ý: CÁC HÀM HỖ TRỢ PHẢI ĐƯỢC NHÚNG TRƯỚC ---
// Đảm bảo các hàm showPopup, playSound, showBrowserNotification
// đã được định nghĩa trong file khác và được nhúng trước groupChat.js