$("#searchForm").keyup(function () {
    console.log("search sent!");
    $("#clearIcon").show();
    $("#searchForm").submit();
});

$("#clearIcon").on('click', function () {
    $("#to").val("");
    $(this).hide();
});

$("#searchFriendContainer").on('click', ".userToSelect", function () {
    var userName = $(this).data('user');
    $("#to").val(userName);
    $("#searchFriendContainer").empty();
});


ShowSearchFriendsResult = function (res) {
    console.log(res);
    var result = res.responseText;
    console.log(result);
    $("#searchFriendContainer").empty();
    $("#searchFriendContainer").append(result);
}

// update main form to include changed values

$("#autocomplete-input").on('change', function () {
    $("#mainCoin").val($("#autocomplet-input").val());
});

$("#to").on('change', function () {
    $("#mainTo").val($("#to").val());
});

$("#for").on('change', function () {
    $("#mainFor").val($("#for").val());
});