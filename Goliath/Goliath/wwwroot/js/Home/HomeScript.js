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

/// Runtime ///

// Click for scroll.

/**
 * Namespace for all JavaScript methods and variables which
 * can be run on Action Methods within the HomeController.
 */
const HomeScript = (() => {

    function hello() {
        console.log("F");
    }
    hello();

    const scrollable = $("html,body");
    const homeNav = $("#home-nav");
    const featuresNav = $("#features-nav");
    const privacyNav = $("#privacy-nav");
    const contactNav = $("#contact-nav");
    const featuresSection = $("#features");
    const privacySection = $("#privacy");
    const contactSection = $("#contact");

    $("#scroll-btn").click(() => {
        scrollable.animate({
            scrollTop: scrollable.height() * 0.15
        }, 2000);
    });

    homeNav.click(() => {
        scrollable.animate({
            scrollTop: 0
        }, 2000);
    });

    featuresNav.click(() => {
        scrollable.animate({
            scrollTop: featuresSection.position().top * 0.95
        }, 2000);
    });

    privacyNav.click(() => {
        scrollable.animate({
            scrollTop: privacySection.position().top * 0.95
        }, 2000);
    });

    contactNav.click(() => {
        scrollable.animate({
            scrollTop: scrollable.height()
        }, 2000);
    });

    // Variables to handle when a different section is entered.
    const scrollFeatures = featuresSection.position().top * 0.8;
    const scrollPrivacy = privacySection.position().top * 0.8;
    const scrollContact = contactSection.position().top * 0.8;

    // Check if scroll has reached different points.
    $(document).scroll(() => {
        HomeScript.updateNav();
    });

    $(document).ready(() => {
        HomeScript.updateNav();
    });

    return {
        /** 
         * Updates the navbar depending on where the users
         * scrollbar is.
         */
        updateNav: () => {
            if ($(this).scrollTop() <= scrollFeatures) {
                $(".nav-item").removeClass("active");
                homeNav.addClass("active");
            }

            if ($(this).scrollTop() >= scrollFeatures
                && $(this).scrollTop() <= scrollPrivacy) {
                $(".nav-item").removeClass("active");
                featuresNav.addClass("active");
            }
            if ($(this).scrollTop() >= scrollPrivacy
                && $(this).scrollTop() <= scrollContact) {
                $(".nav-item").removeClass("active");
                privacyNav.addClass("active");
            }
            if ($(this).scrollTop() >= scrollContact) {
                $(".nav-item").removeClass("active");
                contactNav.addClass("active");
            }
        }
    }
})();
