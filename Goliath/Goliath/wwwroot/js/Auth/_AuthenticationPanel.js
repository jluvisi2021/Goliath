/** RUNTIME PAGELOAD **/

/** Access data from authpanel.js */
const AuthPanel = (function () {
    /** Private Variables */
    let _badZoomAlertShown = false;

    return {
        /**
         * Get the current value for the zoom error notification.
         * */
        getBadZoomAlertShown: function () {
            return _badZoomAlertShown;
        },
        /**
         * Set the value for the zoom warning.
         * @param {boolean} val
         */
        setBadZoomAlertShown: function (val) {
            _badZoomAlertShown = val;
        },
        isGraphicEnabled: function () {
            return isGraphicEnabled();
        }
    };
})();

$(window).ready(function (e) {
    checkToggled();

    // Fixes a bug with the "Other" radio button where it would be clicked
    // and selected but the button would not be registered.
    $("#other-head").click(function (e) {
        if (e.target.id != "other-dropdown") {
            $("#other-dropdown").trigger("click");
            e.stopImmediatePropagation();
        }
    });
});
/* Page load end */


/**
 * Loads the colors of the radio button the user clicked on.
 * If the user clicked on a dropdown then it colors the dropdown elements.
 * @param {string} id
 */
function loadButtonElements(id) {
    const backgroundColor = "#0094ff";

    // Make the button colored
    if ($("#" + id + "-radio-btn").length === 0) {
        // The #other-radio-btn element does not change so we can leave it like this.
        $("#other-radio-btn").prop("checked", true);

        $("#" + id).css({
            "background-color": backgroundColor,
            "font-weight": "bold"
        });
    } else {
        $("#" + id + "-radio-btn").prop("checked", true);
    }
}

/**
 * A function to be run at runtime.
 * Toggles the graphic if it needs to be toggled.
 * */
function checkToggled() {
    if (!isGraphicEnabled()) {
        toggleGraphic(false);
    }
}

/**
 * Checks the browser local storage to see if the login graphic is enabled.
 * If the login graphic cannot be found then it is newly created.
 *
 */
function isGraphicEnabled() {
    // Key saved in local storage.
    const graphicEnabledKey = "graphic";

    if (localStorage.getItem(graphicEnabledKey) === null) {
        localStorage.setItem(graphicEnabledKey, "enabled");
        return true;
    }
    if (localStorage.getItem(graphicEnabledKey) === "disabled") {
        return false;
    }
    return true;
}

/**
 * Enables and disables the graphic.
 * Also allows to send the user a notification saying that there graphics have been changed.
 * @param {boolean} alert
 */
function toggleGraphic(alert) {
    
    if ($("#show-graphic-icon").hasClass("fas fa-eye-slash")) {
        localStorage.setItem("graphic", "enabled");
        $("#show-graphic-icon").attr("class", "fas fa-eye");
        $("#right-container").attr("class", "col-md-6 bg-light round-right");
        $("#right-container").attr("style", "");
        $("#left-container").removeClass("invisible");
        
    } else {
        localStorage.setItem("graphic", "disabled");
        $("#show-graphic-icon").attr("class", "fas fa-eye-slash");
        $("#left-container").addClass("invisible");
        $("#right-container").attr("class", "container bg-light round parent-container");
        $("#right-container").attr("style", "min-height: 100%; margin-top: 3%");
    }
    
    if (alert) {
        displayNotification("You changed the view.", "Your data is saved; You can undo this anytime.", GlobalScript.BannerTypes()["alert-primary"], "center-banner");
        $("#show-graphic").addClass("active");
    }
}

/**
 * Checks if the user attempted to zoom into the login page.
 * If they did then send them a notification only once.
 * */
function checkForPushBadResize() {
    GlobalScript.updateBrowserZoom();
    if (GlobalScript.getBrowserZoom() != GlobalScript.getDefaultBrowserZoom()) {
        if (AuthPanel.getBadZoomAlertShown() == false) {
            displayNotification("Warning", "Browser zooming is not supported. We recommend using the default zoom unless visual errors have occured.", GlobalScript.BannerTypes()["alert-danger"], "center-banner");
        }
        AuthPanel.setBadZoomAlertShown(true);
    }
}