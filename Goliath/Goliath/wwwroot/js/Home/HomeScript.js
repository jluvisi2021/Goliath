"use strict";
// Setup animated background with Vanta.JS.
{
    VANTA.NET({
        el: "#vanta-js", // Div element to change.
        mouseControls: true,
        touchControls: true,
        gyroControls: false,
        minHeight: 200.00,
        minWidth: 200.00,
        scale: 1.00,
        scaleMobile: 1.00,
        color: 0x686868,
        backgroundColor: 0x2a2a2e,
        maxDistance: 22.00,
        spacing: 16.00
    });
    // Make background visible after loading.
    $("#vanta-js").css({
        "visibility": "visible"
    });


}


/**
 * Namespace for all JavaScript methods and variables which
 * can be run on Action Methods within the HomeController.
 */
const HomeScript = (() => {
    return {

    }
})();
