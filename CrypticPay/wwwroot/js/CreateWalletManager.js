walletComplete = function (res) {
    var result = res.responseText;
    $("#walletContainer").empty();
    $("#walletContainer").append(result);
    $("#btnCreateWallet").remove();
}

$("#btnCreateWallet").on("click", function () {
    var basePath = window.location.origin;
    $("#walletContainer").append(`<br> <center><div class="row"><div class="col l8 s10"><p style="font-weight: 500px">Creating your secure Krypik wallet. This may take a moment.</p></div></div><img src="${basePath}/Media/rhombus.gif" class="animation-small"/></center>`);
    $(this).submit();
    $(this).hide();
});