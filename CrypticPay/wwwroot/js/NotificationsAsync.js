function ShowNotification(message) {
    $("#statusHolder").empty();
    var statusMessageClass = "success";

    if (message.startsWith("Error")) {
        statusMessageClass = "error";
    }

    if (message.startsWith("Info")) {
        statusMessageClass = "info";
    }

    console.log(statusMessageClass);

    $("#statusHolder").append(`<div id="toasts" class="center">
        <div class="toast ${statusMessageClass}">
            ${message}
        </div>
            </div>`
    );

    notif = document.getElementById("toasts");
    setTimeout(() => notif.remove(), 3100);
}