$(".msgSideClose").on('click', function () {
    $("#slideOut").sideNav('hide');
});

/*show contact search box on request*/
$(".msgNew").on('click', function () {
    $("#slideOut").sideNav('hide');
    $("#msgCreateArea").hide('slow');
    $("#msgHistoryArea").hide('slow');
    $("#msgBlankArea").hide('slow');
    $("#msgSearchArea").show('fast');
});