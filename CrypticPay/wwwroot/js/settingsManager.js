var completeBasicUpdate = function (res) {
    console.log(res);
    result = res.responseJSON;
    // update local seedshare's name
    if (result.updateduname) {
        updateLocalSeedName(res.olduname, res.newuname);
    }
    // reload page
    if (result.refresh) {
        location.reload();
    }
}

$(document).ready(function () {
    console.log("ready!");
    share = getLocalShare();
    console.log(share);
});

var basicUpdate = function () {
    console.log("Basic update initiated.");
    $("#profileForm").submit();
}


$("#profileForm").children().on("change", basicUpdate);