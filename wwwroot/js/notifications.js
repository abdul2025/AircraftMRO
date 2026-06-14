window.notify = {

    success(message) {
        showToast('Success', message);
    },

    warning(message) {
        showToast('Warning', message);
    },

    error(message) {
        showToast('Error', message);
    },

    info(message) {
        showToast('Information', message);
    }
};

function showToast(title, message, duration = 5000) {

    const toastElement =
        document.getElementById('appToast');

    if (!toastElement)
        return;

    document.getElementById('toastTitle').textContent = title;
    document.getElementById('toastBody').textContent = message;

    const toast = bootstrap.Toast.getOrCreateInstance(toastElement, {
        delay: duration,
        autohide: duration !== 0 // If duration is 0, autohide is false, which no auto hide
    });

    toast.show();
}


// ==========================================
// SIGNAL-R REAL-TIME NOTIFICATION
// ==========================================
document.addEventListener('DOMContentLoaded', () => {
    
    // Only establish a connection if the global signalR library script is loaded on the layout
    if (typeof signalR === 'undefined') {
        console.warn('SignalR library not detected. Live toast listeners skipped.');
        return;
    }

    // Initialize the WebSocket connection to the hub path from Program.cs
    const notificationConnection = new signalR.HubConnectionBuilder()
        .withUrl("/notificationHub")
        .withAutomaticReconnect([0, 2000, 5000, 10000]) // Custom auto-retry intervals
        .build();

    // Catch the exact string payload emitted by your RealTimeInAppChannel C# class
    notificationConnection.on("ReceiveNotification", function (data) {
        console.log("Real-time notification received:", data);

        const title = data.title || "Information";
        const message = data.message || "";

        showToast(title, message, 4000);
    });

    // Fire up the real-time pipeline connection loop
    notificationConnection.start()
        .then(() => console.log("Successfully connected to live notification socket pipeline."))
        .catch(err => console.error("Real-time hub handshake breakdown: ", err.toString()));
});