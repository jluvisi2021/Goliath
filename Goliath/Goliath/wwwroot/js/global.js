/**
 * Closure Scope
 *
 * */

/** Access data from global.js */
const GLOBAL = (function () {

/** Private Variables */

    const _bannerTypes = Object.freeze({
        "alert-primary": 0,
        "alert-secondary": 1,
        "alert-success": 2,
        "alert-danger": 3,
        "alert-dark": 4
    });

    return {
        /** Returns the list of avaliable banner types. */
        BannerTypes: function () {
            return _bannerTypes;
        }
    };
})();

/**
 * Tests if jQuery can be found and loaded at run time.
 * */
function testjQuery() {
    $(document).ready(function () {
        console.log("jQuery has been found and enabled.");
    });
}

/**
 * Automatically displays a notification alert at a parent div.
 * This function makes a new child banner immediatly below the parent div
 *
 * Types:
 * 1- Primary (Dark Blue)
 * 2- Secondary (Blue)
 * 3- Success (Green)
 * 4- Error (Red)
 * 5- Primary Dark (Dark)
 *
 * NOTE:
 * - Requires that the type be in range
 *
 * @param {string} textHeader (BOLD TEXT)
 * @param {string} text
 * @param {GLOBAL.BannerTypes} type
 * @param {string} divParentID
 * @param {string} id (Optional)
 * 
 */
function displayNotification(textHeader, text, type, divParentID) {
    let typeStr = "";
    switch (type) {
        case 0:
            typeStr = "alert-primary";
            break;
        case 1:
            typeStr = "alert-secondary";
            break;
        case 2:
            typeStr = "alert-success";
            break;
        case 3:
            typeStr = "alert-danger";
            break;
        case 4:
            typeStr = "alert-dark";
            break;
        default:
            typeStr = "alert-primary";
            text = "[CRITICAL] Error. Could not find type: " + type;
            console.error("Could not find notification type: " + type);
            break;
    }

    $("#" + divParentID).prepend(
        '<div class="alert ' + typeStr + ' alert-dismissible fade show" role="alert">'
        + '<button type="button" class="close" data-dismiss="alert" aria-label="Close">'
        + '<span aria-hidden="true">×</span>'
        + '</button>'
        + '<strong>' + textHeader + '</strong> ' + text
        + '</div>'
    );
}

/**
 * Prevent flickering when the page loads.
 * All CSS elements are hidden by default and when
 * everything is ready we can use this function to load all
 * elements into view.
 * @param {number} timeout Amount of time to wait after function is called.
 */
function load(timeout = 0) {
    setTimeout(
        function () {
            $(document).ready(function () {
                $('#RenderBody').css("visibility", "visible");
               
            });
        }, timeout);
}

/**
 * Returns the percentile difference between the users machine and
 * the developers machine.
 * This method is used for the view port method which scales
 * content based on the users screen.
 * @param {number} uwh
 * @param {number} uww
 * @param {number} cwh
 * @param {number} cww
 * @returns {number} 
 */
function calculateDifference(uwh, uww, cwh, cww) {
    const whdiff = uwh - cwh;
    const wwdiff = uww - cww;
    const calcA = whdiff / uwh;
    const calcB = wwdiff / cwh;
    return calcA < calcB ? (1 - calcA) : (1 - calcB);
}
/**
 * < jQuery Function >
 * Adjusts the current view port of the display to match the developers display.
 * Optional param of div.
 * If no param is passed div = "ZoomDiv"
 * @param {string} div [Optional]
 * */
function adjustViewport(div = "") {
    if (div === "") {
        div = "ZoomDiv";
    }
    const usualWindowHeight = 969;
    const usualWindowWidth = 1920;
    const currWindowHeight = $(window).height();
    const currWindowWidth = $(window).width();
    
    $("#" + div).css({
        "zoom": String(calculateDifference(usualWindowHeight, usualWindowWidth, currWindowHeight, currWindowWidth))
        
    });
    
    //$("#" + div).append('<style type="text/css">@media (pointer:none), (pointer:coarse) { zoom: 0.2; }</style>');
    
        console.log("Your calculated zoom: " + String(calculateDifference(usualWindowHeight, usualWindowWidth, currWindowHeight, currWindowWidth)));
    
    
}
