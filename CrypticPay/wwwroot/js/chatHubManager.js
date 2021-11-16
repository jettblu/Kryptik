$(".msgSideClose").on('click', function () {
    $("#slideOut").sideNav('hide');
});

var clearMainArea = function () {
    $("#msgCreateArea").hide('slow');
    $("#msgBlankArea").hide('slow');
    $("#msgHistoryArea").hide('slow');
    $("#msgHistoryArea").empty();
    $("#msgSearchArea").hide('fast');
    $("#slideOut").sideNav('hide');
}


$(".msgHubContainer").on('click', ".msgSideBox", function () {
    // ADD handling for group. More than 1 member.
    $("#membersGroup").val($(this).data("members"));
    // submit hidden form so server can get group
    $("#msgNewGroupForm").submit();
    clearMainArea();
});


/*show contact search box on request*/
$(".msgNew").on('click', function () {
    $("#slideOut").sideNav('hide');
    $("#msgCreateArea").hide('slow');
    $("#msgHistoryArea").hide('slow');
    $("#msgHistoryArea").empty();
    $("#msgBlankArea").hide('slow');
    $(".msgTitleText").text("");
    $("#msgSearchArea").show('fast');
});


$("#msgSearchFriendsForm").on('input', (function () {
    console.log("Fired w/ content:");
    console.log($("#msgSearchFriendsForm").text());
    $("#msgSearchFriendsForm").submit();
}));


//set scroll to bottom of page
var messageBody = document.querySelector('#msgHistoryArea');
messageBody.scrollTop = messageBody.scrollHeight - messageBody.clientHeight;

var setScroll = function () {
    var messageBody = document.querySelector('#msgHistoryArea');
    messageBody.scrollTop = messageBody.scrollHeight - messageBody.clientHeight;
}

completeSearch = function (res) {
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
            <div class="row valign-wrapper card hoverable msgFriendResult" data-name="${currUser.name}" data-username="${currUser.userName}">
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
    console.log($(this).data("name"));
    // eventually switch name additions to on complete??
    $(".msgTitleText").text($(this).data("name"));
    console.log($(this).data("username"));
    $(".msgTitleText").data("username", $(this).data("username"));
    // ADD handling for group. More than 1 member.
    $("#membersGroup").val($(this).data("username"));
    // submit hidden form so server can get group
    $("#msgNewGroupForm").submit();
});

var addMessageIn = function (txt) {
    console.log("Adding: ");
    console.log(txt);
    var msgIn = $(`
        <div class="row">
            <div class="col offset-l4 l8 s10 offset-s2">
              <p class="msgText rounded msgBox">${txt}</p>
            </div>
        </div>`);
    $("#msgHistoryArea").append(msgIn);
}

var addMessageOut = function (txt) {
    var msgOut = $(`<div class="row">
      <div class="col offset-l9 offset-s8 l3 s4">
        <p class="msgText rounded msgBox" style="border: solid 1px;">${txt}</p>
      </div>
    </div>`);
    $("#msgHistoryArea").append(msgOut);
}

// async. completion for new group
completeNewGroup = function (res) {
    
    // append messages formatted by server
    var result = res.responseText;
    $("#msgHistoryArea").empty();
    $("#msgHistoryArea").append(result);
    // ADD decryption on client side

    // hide non-msg history elements
    $("#slideOut").sideNav('hide');
    $("#msgBlankArea").hide();
    $("#msgSearchArea").hide('slow');
    $("#searchResults").empty();
    
    // show msg. history for group
    $("#msgHistoryArea").show('fast');
    $("#msgCreateArea").show('fast');
    setScroll();
}


/*message handling*/

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("btnSendMsg").disabled = true;

connection.on("ReceiveMessage", function (user, message, groupId) {
    // $("#msgHistory").find("#msgHistoryPlaceHolder").hide();
    var metaTag = $("#msgMeta");
    var thisGroupId = metaTag.data("group");
    if (groupId == thisGroupId) {
        addMessageIn(message);
    }
    setScroll();
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
});

connection.start().then(function () {
    document.getElementById("btnSendMsg").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("btnSendMsg").addEventListener("click", function (event) {
    // $("#msgHistory").find("#msgHistoryPlaceHolder").hide();
    var message = document.getElementById("msgInput").value;
    addMessageOut(message);
    setScroll();
    var receiver = $(".msgTitleText").data("username");
    // get group id from meta div
    var metaTag = $("#msgMeta");
    var groupId = metaTag.data("group");
    if (receiver != "") {
        connection.invoke("SendMessageToGroup", receiver, message, groupId).catch(function (err) {
            return console.error(err.toString());
        });
    }
    // clear text content of message creation box
    $("#msgInput").val("");
    event.preventDefault();
});