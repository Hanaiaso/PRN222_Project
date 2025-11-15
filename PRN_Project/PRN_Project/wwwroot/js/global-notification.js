const communityConnection = new signalR.HubConnectionBuilder()
    .withUrl("/communityChatHub")
    .build();

const groupConnection = new signalR.HubConnectionBuilder()
    .withUrl("/groupChatHub")
    .build();

communityConnection.on("ReceiveNotification", (senderName, message) => {
    showToast(`${senderName}: ${message}`);
});

groupConnection.on("ReceiveNotification", (senderName, message) => {
    showToast(`${senderName}: ${message}`);
});

// Hàm hiện toast
function showToast(text) {
    let toast = document.createElement("div");
    toast.classList.add("global-toast");
    toast.innerText = text;
    document.body.appendChild(toast);

    setTimeout(() => {
        toast.classList.add("show");
    }, 100);

    setTimeout(() => {
        toast.classList.remove("show");
        toast.remove();
    }, 4000);
}

// Kết nối
communityConnection.start();
groupConnection.start();
