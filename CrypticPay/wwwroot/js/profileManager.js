$("#friendRequestHolder").on('click', ".accept", function (event) {
    console.log("accept");
    var name = $(".accept").attr('name');
    console.log(name);
    $("#friendAccepted").val(true);
    $("#friendName").val(name);
    $("#formFriend").submit();
    event.stopPropagation();
    event.stopImmediatePropagation();
});

$("#friendRequestHolder").on('click', ".reject", function (event) {
    console.log("reject");
    var name = $(".reject").attr('name');
    $("#friendAccepted").val(false);
    $("#friendName").val(name);
    $("#formFriend").submit();
    event.stopPropagation();
    event.stopImmediatePropagation();
});

function UpdateFriendCount(count){
    if (count == 1) {
        friendCountString = `${count} friend`;
    }
    else {
        friendCountString = `${count} friends`;
    }
    $("#friendCount").empty();
    $("#friendCount").append(friendCountString);
}

$("#btnBal").on("click", function () {
    $(this).hide();
    
    if ($(this).data("shown") == false) {
        $("#balContainer").empty();
        var basePath = window.location.origin;
        $("#balContainer").append(`<center><p>Loading wallets. This may take a moment.</p> <img src="${basePath}/Media/rhombus.gif" class="animation-small"/></center>`);
        $(this).submit();
    }
    else {
        $("#balContainer").show();
    }
    
});

$("#balContainer").on("click", "#btnBalHide",function () {
    $("#balContainer").hide();
    $("#btnBal").show();
    $("#btnBal").data("shown", true);
});


function DisplayHideButton() {
    $("#balContainer").append(`<div class="col s12"><center><button class="btn btn-search" style="margin-top:18px;" id="btnBalHide">Hide Accounts</button></center></div>`);
};