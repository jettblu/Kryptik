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
    $("#mainTo").val(userName);
    $("#to").val(userName);
    console.log(userName);
    $("#searchFriendContainer").hide("slow");
    $("#searchFriendContainer").empty();
    // indicate user has been chosen
    $("#to").data('selected', true);
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
    $("#mainCoin").val($("#autocomplete-input").val());
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
    $("#searchFriendContainer").empty();
    // empty send and status container
    $("#sendContainer").hide("slow");
    $("#statusContainer").empty();
    var basePath = window.location.origin;
    $("#statusContainer").append(`<center><p>Creating transaction. This may take a moment.</p> <img src="${basePath}/Media/rocket.gif" class="animation-small"/></center>`);
});

// clear friend options if user clicks on another screen segment
$(document).click(function () {
    if ($("#to").data('selected') == false) {
        $("#to").val("");
        $("#searchFriendContainer").empty();
    }
});

$(".to").click(function (event) {
    event.stopPropagation();
});

$(".menuWraper").click(function (event) {
    alert('clicked inside');
    event.stopPropagation();
});

