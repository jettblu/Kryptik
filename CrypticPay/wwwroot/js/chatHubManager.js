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
    console.log("search sent!");
    console.log($(this).val());
    $("#msgSearchFriendsForm").submit();
}));


complete = function (res) {
    console.log(res);
    console.log("That was raw response");
    var result = res.responseJSON;
    console.log(result);
    console.log(result.length);
    $("#placeHolder").hide();
    $("#searchResults").empty();
    if (result.length == 0) {
        console.log("Zero matching friends.");
        $("#placeHolder").show();
        console.log("Updated!");
    }
    else {
        console.log("Friends found.");
        $(result).each(function (index) {
            var currUser = result[index];
            console.log(currUser);
            var resultHtml = $(`
            <div class="row valign-wrapper card hoverable msgFriendResult" data-name="${currUser.name}">
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
    $("#msgCreateArea").hide();
    $("#msgHistoryArea").hide();
    $("#msgBlankArea").hide();
    $("#msgSearchArea").hide('slow');
    $("#searchResults").empty();
    console.log($(this).data("name"));
    $(".msgTitleText").text($(this).data("name"));
    $("#msgHistoryArea").delay(800).show('fast');
});