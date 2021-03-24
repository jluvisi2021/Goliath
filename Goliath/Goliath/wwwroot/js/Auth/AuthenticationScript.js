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