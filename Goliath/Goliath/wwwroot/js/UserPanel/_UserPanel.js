/**
 * Update the navbar to highlight a specific nav item.
 * @param {string} name
 */
function updateNav(name) {
    $(".nav-item").removeClass("active");
    $("#" + name + "-nav").addClass("active");
}