$("#searchForm").keyup(function () {
    console.log("search sent!");
    $("#searchForm").submit();
});


complete = function (res) {
    console.log(res);
    var result = res.responseJSON;
    console.log(result);
    console.log(result.length);
    $("#placeHolder").empty();
    $("#searchResults").empty();
    if (result.length == 0) {
        console.log("Zero matching friends.");
        var basePath = window.location.origin;
        var placeholderHtml = $(`<center>
            <span class="info-header">Find friends. Send crypto.</span>
            <img src="${basePath}/Media/search.gif" class="animation-small"/>
            </center>`);

        $("#placeHolder").append(placeholderHtml);
        console.log("Updated!");
    }
    else {
        console.log("Friends found.");
        $(result).each(function (index) {
            var currUser = result[index];
            console.log(currUser);
            var resultHtml = $(`<a href="Member?uname=${currUser.userName}">
            <div class="col s10 l6 offset-l3 offset-s1 card hoverable">

                <div class="col s1 l1">

                    <img src="${currUser.profilePhotoPath}" alt="user photo" class="circle profilePhotoSearch">

                </div>

                <div class="col s4 l4 offset-l1 offset-s2" style="padding-left: 0px; padding-top: 0px">
                    <span class="searchUserFullName">${currUser.name}</span>
                    <h6 class="searchUsertag">@${currUser.userName}</h6>
                </div>
            </div>
        </a>`);
            $("#searchResults").append(resultHtml);
        });

    }

};