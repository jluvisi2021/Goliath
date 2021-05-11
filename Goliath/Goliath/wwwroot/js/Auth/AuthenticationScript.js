/**
 * JavaScript methods and variables that control
 * DOM manipulation for the "Auth" Controller.
 * */
const AuthScript = (() => {
    /** Background color for buttons. */
    const backgroundColor = "#3e5af1";
    $(window).ready(() => {
        // Fixes a bug with the "Other" radio button where it would be clicked
        // and selected but the button would not be registered.
        $("#other-head").click((e) => {
            if (e.target.id != "other-dropdown") {
                $("#other-dropdown").trigger("click");
                e.stopImmediatePropagation();
            }
        });

        $("#toggle-dark").click(() => {
            AuthScript.toggleDarkTheme();
        });
        
        $("#login-submit-btn").click(() => {
            GlobalScript.loadCaptcha('login-form');
        });
        $("#register-submit-btn").click(() => {
            GlobalScript.loadCaptcha('register-form');
        });
        if (!navigator.cookieEnabled) {
            GlobalScript.displayNotification("Warning", "Some Goliath functions may not work without Cookies enabled.", GlobalScript.BannerTypes()["alert-danger"], "center-banner", false);
        }
    });
    return {
        /**
         * Changes the colors of the radio buttons on the
         * authentication screen.
         * Also changes the color of the dropdown elements.
         * @param {string} id HTML ID of the DOM element.
         */
        loadButtonElements: (id) => {
            // Make the button colored
            if ($("#" + id + "-radio-btn").length === 0) {
                // The #other-radio-btn element does not change so we can leave it like this.
                $("#other-radio-btn").prop("checked", true);
                $("#" + id).css({
                    "background-color": backgroundColor,
                    "color": "white"
                });
            } else {
                $("#" + id + "-radio-btn").prop("checked", true);
            }
        },
        /**
         * Swaps between light and dark theme depending on
         * values in the localStorage.
         * */
        changeTheme: () => {
            if (localStorage.getItem("darkTheme") === null || localStorage.getItem("darkTheme") === "disabled") {
                return;
            }
            $("body").removeClass("gradient-light")
                .addClass("gradient-dark");
            $("#footer").css({
                "background-color": "#585858",
                "color": "#FFFFFF"
            });
            $("#right-container").css({
                "background-color": "#d2d2d2"
            });
            $("input").css({
                "background-color": "#c2c2c2"
            });
            $("#dropdown").css({
                "background-color": "#c2c2c2"
            });
            $("#signup-goliath").removeClass("btn-success")
                .addClass("btn-dark");
            $("button").removeClass("btn-primary")
                .addClass("btn-dark");
            $("i").addClass("text-white");
            $(".btn-outline-primary").css({
                "border-color": "#363636"
            });
            $("#toggle-dark").removeClass("btn-dark")
                .addClass("btn-light")
                .addClass("border-dark");
            $("#signin-google-icon").addClass("text-dark");
            $("#signin-fb-icon").addClass("text-dark");
            $("#signin-ms-icon").addClass("text-dark");
            $("#dd-git-icon").addClass("text-dark");
            $("#dropdown-divider").addClass("border-dark");
        },
        /**
         * A method to be run on button press.
         * Updates the localStorage.
         * */
        toggleDarkTheme: () => {
            if (localStorage.getItem("darkTheme") === null) {
                localStorage.setItem("darkTheme", "enabled");
                location.reload();
            } else if (localStorage.getItem("darkTheme") === "enabled") {
                localStorage.setItem("darkTheme", "disabled");
                location.reload();
            } else {
                localStorage.setItem("darkTheme", "enabled");
                location.reload();
            }
        }
    }
})();