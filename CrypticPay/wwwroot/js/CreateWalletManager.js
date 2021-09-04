walletComplete = function (res) {
    var result = res.responseText;
    $("#walletContainer").empty();
    $("#walletContainer").append(result);
    $("#btnCreateWallet").remove();
}

$("#btnCreateWallet").on("click", function () {
    var basePath = window.location.origin;
    $("#walletContainer").append(`<center><p>Creating your secure Krypik wallet. This may take a moment.</p> <img src="${basePath}/Media/rhombus.gif" class="animation-small"/></center>`);
    $(this).submit();
    $(this).hide();
});