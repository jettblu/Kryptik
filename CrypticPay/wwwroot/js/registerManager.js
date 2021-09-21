
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



const input = document.getElementById('value')
const progressValue = document.querySelector('.Progressbar__value');
const progress = document.querySelector('progress');

function setValue(value) {
    progressValue.style.width = `${value}%`;
    progress.value = value;
}


var showName = function () {
    $("#nameStep").show();
}

var showNumber = function () {
    $("#numberStep").show();
};

var showPass = function () {
    $("#pwordStep").show();
};

// hide all sections
var hideSteps = function () {
    $(".step").hide("slow");
};



var resetFlow = function () {
    hideSteps();
    $("#introContainer").show(2200);
    $("#stepContainer").hide("slow");
    $("#progressContainer").hide("slow");
    $(".btnStep").hide();
    $("#btnNext").data("step", "0");
}

var flow = function () {
    console.log("Flow initiated");
    var step = $("#btnNext").data("step");
    var stepContainer = $("#stepContainer");
    var progressContainer = $("#progressContainer");
    if (step != "0") {
        hideSteps();
    }

    if ($(this).attr("id") == "btnBack") {
        if (step == "1") {
            resetFlow();
        }

        else if (step == "2") {
            showName();
            $("#btnNext").data("step", "1");
            setValue(25);
        }

        else if (step == "3") {
            showNumber();
            $("#btnSubmit").hide();
            $("#btnNext").data("step", "2");
            $("#btnNext").show();
            setValue(50);
        };
    }
    else {
        if (step == "0") {
            // case where register button is clicked
            $("#btnSubmit").hide();
            $("#introContainer").hide("fast");
            stepContainer.show("fast");
            showName();
            progressContainer.show();
            $(".btnStep").show();
            $("#btnNext").data("step", "1");
            setValue(25);
        }

        else if (step == "1") {
            showNumber();
            $("#btnNext").data("step", "2");
            setValue(50);
        }

        else if (step == "2") {
            showPass();
            $("#btnNext").data("step", "3");
            $("#btnNext").hide();
            $("#btnSubmit").show();
            setValue(75);
        }

        else {
            // add logic for form submission
            hideSteps();
        };
    }


};

$('.btnStep').on('click', flow);

$('#btnRegister').on('click', flow);

$("#btnSubmit").click(function () {
    $("#registerForm").submit();
});
