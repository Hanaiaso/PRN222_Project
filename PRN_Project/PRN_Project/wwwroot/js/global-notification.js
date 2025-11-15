// global-notification.js
"use strict";

/*
  Central notification helper + small "notification listener" for community & group hubs.
  - Exposes window.NotificationHelpers: showPopup, playSound, showBrowserNotification, isEnabled, toggle
  - Connects to Community & Group hubs for global ReceiveNotification events.
  - If #groupIdHidden exists on page, will JoinGroup on group hub so it receives group notifications.
*/

(function () {
    // -------------------------
    // Notification Helpers API
    // -------------------------
    const SOUND_URL = "/sounds/newmessage.mp3";
    const ICON_URL = "/img/chat.png";

    const NotificationHelpers = {
        isEnabled() {
            const v = localStorage.getItem("notificationsEnabled");
            return v === null ? true : v === "true";
        },
        setEnabled(val) {
            localStorage.setItem("notificationsEnabled", val ? "true" : "false");
        },
        showPopup(message) {
            if (!NotificationHelpers.isEnabled()) return;
            const notif = document.createElement("div");
            notif.className = "notification-popup global";
            notif.textContent = message;
            document.body.appendChild(notif);
            // simple CSS fade in/out: you can style .notification-popup.global in CSS
            setTimeout(() => notif.classList.add("show"), 50);
            setTimeout(() => {
                notif.classList.remove("show");
                setTimeout(() => notif.remove(), 300);
            }, 4000);
        },
        playSound() {
            if (!NotificationHelpers.isEnabled()) return;
            try {
                const audio = new Audio(SOUND_URL);
                audio.play().catch(() => { /* ignore autoplay errors */ });
            } catch (e) { /* ignore */ }
        },
        showBrowserNotification(title, body) {
            if (!NotificationHelpers.isEnabled()) return;
            if (!("Notification" in window)) return;
            if (Notification.permission === "granted") {
                new Notification(title, { body, icon: ICON_URL });
            } else if (Notification.permission !== "denied") {
                Notification.requestPermission().then(p => {
                    if (p === "granted") {
                        new Notification(title, { body, icon: ICON_URL });
                    }
                });
            }
        },
        toggle() {
            const newVal = !NotificationHelpers.isEnabled();
            NotificationHelpers.setEnabled(newVal);
            return newVal;
        }
    };

    window.NotificationHelpers = NotificationHelpers;

    // -------------------------
    // Small toast manager (visual)
    // -------------------------
    function showToast(text) {
        if (!NotificationHelpers.isEnabled()) return;
        const toast = document.createElement("div");
        toast.className = "global-toast";
        toast.innerText = text;
        document.body.appendChild(toast);
        // CSS animation classes assumed
        setTimeout(() => toast.classList.add("show"), 100);
        setTimeout(() => {
            toast.classList.remove("show");
            setTimeout(() => toast.remove(), 300);
        }, 4000);
    }

    // -------------------------
    // Connections to Hubs for global notifications
    // -------------------------
    const communityNotifConnection = new signalR.HubConnectionBuilder()
        .withUrl("/communityChatHub")
        .build();

    const groupNotifConnection = new signalR.HubConnectionBuilder()
        .withUrl("/groupChatHub")
        .build();

    // event handlers
    communityNotifConnection.on("ReceiveNotification", (senderName, message, timestamp) => {
        const text = `${senderName}: ${message}`;
        showToast(text);
        NotificationHelpers.showPopup ? NotificationHelpers.showPopup(text) : null;
        NotificationHelpers.playSound();
        NotificationHelpers.showBrowserNotification("Thông báo", text);
    });

    groupNotifConnection.on("ReceiveNotification", (senderName, message, timestamp) => {
        const text = `${senderName}: ${message}`;
        showToast(text);
        NotificationHelpers.showPopup ? NotificationHelpers.showPopup(text) : null;
        NotificationHelpers.playSound();
        NotificationHelpers.showBrowserNotification("Thông báo nhóm", text);
    });

    // start both connections (fire-and-forget)
    communityNotifConnection.start()
        .then(() => console.log("Notification listener connected to Community Hub"))
        .catch(err => console.error("Community notif conn error:", err.toString()));

    groupNotifConnection.start()
        .then(async () => {
            console.log("Notification listener connected to Group Hub");
            // If there's a groupId on page, join that group so notifications are delivered
            const g = document.getElementById("groupIdHidden");
            if (g && g.value) {
                const gid = parseInt(g.value);
                try {
                    await groupNotifConnection.invoke("JoinGroup", gid);
                    console.log("Notification listener joined Group:", gid);
                } catch (e) {
                    console.error("Notification listener JoinGroup error:", e.toString());
                }
            }
        })
        .catch(err => console.error("Group notif conn error:", err.toString()));

})();
