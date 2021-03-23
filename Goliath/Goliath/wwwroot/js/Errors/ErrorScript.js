// Update text.
$(window).ready(() => {
    if (GlobalScript.checkBrowser(false)) {
        $("#browser-supported").html("Yes");
        $("#browser-supported").css({
            "color": "green"
        });
    } else {
        $("#browser-supported").html("No");
        $("#browser-supported").css({
            "color": "red"
        });
    }
});
// Enable tool tips
$(document).ready(() => {
    $('[data-toggle="tooltip"]').tooltip({
        container: 'body'
    });
});

/**
 * Update the text for elements.
 * @param {string} id
 * @param {boolean} val
 */
function updateText(id, val) {
    if (val) {
        $(id).html("Yes");
        $(id).css({
            "color": "green"
        });
    } else {
        $(id).html("No");
        $(id).css({
            "color": "red"
        });
    }
}

/*
 * Update the arrow
 */
function update() {
    if ($("#arrow").hasClass("fas fa-arrow-right")) {
        $("#arrow").removeClass("fas fa-arrow-right");
        $("#arrow").addClass("fas fa-arrow-down");
    } else {
        $("#arrow").removeClass("fas fa-arrow-down");
        $("#arrow").addClass("fas fa-arrow-right");
    }
}

/**
 * show the report error modal.
 * */
function reportError() {
    GlobalScript.displayModal("Report Error", 'To report an error please email: <a href="mailto: support@golaith.com">support@golaith.com</a> ');
}