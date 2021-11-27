walletComplete = function (res) {
    var result = res.responseText;
    $("#walletContainer").empty();
    $("#walletContainer").append(result);
    $("#btnCreateWallet").remove();
}

$("#btnCreateWallet").on("click", function () {
    var basePath = window.location.origin;
    $("#walletContainer").append(`<br> <center><div class="row"><div class="col l8 s10"><p style="font-weight: 500px">Creating your secure Krypik wallet. This may take a moment.</p></div></div><img src="${basePath}/Media/rhombus.gif" class="animation-small"/></center>`);
    // generate mnemeonic 

    // add ext. pub. key and shamir secret for secure storage on server
    var remoteData = createShares();
    console.log(remoteData);
    // update form values w/ client gen. values
    $("#extPubKey").val(remoteData.xpub);
    $("#remoteShare").val(remoteData.seedShareHex);
    // submit wallet creation form
    $(this).submit();
    $(this).hide();
});



