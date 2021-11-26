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
    console.log("Side box hit");
    console.log($(this).data("members"));
    console.log($(this).data("nametitle"));
    $(".msgTitleText").text($(this).data("nametitle"));
    $(".msgTitleText").data("username", $(this).data("members"));
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
              <p class="msgText rounded msgBox msgIn">${txt}</p>
            </div>
        </div>`);
    $("#msgHistoryArea").append(msgIn);
}

var addMessageOut = function (txt) {
    var msgOut = $(`<div class="row">
      <div class="col offset-l9 offset-s8 l3 s4">
        <p class="msgText rounded msgBox msgOut" style="border: solid 1px;">${txt}</p>
      </div>
    </div>`);
    $("#msgHistoryArea").append(msgOut);
}

// async. completion for new group
completeNewGroup = function (res) {
    var metaTag = $("#msgMeta");
    var isEncrypted = metaTag.data("encrypted");
   
    console.log("Completing msg history");
    // append messages formatted by server
    
    var result = res.responseText;
    if (isEncrypted) var result = decryptGroupInit(result);
    // decrypt each message
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


var updateBoxesSmall = function (groupId, sideBox) {
    var exists = false;
    console.log("Updating big");
    $("#msgHubBoxesBig").children('.msgSideBox').each(function () {
        console.log($(this));
        if ($(this).data('group') == groupId) {
            console.log("Side Box exists.");
            $(this).prependTo($("#msgHubBoxesSmall"));
            exists = true;
            // break the each statement
            return false;
        }
    });
    // if sideBox doesn't exist add in new
    if (!exists) {
        console.log("Side box doesn't exist.");
        $(".msgHubPlaceHolder").hide();
        $("#msgHubBoxesSmall").prepend(sideBox);
        console.log("Add side box success!");
    }
}

var updateBoxesBig = function (groupId, sideBox) {
    var exists = false;
    console.log("Updating small");
    $("#msgHubBoxesSmall").children('.msgSideBox').each(function () {
        console.log($(this));
        if ($(this).data('group') == groupId) {
            console.log("Side Box exists.");
            $(this).prependTo($("#msgHubBoxesBig"));
            exists = true;
            // break the each statement
            return false;
        }
    });
    // if sideBox doesn't exist add in new
    if (!exists) {
        console.log("Side Box doesn't exist.");
        $(".msgHubPlaceHolder").hide();
        $("#msgHubBoxesBig").prepend(sideBox);
        console.log("Add side box success!");
    }
}


var updateHub = function (groupId, sideBox) {
    console.log("Updating hub");
    updateBoxesBig(groupId, sideBox);
    updateBoxesSmall(groupId, sideBox);
}

/*message handling*/

"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable send button until connection is established
document.getElementById("btnSendMsg").disabled = true;

connection.on("ReceiveMessage", function (user, message, groupId, sideBox) {
    // $("#msgHistory").find("#msgHistoryPlaceHolder").hide();
    var metaTag = $("#msgMeta");
    var thisGroupId = metaTag.data("group");
    if (groupId == thisGroupId) {
        var isEncrypted = metaTag.data("encrypted");
        // convert cipherText to plaintext
        if(isEncrypted) message = decryptMessageIn(message);
        addMessageIn(message);
    }
    updateHub(groupId, sideBox);
    setScroll();
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
});

connection.on("SetCrypto", function (keyPath, keyShare) {
    console.log("Setting crypto");
    // save remote data in session storage
    sessionStorage.setItem("keypath", keyPath);
    sessionStorage.setItem("remoteshare", keyShare);
});


connection.start().then(function (share) {
    document.getElementById("btnSendMsg").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("btnSendMsg").addEventListener("click", function (event) {
    // $("#msgHistory").find("#msgHistoryPlaceHolder").hide();
    var message = document.getElementById("msgInput").value;
    var metaTag = $("#msgMeta");
    var isEncrypted = metaTag.data("encrypted");
    var messageReciever = "";
    var messageSender = "";
    if (isEncrypted) {
        // convert plaintext message to ciphertext for receiver
        messageReciever = encryptMessageOut(message);
        // convert plaintext message to cipherText for sender
        messageSender = encryptMessageOutSender(message);
    }
    addMessageOut(message);
    setScroll();
    var receiver = $(".msgTitleText").data("username");
    // get group id from meta div
    var metaTag = $("#msgMeta");
    var groupId = metaTag.data("group");
    if (receiver != "") {
        connection.invoke("SendMessageToGroup", receiver, message, messageSender, messageReciever, groupId).catch(function (err) {
            return console.error(err.toString());
        });
    }
    // clear text content of message creation box
    $("#msgInput").val("");
    event.preventDefault();
});



// encrypt message for receiver
var encryptMessageOut = function (msg) {
    var metaTag = $("#msgMeta");
    // recipient's public key
    var receiverPubKey = metaTag.data("recieverkey");
    var cipherText = encryptMessageWithPub(receiverPubKey, msg);
    return cipherText;
}

// encrypt message for sender
var encryptMessageOutSender = function (msg) {
    // get remote share
    var shareRemote = sessionStorage.getItem("keypath");
    // get local share
    var shareLocal = localStorage.getItem("seedShare");
    // combine shares into original seed (in hex)
    var seedHex = combineShares(shareLocal, shareRemote);
    // encrypt message with seed
    var cipherText = encryptMessageWithSeed(seedHex, msg);
    return cipherText;
}

// decrypt incoming message by combining local and remote seed shares
var decryptMessageIn = function(cipherText){
    // get remote share
    var shareRemote = sessionStorage.getItem("keypath");
    // get local share
    var shareLocal = localStorage.getItem("seedShare");
    // combine shares into original seed (in hex)
    var seedHex = combineShares(shareLocal, shareRemote);
    var plainText = decryptCipher(seedHex, cipherText);
    // return plaintext
    return plainText;
}


// decrypt message history formatted on server
var decryptGroupInit = function (msgHistory) {
    console.log(msgHistory);
    $(msgHistory).each(function () {
        console.log($(this));
    });
}