"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();
// -------------------------------------------------------------
// THÊM CHỨC NĂNG CHAT
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

    // Xóa nội dung ô message sau khi gửi (tùy chọn)
    document.getElementById("messageInput").value = "";
});


// 2. Gắn sự kiện cho nút "Send Message"
document.getElementById("sendButton").addEventListener("click", (event) => {
    const user = document.getElementById("userInput").value;
    const message = document.getElementById("messageInput").value;

    if (!user || !message) {
        console.warn("User name and message cannot be empty.");
        return;
    }

    // Gọi phương thức SendMessage trên Hub Server
    connection.invoke("SendMessage", user, message).catch((err) => {
        return console.error(err.toString());
    });

    // Ngăn chặn hành vi mặc định của form/button
    event.preventDefault();
});

connection.start().then(() => console.log("SignalR connected"));
