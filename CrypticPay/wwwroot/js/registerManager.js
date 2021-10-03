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

    if (e.shiftKey) {
        return false;
    };

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

function codeChanged(e) {
    var codeLength = $("#codeInput");
    if (codeLength >= 6) {
        $("#btnNext").prop("disabled", false);
    }
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
    $("#confirmPhone").show();
    $("#phoneCode").hide();
};

var showPass = function () {
    $("#pwordStep").show();
};

// hide all sections
var hideSteps = function () {
    $(".step").hide("slow");
};

var sendTrigger = function () {
    $("#phoneNumberSend").val($("#phoneNumber").val());
    $("#phoneCountrySend").val($("#country").val());
    $("#sendForm").submit();
}

var confirmFlow = function () {
    $("#confirmPhone").hide();
    $("#phoneCode").val("");
    $("#phoneCode").show("fast");
    $("#btnNext").prop("disabled", true);
    $("#btnNext").delay(500).show("slow");
    $("#sendForm").submit();
};

var resetFlow = function () {
    hideSteps();
    $("#introContainer").show(2200);
    $("#stepContainer").hide("slow");
    $("#progressContainer").hide("slow");
    $(".btnStep").hide();
    $("#btnNext").data("step", "0");
}

var hideCode = function () {
    if ($("#phoneCode").is(":visible")) {
        $("#confirmPhone").show("fast");
        $("#phoneCode").hide("fast");
        $("#btnNext").hide("fast");
        $("#number").val("");
        $("#phoneCode").val("");
    }
}

$("#number").on('change', hideCode);

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
            $("#btnNext").show();
            setValue(25);
        }

        else if (step == "3") {
            showNumber();
            $("#btnSubmit").hide();
            $("#btnNext").data("step", "2");
            $("#btnNext").prop("disabled", true);
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
            $("#btnNext").prop("disabled", true);
            $("#btnNext").hide();
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

$('#resetPhone').on('click', hideCode)

$("#btnSubmit").click(function () {
    $("#registerForm").submit();
});

$("#confirmPhone").on('click', confirmFlow)


var handleSend = function(res) {
    console.log("Async update initiated.")
    console.log(res);
    var result = res.responseJSON;
    console.log(result);
};
