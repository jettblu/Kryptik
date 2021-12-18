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

