$("#searchForm").keyup(function () {
    console.log("search sent!");
    $("#clearIcon").show();
    $("#searchForm").submit();
});

$("#clearIcon").on('click', function () {
    $("#to").val("");
    $("#searchFriendContainer").empty();
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

$("#create-transaction-button").on('click', function () {
    $("#transactionForm").submit();
    // empty search container if still full
    $("##searchFriendContainer").empty();
    // empty send and status container
    $("#sendContainer").empty();
    $("#statusContainer").empty();
    var basePath = window.location.origin;
    $("#statusContainer").append(`<center><p>Creating transaction. This may take a moment.</p> <img src="${basePath}/Media/rocket.gif" class="animation-small"/></center>`);
});