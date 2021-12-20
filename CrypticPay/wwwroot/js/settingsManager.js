$("#user").on("change",)

var updateUsername = function () {
    $("#profileForm").submit();
}

var completeBasicUpdate = function (res) {
    // update local seedshare's name
    if (res.updateduname) {
        updateLocalSeedName(res.oldname, res.newname);
    }
    // reload page
    if (res.refresh) {
        location.reload();
    }
}