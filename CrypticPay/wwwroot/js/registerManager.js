
function showPassword() {
    var pwordDisplay = document.getElementById("pword");
    var pwordConfirmDisplay = document.getElementById("pwordConfirm");
    if (pwordDisplay.type === "password") {
        pwordDisplay.type = "text";
        pwordConfirmDisplay.type = "text";
    } else {
        pwordDisplay.type = "password";
        pwordConfirmDisplay.type = "password";
    }
}



function keyispressed(e) {
    var charval = String.fromCharCode(e.keyCode);
    // allow backspace and decimal
    if (e.keyCode == 8 || e.keyCode == 110) {
        console.log(charval);
        return true;
    }
    // don't allow any other characters
    if (isNaN(charval)) {
        return false;
    }
    return true;
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
