$('#authSwitch').on('change', switchHandler);

$(document).ready(function () {


    // ensure switch reflects current model state
    if ($('#authHiddenValue').val() == "False") {
        cSwitch = false;
    }
    else {
        cSwitch = true;
    }


    $('#authSwitch').prop('checked', cSwitch);

    
    console.log(cSwitch);
    toggleSwitchColor(cSwitch);

});




function toggleSwitchColor(cSwitchValue) {

    if (cSwitchValue != true) {
        console.log("Transform");
        $("#authOnLabel").css('font-weight', 'bold')
        $("#authOnLabel").css('color', '#03befc')
        $("#authOffLabel").css('color', '#9e9e9e')
        $("#authOffLabel").css('font-weight', 'normal')
    }

    else {
        $("#authOffLabel").css('font-weight', 'bold')
        $("#authOffLabel").css('color', '#03befc')
        $("#authOnLabel").css('color', '#9e9e9e')
        $("#authOnLabel").css('font-weight', 'normal')
    }
}

function switchHandler($event) {

    cSwitch = $('#authSwitch').prop('checked');

    // set auth pref., so form can be submitted and changes can be commited to DB
    $('#authHiddenValue').val(cSwitch);
    // submit form
    $('#pwordForm').submit();

    toggleSwitchColor(cSwitch);

    $event.stopPropagation();

}


$("#number").keyup(function () {

    var number = $("#number").val();
    var countryCode = $("#country").val();
    console.log(number);
    if (number.length < 13 & number.length != 0 & countryCode == "US") {
        var formatted = formatUSNumber(number);
        $("#number").val(formatted);
        console.log(formatted);
    }

});


function formatUSNumber(entry = '') {
    const match = entry
        .replace(/\D+/g, '').replace(/^1/, '')
        .match(/([^\d]*\d[^\d]*){1,10}$/)[0]
    const part1 = match.length > 2 ? `(${match.substring(0, 3)})` : match
    const part2 = match.length > 3 ? ` ${match.substring(3, 6)}` : ''
    const part3 = match.length > 6 ? `-${match.substring(6, 10)}` : ''
    return `${part1}${part2}${part3}`
}


