
    const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .build();

    connection.on("ReceiveNotification", function (data) {

        const box = document.getElementById("notifyBox");

    const item = document.createElement("div");
    item.classList.add("alert", "alert-info", "shadow");
    item.style.cursor = "pointer";
    item.style.marginBottom = "10px";

    item.innerHTML = `
    <strong>${data.title}</strong><br />
    <small>${data.sentTime}</small>
    `;

    // Khi click => chuyển hướng đến thông báo chi tiết
    item.onclick = function () {
        window.location.href = "/Notification/ViewNotification/" + data.id;
        };

    box.appendChild(item);

        setTimeout(() => item.remove(), 5000);
    });

    connection.start().catch(error => console.error(error));
