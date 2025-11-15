"use strict";

// Định nghĩa Hub Connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/groupChatHub") // Kết nối đến Group Chat Hub
    .build();

// Lấy thông tin người dùng và nhóm từ View (Được đặt trong input ẩn)
const currentAccountId = parseInt(document.getElementById("studentIdHidden").value);
const currentUserName = document.getElementById("userInput").value;
const currentGroupId = document.getElementById("groupIdHidden").value;

function formatTime(timestamp) {
    // Chuyển đổi chuỗi ISO (từ C#) thành đối tượng Date
    const date = new Date(timestamp);
    // Format thành giờ:phút:giây Ngày/Tháng/Năm
    return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', second: '2-digit' })
        + ' ' + date.toLocaleDateString('vi-VN');
}

// --- 1. LẮNG NGHE SỰ KIỆN TỪ HUB (CẬP NHẬT) ---

// Lắng nghe tin nhắn từ nhóm
connection.on("ReceiveGroupMessage", (user, message, timestamp) => {
    const messagesList = document.getElementById("messagesList");
    const li = document.createElement("li");

    // Logic color: So sánh tên người gửi (user) với tên người dùng hiện tại
    const isCurrentUser = user === currentUserName;
    const senderNameHtml = `<span class="${isCurrentUser ? 'current-user-name' : 'other-user-name'}">[${user}]</span>`;

    // Thêm thời gian vào hiển thị
    const timeDisplay = timestamp ? ` (${formatTime(timestamp)})` : '';

    li.className = "list-group-item";
    li.innerHTML = `${senderNameHtml}${timeDisplay}: ${message}`; // Sử dụng innerHTML để chèn thẻ span

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


// --- 2. XỬ LÝ GỬI TIN NHẮN (GIỮ NGUYÊN) ---

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
            // Tin nhắn sẽ được hiển thị khi Hub phát sóng lại thông qua ReceiveGroupMessage
            document.getElementById("messageInput").value = "";
        })
        .catch((err) => {
            console.error("SendGroupMessage Error:", err.toString());
        });

    event.preventDefault();
});
document.getElementById('btnAddMembersConfirm')?.addEventListener('click', function () {
    const emailsString = document.getElementById('newMemberEmailsInput').value.trim();
    const statusDiv = document.getElementById('addMemberStatus');
    const groupId = parseInt(document.getElementById("groupIdHidden").value);

    if (emailsString === "") {
        statusDiv.textContent = "Vui lòng nhập ít nhất một Email.";
        statusDiv.style.color = 'red';
        return;
    }

    const memberEmails = emailsString.split(',').map(email => email.trim()).filter(email => email !== "");

    if (memberEmails.length === 0) {
        statusDiv.textContent = "Email không hợp lệ.";
        statusDiv.style.color = 'red';
        return;
    }

    statusDiv.textContent = "Đang thêm thành viên...";
    statusDiv.style.color = 'blue';

    // Gửi yêu cầu AJAX
    fetch(`/Chat/AddMembers?groupId=${groupId}`, { // Thêm groupId vào query string
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ // Gửi dữ liệu theo cấu trúc DTO (GroupName không cần, nhưng vẫn phải gửi list)
            groupName: "", // Gửi rỗng để khớp với DTO
            memberEmails: memberEmails
        })
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                statusDiv.textContent = data.message;
                statusDiv.style.color = 'green';

                // Xóa nội dung input và tự động đóng modal sau 2s
                document.getElementById('newMemberEmailsInput').value = "";
                setTimeout(() => {
                    // Dùng API Bootstrap để đóng modal
                    const modalElement = document.getElementById('addMemberModal');
                    const modal = bootstrap.Modal.getInstance(modalElement);
                    if (modal) modal.hide();
                }, 2000);

                // Gửi thông báo qua SignalR (Tùy chọn: thông báo cho nhóm)
                // connection.invoke("NotifyGroupOfNewMember", groupId, data.message);

            } else {
                statusDiv.textContent = `Lỗi: ${data.message}`;
                statusDiv.style.color = 'red';
            }
        })
        .catch(error => {
            statusDiv.textContent = "Lỗi kết nối Server.";
            statusDiv.style.color = 'red';
            console.error('Error adding members:', error);
        });
});
document.getElementById('btnLeaveGroup')?.addEventListener('click', function () {
    const groupId = this.getAttribute('data-group-id');

    if (!confirm("Bạn có chắc chắn muốn rời nhóm này không?")) {
        return;
    }

    // Gửi yêu cầu rời nhóm
    fetch('/Chat/LeaveGroup', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded'
        },
        body: `groupId=${groupId}` // Gửi dữ liệu dưới dạng form data
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert("Bạn đã rời nhóm thành công.");

                // NGẮT KẾT NỐI VÀ CHUYỂN HƯỚNG:
                connection.stop();
                // Chuyển hướng về trang danh sách nhóm
                window.location.href = '/Chat/GroupList';
            } else {
                alert(`Lỗi: ${data.message}`);
            }
        })
        .catch(error => {
            console.error('Lỗi rời nhóm:', error);
            alert('Lỗi kết nối khi cố gắng rời nhóm.');
        });
});
// --- 3. KẾT NỐI VÀ TẢI LỊCH SỬ (CẬP NHẬT) ---

connection.start()
    .then(() => {
        console.log(`SignalR connected to Group Hub. Attempting to join Group ${currentGroupId}`);
        const groupId = document.getElementById("groupIdHidden").value;

        // BẬT NÚT GỬI SAU KHI KẾT NỐI THÀNH CÔNG
        document.getElementById("sendButton").disabled = false;

        // Tham gia Group SignalR bằng Group ID
        connection.invoke("JoinGroup", parseInt(currentGroupId))
            .catch(err => console.error("Join Group Error:", err.toString()));

        // TẢI LỊCH SỬ CHAT
        connection.invoke("LoadHistory", parseInt(groupId))
            .then(history => {
                const messagesList = document.getElementById("messagesList");
                messagesList.innerHTML = ''; // Xóa bất kỳ placeholder nào

                history.forEach(msg => {
                    const li = document.createElement("li");

                    // Logic color: So sánh tên người gửi
                    const isCurrentUser = msg.senderName === currentUserName;
                    const senderNameHtml = `<span class="${isCurrentUser ? 'current-user-name' : 'other-user-name'}">[${msg.senderName}]</span>`;

                    // Thêm thời gian vào hiển thị
                    const timeDisplay = msg.timestamp ? ` (${formatTime(msg.timestamp)})` : '';

                    li.className = "list-group-item";
                    li.innerHTML = `${senderNameHtml}${timeDisplay}: ${msg.content}`;

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