// groupChat.js
"use strict";

const groupConnection = new signalR.HubConnectionBuilder()
    .withUrl("/groupChatHub")
    .build();

const currentAccountId = parseInt(document.getElementById("studentIdHidden").value);
const currentUserName = document.getElementById("userInput").value;
const currentGroupId = parseInt((document.getElementById("groupIdHidden") || {}).value || "0");

function formatTime(timestamp) {
    const date = new Date(timestamp);
    return date.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit', second: '2-digit' })
        + ' ' + date.toLocaleDateString('vi-VN');
}

function escapeHtml(s) {
    return String(s).replace(/[&<>"']/g, m => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": "&#39;" }[m]));
}

// --- RECEIVE GROUP MESSAGE ---
groupConnection.on("ReceiveGroupMessage", (user, message, timestamp) => {
    const messagesList = document.getElementById("messagesList");
    const li = document.createElement("li");
    const isCurrentUser = user === currentUserName;
    const senderNameHtml = `<span class="${isCurrentUser ? 'current-user-name' : 'other-user-name'}">[${user}]</span>`;
    const timeDisplay = timestamp ? ` (${formatTime(timestamp)})` : '';
    li.className = "list-group-item";
    li.innerHTML = `${senderNameHtml}${timeDisplay}: ${escapeHtml(message)}`;
    messagesList.appendChild(li);
    messagesList.scrollTop = messagesList.scrollHeight;
});

// --- RECEIVE NOTIFICATION ---
// Thông báo được xử lý bởi global-notification.js
// Không cần listener ở đây để tránh duplicate notifications

// --- SEND MESSAGE ---
document.getElementById("sendButton")?.addEventListener("click", (event) => {
    const message = document.getElementById("messageInput").value;
    if (!message || !currentGroupId) {
        console.warn("Message or Group ID is missing.");
        return;
    }

    groupConnection.invoke("SendGroupMessage", currentGroupId, currentAccountId, currentUserName, message)
        .then(() => { document.getElementById("messageInput").value = ""; })
        .catch(err => console.error("SendGroupMessage Error:", err.toString()));

    event.preventDefault();
});

// --- ADD MEMBERS ---
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
    fetch(`/Chat/AddMembers?groupId=${groupId}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            groupName: "",
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
                    const modalElement = document.getElementById('addMemberModal');
                    const modal = bootstrap.Modal.getInstance(modalElement);
                    if (modal) modal.hide();
                }, 2000);

                // Gửi thông báo qua SignalR (Tùy chọn)
                // groupConnection.invoke("NotifyGroupOfNewMember", groupId, data.message);

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

// --- LEAVE GROUP ---
document.getElementById('btnLeaveGroup')?.addEventListener('click', function () {
    const groupId = this.getAttribute('data-group-id');
    if (!confirm("Bạn có chắc chắn muốn rời nhóm này không?")) {
        return;
    }

    fetch('/Chat/LeaveGroup', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `groupId=${groupId}`
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                alert("Bạn đã rời nhóm thành công.");
                groupConnection.stop();
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

// --- START CONNECTION AND LOAD HISTORY ---
groupConnection.start()
    .then(async () => {
        console.log(`SignalR connected to Group Hub. Attempting to join Group ${currentGroupId}`);

        if (currentGroupId) {
            try {
                await groupConnection.invoke("JoinGroup", currentGroupId);
            } catch (e) {
                console.error("Join Group Error:", e.toString());
            }
        }

        document.getElementById("sendButton").disabled = false;

        // Load history
        groupConnection.invoke("LoadHistory", currentGroupId)
            .then(history => {
                const messagesList = document.getElementById("messagesList");
                messagesList.innerHTML = '';
                history.forEach(msg => {
                    const li = document.createElement("li");
                    const isCurrentUser = msg.senderName === currentUserName;
                    const senderNameHtml = `<span class="${isCurrentUser ? 'current-user-name' : 'other-user-name'}">[${msg.senderName}]</span>`;
                    const timeDisplay = msg.timestamp ? ` (${formatTime(msg.timestamp)})` : '';
                    li.className = "list-group-item";
                    li.innerHTML = `${senderNameHtml}${timeDisplay}: ${escapeHtml(msg.content)}`;
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