/**
 * Closure Scope
 *
 * */
// When the user scrolls to the bottom of a scrollable page remove the footer.
$(window).scroll(() => {
    if (!($(document).height() > $(window).height())) {
        $("#footer").css({
            "visibility": "visible"
        });
        return;
    }
    const scrollHeight = $(document).height();
    const scrollPosition = $(window).height() + $(window).scrollTop();

    if (scrollHeight - scrollPosition <= 1) {
        $("#footer").css({
            "visibility": "hidden"
        });
    } else {
        $("#footer").css({
            "visibility": "visible"
        });
    }
});
/** Access data from global.js */
const GlobalScript = (() => {
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
        BannerTypes: () => {
            return _bannerTypes;
        },
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
        displayNotification: (textHeader, text, type, divParentID) => {
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
        },
        /**
    * Displays a modal to the user.
    * The modal only has a close option and its buttons
    * are set to the primary color.
    * The modal is appended to the #RenderBody.
    *
    * @param {string} title [Supports HTML(Use ``)]
    * @param {string} body [Supports HTML(Use ``)]
    */
        displayModal: (title, body) => {
            $("#outer").append(`
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
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Okay</button>
                    </div>
                </div>
            </div>
        </div >
        `
            );
            $('#render-model').modal('show');
        },
        /**
         * Checks if the browser is supported.
         * If not then it sends a notification to the user.
         * Officially Supported Browsers: Chrome, all Chromium-based browsers, Safari, Firefox
         * @returns {boolean}
         * */
        checkBrowser: (action = true) => {
            if (navigator.userAgent.search("Chrome") >= 0) {
                return true;
            }
            else if (navigator.userAgent.search("Safari") >= 0) {
                return true;
            } else if (navigator.userAgent.search("Firefox") >= 0) {
                return true;
            }
            if (!action) {
                return false;
            }
            GlobalScript.displayNotification("Warning", "Goliath does not support this browser. Goliath may continue to function but not all features may work.", GlobalScript.BannerTypes()["alert-danger"], "center-banner");
            $("#footer-text").addClass("font-weight-bold");
            $("#footer-text").addClass("text-danger");
            $("#footer-text").html($("#footer-text").html() + ' [Unsupported]');
            $("#footer-text").attr("title", $("#footer-text").attr("title").toString() + ' <div class="font-weight-bold text-danger">WARNING: You are using an unsupported browser. Not all features may function correctly.</div>');
            return false;
        },
        /**
         * Loads a captcha partial view on top of the current view.
         * Requires the CSS ID of the HTML form.
         * Pound sign is not required.
         * REQUIRES: A <div> tag (or similar) with a #captcha-view ID to put the captcha into.
         * @param {string} formID
         */
        loadCaptcha: (formID) => {
            $("#captcha-view").load("/Captcha/LoadCaptcha?formID=" + formID);
        }
    };
})(); 