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
    console.log(text);
    dummy.value = text;
    dummy.select();
    document.execCommand('copy');
    document.body.removeChild(dummy);
    createNotification();
    console.log("Address Copied!");
}