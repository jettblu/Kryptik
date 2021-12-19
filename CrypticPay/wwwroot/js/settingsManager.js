$("#user").on("change",)

var updateUsername = function () {
    $("#profileForm").submit();
}

var completeUsernameUpdate = function(res) {
    if (!res.updated) return;
    // update local seedshare's name
    updateLocalSeedName(res.oldname, res.newname);
    // reload page
    location.reload();
}
