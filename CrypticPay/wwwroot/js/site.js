// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

window.addEventListener('offline', function () {
    var pageAll = document.getElementById("allContent"); 
    var tag = document.createElement("h4");
    var text = document.createTextNode("Sorry, Kryptik App requires an internet connection to work.");
    tag.setAttribute("id", "connectionStatus");
    tag.appendChild(text);
    pageAll.appendChild(tag);
    var content = document.getElementById("pageContent"); 
    content.style.display = "none";;
});

window.addEventListener('online', function () {
    var statusTag = document.getElementById("connectionStatus");
    if (statusTag == null) {
        // do nothing
    }
    else {
        // reshow page content
        location.reload();
    }
});

// add current user's username to session storage
sessionStorage["uname"] = $("#metaUserCurrent").data("uname");
