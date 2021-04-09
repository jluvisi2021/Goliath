"use strict";

// Update text.
$(window).ready(() => {
    const browserSupportedElement = $("#browser-supported");

    if (GlobalScript.checkBrowser(false)) {
        browserSupportedElement.html("Yes");
        browserSupportedElement.css({
            "color": "green"
        });
    } else {
        browserSupportedElement.html("No");
        browserSupportedElement.css({
            "color": "red"
        });
    }

    $("#collapse").click(() => {
        ErrorScript.updateArrow();
    });
    $("#report-error-btn").click(() => {
        ErrorScript.reportError();
    });


});
// Enable tool tips
$(document).ready(() => {
    $('[data-toggle="tooltip"]').tooltip({
        container: 'body'
    });
});

/**
 * Represents methods and variables that belong to the
 * "Errors" controller.
 */
const ErrorScript = (() => {

    return {
        /**
         * Update the text in the DOM depending on
         * different values read.
         * @param {string} id
         * @param {boolean} val
         */
        updateText: (id, val) => {
            const elementID = $(id);
            if (val) {
                elementID.html("Yes").css({
                    "color": "green"
                });
            } else {
                elementID.html("No").css({
                    "color": "red"
                });
            }
        },
        /** 
         *  Update the "Show additional information" arrow.
         */
        updateArrow: () => {
            const arrowElement = $("#arrow");
            if (arrowElement.hasClass("fas fa-arrow-right")) {
                arrowElement.removeClass("fas fa-arrow-right")
                    .addClass("fas fa-arrow-down");
            } else {
                arrowElement.removeClass("fas fa-arrow-down")
                    .addClass("fas fa-arrow-right");
            }
        },
        /**
         * Shows a modal with infomation on how to report an error.
         * */
        reportError: () => {
            GlobalScript.displayModal("Report Error", 'To report an error please email: <a href="mailto: support@golaith.com">support@golaith.com</a> ');
        }
    }
})();
