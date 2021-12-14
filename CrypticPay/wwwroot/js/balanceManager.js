var showBal = function (res) {
    var result = res.responseText;
    $("#balContainer").empty();
    $("#balContainer").append(result);
    DisplayHideButton();
    console.log("Balances updated!");
};