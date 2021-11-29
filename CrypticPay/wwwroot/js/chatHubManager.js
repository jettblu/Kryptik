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
    console.log(res);
    $("#msgHistoryArea").empty();
    var result = res.responseJSON;
    // UPDATE to be dynamic
    var isEncrypted = result.isEncrypted;
    console.log(result);
    console.log("Completing msg history");
    $("#msgHistoryArea").append(result.metaTag);
    // append messages formatted by server
    // decrypt each message if encrypted
    if (isEncrypted) decryptGroupInit(result.messages, isEncrypted);
    // decrypt each message
    /*$("#msgHistoryArea").append(result);*/
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

connection.on("ReceiveMessage", async function (user, message, groupId, sideBox) {
    // $("#msgHistory").find("#msgHistoryPlaceHolder").hide();
    var metaTag = $("#msgMeta");
    var thisGroupId = metaTag.data("group");
    if (groupId == thisGroupId) {
        var isEncrypted = metaTag.data("encrypted");
        // convert cipherText to plaintext
        if(isEncrypted) message = await decryptMessageIn(message);
        addMessageIn(message);
    }
    updateHub(groupId, sideBox);
    setScroll();
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
});

// client handler to set user crypto
connection.on("SetCrypto", function (keyPath, keyShare) {
    console.log("Setting crypto");
    // save remote data in session storage
    sessionStorage.setItem("keypath", keyPath);
    sessionStorage.setItem("remoteshare", keyShare);
});

// client handler once conection with signalr is made
connection.start().then(function (share) {
    document.getElementById("btnSendMsg").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("btnSendMsg").addEventListener("click", async function (event) {
    // $("#msgHistory").find("#msgHistoryPlaceHolder").hide();
    var message = document.getElementById("msgInput").value;
    var metaTag = $("#msgMeta");
    console.log(metaTag);
    var isEncrypted = metaTag.data("isencrypted");
    console.log(isEncrypted);
    var messageReciever = "";
    var messageSender = "";
    if (isEncrypted) {
        console.log("Encrypting messages");
        // convert plaintext message to ciphertext for receiver
        messageReciever = await encryptMessageOut(message);
        // convert plaintext message to cipherText for sender
        messageSender = await encryptMessageOutSender(message);
    }
    console.log("msg. sender:");
    console.log(messageSender);
    var txt = await decryptMessageIn(messageSender);
    console.log("Decrypted sender:");
    console.log(txt);
    console.log("msg. reciever:");
    console.log(messageReciever);
    addMessageOut(message);
    setScroll();
    var receiver = $(".msgTitleText").data("username");
    // get group id from meta div
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
    console.log("Encrypting: ");
    console.log(msg);
    var metaTag = $("#msgMeta");
    // recipient's public key
    var receiverXpubKey = metaTag.data("recipientkey");
    console.log("Recip. Pub Key:")
    console.log(receiverXpubKey);
    console.log("xpub passed to client:");
    var xpub = metaTag.data("xpub");
    console.log(xpub);
    console.log("Key generated on client:");
    var hdk = hdkey.fromExtendedKey(xpub);
    path = sessionStorage.getItem("keypath");
    console.log(hdk.derive(path).publicKey);
    var cipherText = encryptMessageWithPub(receiverXpubKey, msg);
    return cipherText;
}

// encrypt message for sender
var encryptMessageOutSender = function (msg) {
    // get remote share
    var shareRemote = sessionStorage.getItem("remoteshare");
    console.log("User Shares:");
    console.log(shareRemote);
    // get local share
    var shareLocal = localStorage.getItem("seedShare");
    console.log(shareLocal);
    // combine shares into original seed (in hex)
    var seedHex = combineShares(shareLocal, shareRemote);
    // encrypt message with seed
    var cipherText = encryptMessageWithSeed(seedHex, msg);
    return cipherText;
}

// decrypt incoming message by combining local and remote seed shares
var decryptMessageIn = async function(encrypted){
    // get remote share
    var shareRemote = sessionStorage.getItem("remoteshare");
    // get local share
    var shareLocal = localStorage.getItem("seedShare");
    // combine shares into original seed (in hex)
    var seedHex = combineShares(shareLocal, shareRemote);
    
    var plainText = await decryptCipher(seedHex, encrypted);
    console.log("Plain text");
    console.log(plainText);
    console.log(plainText.toString());
    // return plaintext
    return plainText.toString();
}


// decrypt message history formatted on server
/*var decryptGroupInit = function (msgHistory) {
    console.log("Decrypting initial msg. batch.......");
    console.log(msgHistory);
    $(msgHistory).each(function () {
        console.log($(this));
        if ($(this).hasClass("msgEncrypted")) {
            console.log("Is text: ");
            console.log($(this));
            console.log("Inner: ");
            console.log($(this).find(".msgText"));
            var encryptedMsg = JSON.parse($(this).data("encrypted"));
            console.log(encryptedMsg);
            // decrypt message cipher text
            $(this).text = decryptMessageIn(encryptedMsg);
        }
    });
}*/

var decryptGroupInit = async function (msgList, isEncrypted) {
    console.log("message list:");
    console.log(msgList);
    console.log(msgList.length);
    for (var i = 0; i < msgList.length; i++) {
        console.log("------MsgList Next------");
        console.log(msgList[i]);
        console.log("Msg. Encrypted:");
        var msg = msgList[i].msgEncrypted;
        console.log(msg.ciphertext);
        console.log(msg.ciphertext.data);
        msg.ciphertext = buffer.Buffer.from(msg.ciphertext.data);
        msg.ephemPublicKey = buffer.Buffer.from(msg.ephemPublicKey.data);
        msg.iv = buffer.Buffer.from(msg.iv.data);
        msg.mac = buffer.Buffer.from(msg.mac.data);
        console.log(msg);
        var text;
        // decrypt if message is encrypted
        if (isEncrypted) text = await decryptMessageIn(msg);
        else text = msg;
        // display message
        if (msgList[i].isIn) {
            addMessageIn(text);
            console.log("is in");
        }
        else {
            addMessageOut(text);
            console.log("is out");
        }
    } 
}