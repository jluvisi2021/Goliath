/**
 * Closure Scope
 *
 * */

/** Access data from global.js */
const GlobalScript = (function () {
    /** Private Variables */

    let _browserZoom = window.devicePixelRatio;
    const _defaultBrowserZoom = _browserZoom;

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
        },
        getBrowserZoom: function () {
            return _browserZoom;
        },
        updateBrowserZoom: function () {
            _browserZoom = window.devicePixelRatio;
        },
        getDefaultBrowserZoom: function () {
            return _defaultBrowserZoom;
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
 * @param {GlobalScript.BannerTypes} type
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
 * Displays a modal to the user.
 * The modal only has a close option and its buttons
 * are set to the primary color.
 * The modal is appended to the #RenderBody.
 * 
 * @param {string} title [Supports HTML(Use ``)]
 * @param {string} body [Supports HTML(Use ``)]
 */
function displayModal(title, body) {



        $("#RenderBody").append(`
        <div class="modal fade" id="render-model" tabindex="-1" role="dialog" aria-labelledby="exampleModalCenterTitle" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLongTitle">` + title + `</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        ` + body + `
                </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                    </div>
                </div>
            </div>
        </div >
        `
        );
        $('#render-model').modal('show');

    
}

/**
 * Checks if the browser is supported.
 * If not then it sends a notification to the user.
 * */
function checkBrowser() {
    if (navigator.userAgent.search("Chrome") >= 0) {
        return;
    }
    else if (navigator.userAgent.search("Safari") >= 0) {
        return;
    }
    displayNotification("Warning", "Goliath does not support this browser. Goliath may continue to function but not all features may work.", GlobalScript.BannerTypes()["alert-danger"], "primary-content");
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
 * This function can be called whenever and it will automatically scale the content of
 * the display to the correct margins.
 * The function ignores the user zooming in.
 * */
function adjustViewport() {
    const usualWindowHeight = 969;
    const usualWindowWidth = 1920;
    const currWindowHeight = $(window).height();
    const currWindowWidth = $(window).width();

    // Ignore the resize if the user is manually manipulating the browser zoom.
    GlobalScript.updateBrowserZoom();
    if (GlobalScript.getBrowserZoom() != GlobalScript.getDefaultBrowserZoom()) {
        return;
    }
    // Adjust the backend zoom of the page.
    $("html").css({
        "zoom": String(calculateDifference(usualWindowHeight, usualWindowWidth, currWindowHeight, currWindowWidth))
    });

}
