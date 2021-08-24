const base = "$0";



$('#amount').keydown(function (e) {
    if (e.shiftKey) {
        return false;
    };

    var currAmount = $('#amount').val();
    var charval = String.fromCharCode(e.keyCode);
    console.log(e.keyCode);
    console.log(charval);

    if (currAmount.length == 2 && currAmount != base && e.keyCode == 8) {
        e.preventDefault();
        $("#amount").val(base);
        return false;
    }

    if (currAmount.length <= 2 && e.keyCode == 8) {
        $("#amount").effect("shake", { times: 3 }, 1000);
        return false;
    }

    if (currAmount.slice(-1) == "." && e.keyCode == 190) {
        $("#amount").effect("shake", { times: 3 }, 1000);
        return false;
    }

    if (currAmount.charAt(1) == "0" && currAmount.length == 2) {
        if (e.keyCode == 190) {
            $("#amount").val("$0.");
        }
        else {
            $("#amount").val("$" + charval);
        }

        return false;
    }

    var verified = (e.keyCode == 190 || e.keyCode == 37 || e.keyCode == 39 || e.which == 8 || e.which == undefined || e.which == 0) ? null : String.fromCharCode(e.which).match(/[^0-9]/);
    if (verified) { e.preventDefault(); }
});


$('#amount').keyup(function () {
    console.log("key UP");
    FormatAmount();
});

function FormatAmount() {
    var amount = $('#amount');
    var amountString;

    if (amount.val() == base) {
        return;
    }

    console.log(amount.val().slice(-1));




    console.log("format started");

    amountString = amount.val();

    if (amountString.endsWith('.')) {
        return;
    }

    amountString = addCommas(amountString.substring(1));

    console.log(amountString);

    amountString = "$" + amountString;
    amount.val(amountString);

    return amountString;
};




function removeCharacter(str, charPos) {
    part1 = str.substring(0, charPos);
    part2 = str.substring(charPos + 1, str.length);
    return (part1 + part2);
};

function addCommas(nStr) {
    nStr = nStr.replace(/,/g, '');
    console.log(nStr);
    return Number(nStr).toLocaleString("en");
};