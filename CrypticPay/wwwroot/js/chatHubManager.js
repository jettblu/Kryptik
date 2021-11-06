$(".msgSideClose").on('click', function () {
    $("#slideOut").sideNav('hide');
});

/*show contact search box on request*/
$(".msgNew").on('click', function () {
    $("#slideOut").sideNav('hide');
    $("#msgCreateArea").hide('slow');
    $("#msgHistoryArea").hide('slow');
    $("#msgBlankArea").hide('slow');
    $(".msgTitleText").text("");
    $("#msgSearchArea").show('fast');
});


$("#msgSearchFriendsForm").on('input', (function () {
    $("#msgSearchFriendsForm").submit();
}));


complete = function (res) {
    console.log("That was raw response");
    var result = res.responseJSON;
    $("#placeHolder").hide();
    $("#searchResults").empty();
    if (result.length == 0) {
        console.log("Zero matching friends.");
        $("#placeHolder").show();
    }
    else {
        console.log("Friends found.");
        $(result).each(function (index) {
            var currUser = result[index];
            console.log(currUser);
            var resultHtml = $(`
            <div class="row valign-wrapper card hoverable msgFriendResult" data-name="${currUser.name}" data-userName="${currUser.userName}">
              <div class="col s2">
                <img src="${currUser.profilePhotoPath}" alt="friend photo" class="circle profilePhotoSearch">
              </div>
              <div class="col s10">
                <span class="searchUserFullName">${currUser.name}</span>
                <h6 class="searchUsertag">@${currUser.userName}</h6>
              </div>
            </div>`);
            $("#searchResults").append(resultHtml);
        });
    }

};


$("#msgSearchArea").on('click', '.msgFriendResult', function () {
    $("#slideOut").sideNav('hide');
    $("#msgBlankArea").hide();
    $("#msgSearchArea").hide('slow');
    $("#searchResults").empty();
    console.log($(this).data("name"));
    $(".msgTitleText").text($(this).data("name"));
    $(".msgTitleText").text($(this).data("name"));
    $(".msgTitleText").data("userName", $(this).data("userName"));
    $("#msgHistoryArea").show('fast');
    $("#msgCreateArea").show('fast');
});




/*message handling*/

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("btnSendMsg").addEventListener("click", function (event) {
    var message = document.getElementById("msgInput").value;
    if (receiver != "") {
        connection.invoke("SendMessageToGroup", receiver, message).catch(function (err) {
            return console.error(err.toString());
        });
    }
    event.preventDefault();
});