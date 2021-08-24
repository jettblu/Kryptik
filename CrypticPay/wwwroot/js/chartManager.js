






$(".time").on('click', function (event) {
    var timeBlock = $(this);
    $(".time").css("background-color", "white");
    $("#lookback").val(timeBlock.text())
    timeBlock.css("background-color", "#42bff5");
    console.log($("#lookback").val());
    // only update the form on post
/*    $.ajax({
        url: "/Payments/Assets/UpdateChart'",
    contentType: "application/json",
    dataType: "json",
    success: function (response) {
            console.log("Chart refreshed!");
            console.log(response);
            var ctx = $("#myChart");
            var chart = new Chart(ctx, res);
    },
    failure: function (response) {
        console.log("Chart Update Failed");
    }
});*/
    $("#chartForm").submit();
    event.stopPropagation();
    event.stopImmediatePropagation();
});










