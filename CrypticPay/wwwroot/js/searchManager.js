
$('#clearIcon').on('click', clearSearch)




function clearSearch($event) {
    var search = $('#search');
    search.val("");
}

$(document).ready(function () {
    var search = $('#search');
    search.focus();
    var val = search.val(); //store the value of the element
    search.val(""); //clear the value of the element
    search.val(val); //set that value back.
});

