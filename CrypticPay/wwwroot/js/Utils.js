$("#balContainer").on("click", ".selectable",function () {
    console.log("clicked!");
    copyToClip($(this).data("dress"));
});


const toasts = document.getElementById("toasts");


const copiedMessage = "Link Copied";


const createNotification = (message = null, type = null) => {
    const notif = document.createElement("div");
    notif.classList.add("toast");
    notif.classList.add(type ? type : 'info');
    notif.innerText = message ? message : copiedMessage;
    toasts.appendChild(notif);
    setTimeout(() => notif.remove(), 2000);
};

function copyToClip(toCopy) {
    var dummy = document.createElement('input');
    var text = toCopy;
    document.body.appendChild(dummy);
    console.log("Copied: ");
    console.log(text);
    dummy.value = text;
    dummy.select();
    document.execCommand('copy');
    document.body.removeChild(dummy);
}

// toggles password visibility
function showPassword() {
    var pwordDisplay = document.getElementById("pword");
    var pwordConfirmDisplay = document.getElementById("pwordConfirm");
    if (pwordDisplay.type === "password") {
        pwordDisplay.type = "text";
        pwordConfirmDisplay.type = "text";
    } else {
        pwordDisplay.type = "password";
        pwordConfirmDisplay.type = "password";
    }
}

// copy data to clipboard when selectable class is clicked
$("#pageContent").on('click', ".selectable", function () {
    var toCopy = $(this).data("tocopy");
    console.log($(this).data());
    console.log(toCopy);
    copyToClip(toCopy);
    // notify user content has been copied
    createNotification("Address Copied", "info");
    console.log("Address Copied!");
});
