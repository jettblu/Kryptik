
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
    $(".step").hide();
};

var resetFlow = function () {
    hideSteps();
    $("#btnNext").data("step", "0");
}

var showFile = function () {
    $("#fileStep").show("fast");
    $("#fileStepInfo").show("fast");
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

var showMeta = function () {
    $("#metaStep").show("fast");
}

$("#number").on('change', hideCode);

var flow = function () {
    console.log("Flow initiated");
    var step = $("#btnNext").data("step");
    var stepContainer = $("#stepContainer");
    var progressContainer = $("#progressContainer");
    var btnContainer = $("#btnContainer");
    if (step != 0) {
        hideSteps();
    }


    if ($(this).attr("id") == "btnBack") {
        if (step == "1") {
            showFile();
        }
        else if (step == "2") {
            showFile();
            $("#btnSubmit").hide();
            $("#btnBack").hide();
            $("#btnNext").data("step", "1");
            $("#btnNext").show();
            setValue(50);
        }
    }
    // case where next button is clicked
    else {
        if (step == "0") {
            showFile();
            $("#btnSubmit").hide();
            $("#btnBack").hide();
            progressContainer.show();
            btnContainer.show();
            $("#btnNext").data("step", "1");
            setValue(50);
        }

        else if (step == "1") {
            showMeta();
            $("#btnBack").show();
            $("#btnNext").hide();
            $("#btnSubmit").show();
            $("#btnNext").data("step", "2");
            setValue(75);
        }

        else {
            uploadTrigger();
        };
    }


};

var showPending = function () {
    $("#pendingContainer").show();
    $("#pendingTitle").show();
}

var hidePending = function () {
    $("#pendingContainer").hide();
    $("#pendingTitle").hide();
}

var uploadTrigger = function () {
    $("#uploadForm").submit();
    $("#progressContainer").hide();
    resetFlow();
    $("#btnContainer").hide();
    showPending();
}


$('#btnSubmit').on('click', uploadTrigger);

$('.btnStep').on('click', flow);

$('#fileInput').on('change', flow);



var handleSend = function (res) {
    var result = res.responseJSON;
    if (result == false) {
        hideCode();
    }
};
