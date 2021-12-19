var showBal = function (res) { 
    console.log(res);
    var result = res.responseText;
    $("#balContainer").empty();
    $("#balContainer").append(result);
    DisplayHideButton();
    console.log("Balances updated!");
};

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

$("#balContainer").on("click", "#btnBalHide", function () {
    $("#balContainer").hide();
    $("#btnBal").show();
    $("#btnBal").data("shown", true);
});


function DisplayHideButton() {
    $("#balContainer").append(`<div class="col s12"><center><button class="btn btn-search" style="margin-top:18px;" id="btnBalHide">Hide Accounts</button></center></div>`);
};