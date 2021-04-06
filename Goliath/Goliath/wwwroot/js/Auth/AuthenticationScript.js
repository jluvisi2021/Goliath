/** RUNTIME PAGELOAD **/

$(window).ready((e) => {
    // Fixes a bug with the "Other" radio button where it would be clicked
    // and selected but the button would not be registered.
    $("#other-head").click((e) => {
        if (e.target.id != "other-dropdown") {
            $("#other-dropdown").trigger("click");
            e.stopImmediatePropagation();
        }
    });
});

/* Page load end */

/** Background color for buttons. */
const backgroundColor = "#3e5af1";
/**
 * Loads the colors of the radio button the user clicked on.
 * If the user clicked on a dropdown then it colors the dropdown elements.
 * @param {string} id
 */
function loadButtonElements(id) {
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
}

/** Adjusts the theme of the login screen.
 *  Depends on the localStorage.*/
function changeTheme() {
    if (!(localStorage.getItem("darkTheme") === null || localStorage.getItem("darkTheme") === "enabled")) {
        return;
    }
    $("body").removeClass("gradient-light");
    $("body").addClass("gradient-dark");
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
    $("#signup-goliath").removeClass("btn-success");
    $("#signup-goliath").addClass("btn-dark");
    $("button").removeClass("btn-primary");
    $("button").addClass("btn-dark");
    $("i").addClass("text-white");
    $(".btn-outline-primary").css({
        "border-color": "#363636"
    });
    $("#toggle-dark").removeClass("btn-dark");
    $("#toggle-dark").addClass("btn-light");
    $("#toggle-dark").addClass("border-dark");

    $("#signin-google-icon").addClass("text-dark");
    $("#signin-fb-icon").addClass("text-dark");
    $("#signin-ms-icon").addClass("text-dark");
    $("#dd-git-icon").addClass("text-dark");
    $("#dropdown-divider").addClass("border-dark");
}

/**
 * Toggles if dark theme should be active.
 * */
function toggleDarkTheme() {
    if (localStorage.getItem("darkTheme") === null) {
        localStorage.setItem("darkTheme", "disabled");
        location.reload();
    } else if (localStorage.getItem("darkTheme") === "enabled") {
        localStorage.setItem("darkTheme", "disabled");
        location.reload();
    } else {
        localStorage.setItem("darkTheme", "enabled");
        location.reload();
    }
}