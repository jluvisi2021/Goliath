/** RUNTIME PAGELOAD **/

/** Access data from authpanel.js */
const AuthPanel = (function () {
    /** Private Variables */
    let _zoomNotif = false;

    return {
        /**
         * Get the current value for the zoom error notification.
         * */
        getZoomNotif: function () {
            return _zoomNotif;
        },
        /**
         * Set the value for the zoom warning.
         * @param {boolean} val
         */
        setZoomNotif: function (val) {
            _zoomNotif = val;
        }
    };
})();

$(window).ready(function (e) {
    checkToggled();

    // Fixes a bug with the "Other" radio button where it would be clicked
    // and selected but the button would not be registered.
    $("#OtherHead").click(function (e) {
        if (e.target.id != "OtherDrop") {
            $("#OtherDrop").trigger("click");
            e.stopImmediatePropagation();
        }
    });
});
/* Page load end */

/** Access data from global.js */
const _AuthenticationPanel = (function () {
    return {
        /** Checks if user has graphic enabled */
        isGraphicEnabled: function () {
            return isGraphicEnabled();
        }
    };
})();

/**
 * Loads the colors of the radio button the user clicked on.
 * If the user clicked on a dropdown then it colors the dropdown elements.
 * @param {string} id
 */
function loadButtonElements(id) {
    const backgroundColor = "#0094ff";

    // Make the button colored
    if ($("#" + id + "RadioBtn").length === 0) {
        // The #OtherRadioBtn element does not change so we can leave it like this.
        $("#OtherRadioBtn").prop("checked", true);

        $("#" + id).css({
            "background-color": backgroundColor,
            "font-weight": "bold"
        });
    } else {
        $("#" + id + "RadioBtn").prop("checked", true);
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
 *
 *
 * Enables and disables the graphic.
 * Also allows to send the user a notification saying that there graphics have been changed.
 * @param {boolean} alert
 */
function toggleGraphic(alert) {
    if ($("#ShowGraphicIcon").hasClass("fas fa-eye-slash")) {
        localStorage.setItem("graphic", "enabled");
        $("#ShowGraphicIcon").attr("class", "fas fa-eye");
        $("#RightContainer").attr("class", "col-md-6 bg-light round-right");
        $("#RightContainer").attr("style", "");
        $("#LeftContainer").removeClass("invisible");
    } else {
        localStorage.setItem("graphic", "disabled");
        $("#ShowGraphicIcon").attr("class", "fas fa-eye-slash");
        $("#LeftContainer").addClass("invisible");
        $("#RightContainer").attr("class", "container bg-light round parent-container");
        $("#RightContainer").attr("style", "min-height: 100%; margin-top: 3%");
    }
    if (alert) {
        displayNotification("You changed the view.", "Your data is saved; You can undo this anytime.", GLOBAL.BannerTypes()["alert-primary"], "center-banner", "ChangedGraphicAlert");
    }
}

/**
 * Checks if the user attempted to zoom into the login page.
 * If they did then send them a notification only once.
 * */
function checkForPushBadResize() {
    GLOBAL.updateBrowserZoom();
    if (GLOBAL.getBrowserZoom() != GLOBAL.getDefaultBrowserZoom()) {
        if (AuthPanel.getZoomNotif() == false) {
            displayNotification("Warning", "Browser zooming is not supported. We reccomend using the default zoom unless visual errors have occured.", GLOBAL.BannerTypes()["alert-danger"], "center-banner");
        }
        AuthPanel.setZoomNotif(true);
    }
}