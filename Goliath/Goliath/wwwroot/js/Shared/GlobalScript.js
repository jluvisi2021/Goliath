"use strict";
/*
 * JavaScript which should be accessed in all browser windows.
 * Hence name: "GlobalScript"
 */
/**
 * Namespace for the GlobalScript.js which contains
 * universal methods and variables that each
 * controller and action method can run.
 *
 */
const GlobalScript = (() => {
    /*Types of banners for notifications on home screen */
    const _bannerTypes = Object.freeze({
        "alert-primary": 0,
        "alert-secondary": 1,
        "alert-success": 2,
        "alert-danger": 3,
        "alert-dark": 4
    });

    // Track the time (in minutes) that there has been no mouse movement.
    let idleTime = 0;

    // When the user scrolls to the bottom of a scrollable page remove the footer.
    $(window).scroll(() => {
        const footerElement = $("#footer");
        if (!($(document).height() > $(window).height())) {
            footerElement.css({
                "visibility": "visible"
            });
            return;
        }
        const scrollHeight = $(document).height();
        const scrollPosition = $(window).height() + $(window).scrollTop();
        if (scrollHeight - scrollPosition <= 1) {
            footerElement.css({
                "visibility": "hidden"
            });
        } else {
            footerElement.css({
                "visibility": "visible"
            });
        }
    });
    // Run immediately.
    $(document).ready(() => {
        // General javascript.

        $('[data-toggle="popover"]').popover();

        ////////////////////////////////////////////////////////////////////////////////
        // Load the saved notifications in localStorage.
        GlobalScript.loadSavedNotifications();

        // When a notification is closed.
        $(".close").click((e) => {
            // Get id of the notification and delete it from localStorage.
            GlobalScript.deleteNotification($(e.target).closest('div').attr('id'));
        });
        $("form").submit(() => {
            GlobalScript.renderLoading();
        });

        ////////////////////////////////////////////////////////////////////////////////

        // Setup Logout Threshold
        setInterval(GlobalScript.timerIncrement, 10000); // 1 minute

        //Zero the idle timer on mouse movement or key press.
        $(this).mousemove(function (e) {
            idleTime = 0;
        });
        $(this).keypress(function (e) {
            idleTime = 0;
        });

    });
    return {
        /** Returns the list of avaliable banner types. */
        BannerTypes: () => {
            return _bannerTypes;
        },
        timerIncrement: () => {
            // Increment idle time.
            idleTime = idleTime + 1;

            // Check if the idle time is greater then the logout threshold.
            if (idleTime > sessionStorage.getItem("logoutThreshold") && sessionStorage.getItem("logoutThreshold") > 0) { 
                // Log the user out.
                window.location.href = "/logout";
            }
        },
        /**
         * Takes a notification and makes it into an object and appends
         * it to a JSON which is stored in local storage.
         * @param {string} textHeader
         * @param {string} text
         * @param {string} type
         * @param {string} divParentID
         * @param {number} id (Auto Generated)
         */
        saveNotification: (textHeader, text, type, divParentID, id) => {
            // Object to repersent a single notification.
            const notification = {
                id: id,
                header: textHeader,
                body: text,
                type: type,
                parentDiv: divParentID
            }
            // List to store notifications.
            let notifications = [];
            // Curent text in local storage.
            const storageValue = localStorage.getItem("Notifications");
            // If it is the first notification then...
            if (storageValue === null || storageValue == "[]") {
                // Make it a JSON and add square brackets for more notifications.
                notifications.push("[" + JSON.stringify(notification) + "]");
                // Set the value as the array.
                localStorage.setItem("Notifications", notifications);
                return;
            }
            // Parse the current storage value to make a JSON that stores multiple items.
            notifications.push(
                "[" // Leading Bracket.
                +
                storageValue // Current text in local storage.
                    .replace("[", "") // Remove previous leading brackets.
                    .replace("]", "") +
                "," // Add a comma to separate this new JSON object.
                +
                JSON.stringify(notification) // Turn the JSOn into a string.
                +
                "]"); // Add ending grouping bracket.
            localStorage.setItem("Notifications", notifications);
        },
        /**
         * Deletes the notification with the specified ID from local storage.
         * @param {any} id
         */
        deleteNotification: (id) => {
            let notifications = JSON.parse(localStorage.getItem("Notifications"));
            if (notifications === null) {
                return;
            }
            for (let i = 0; i < notifications.length; i++) {
                const notification = notifications[i];
                if (notification.id === id) {
                    if (i > -1) {
                        notifications.splice(i, 1);
                    }
                }
            }
            localStorage.setItem("Notifications", JSON.stringify(notifications));
        },
        /**
         * Gets the current text in localStorage. Turns it into a JSON object.
         * Displays notifications on the screen for each notification object in the JSON object.
         */
        loadSavedNotifications: () => {
            let notifications = JSON.parse(localStorage.getItem("Notifications"));
            // If no notifications are saved.
            if (notifications === null) {
                return;
            }
            for (let i = 0; i < notifications.length; i++) {
                const notification = notifications[i];
                // Display the notification but do not save it in localStorage because it is already there.
                GlobalScript.displayNotification(notification.header, notification.body, _bannerTypes[notification.type], notification.parentDiv, false, notification.id);
            }
        },
        /**
         * Automatically displays a notification alert at a parent div.
         * This function makes a new child banner immediately below the parent div.
         *
         * This function also grants the option on whether or not to instantly display it.
         *
         * If save=true, the notification will display immediately without reload but will
         * not be saved in local storage.
         * If save=false, the notification will not display but will be saved as a JSON to the
         * "Notifications" key in localStorage and will display on each new window until it
         * is closed.
         *
         * Each notification is given a random ID to recognize it.
         *
         * Types:
         * 1- Primary (Dark Blue)
         * 2- Secondary (Blue)
         * 3- Success (Green)
         * 4- Error (Red)
         * 5- Primary Dark (Dark)
         *
         * @param {string} textHeader (BOLD TEXT)
         * @param {string} text
         * @param {GlobalScript.BannerTypes} type
         * @param {string} divParentID
         * @param {boolean} save (Should the notification be saved in localStorage?)
         */
        displayNotification: (textHeader, text, type, divParentID, save = true, id = "") => {
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
                    return;
            }
            if (id === "") {
                id = GlobalScript.createUUID();
            }
            if (id.length != 36) {
                console.error("UUID Security: Invalid UUID.");
                return;
            }
            if (save) {
                GlobalScript.saveNotification(textHeader, text, typeStr, divParentID, id);
            }
            // Notification will not be saved to localStorage so we display it now with DOM Injection.
            if (!save) {
                $("#" + divParentID).prepend().html(
                    '<div id=' + id + ' class="alert ' + typeStr + ' alert-dismissible fade show" role="alert">' +
                    '<button type="button" class="close" data-dismiss="alert" aria-label="Close">' +
                    '<span aria-hidden="true">×</span>' +
                    '</button>' +
                    '<strong id=' + id + '-strong></strong> ' +
                    '<span id=' + id + '-body></span>' +
                    '</div>'
                );
                // Encode Potential HTML Tags
                $("#" + id + "-strong").text(textHeader);
                $("#" + id + "-body").text(text);
            }
        },
        /**
         * Creates a random UUID.
         * Credit: https://www.w3resource.com/javascript-exercises/javascript-math-exercise-23.php
         */
        createUUID: () => {
            let dt = new Date().getTime();
            const uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
                const r = (dt + Math.random() * 16) % 16 | 0;
                dt = Math.floor(dt / 16);
                return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
            });
            return uuid;
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
        `);
            $('#render-model').modal('show');
        },
        /**
         * Checks if the browser is supported.
         * If not then it sends a notification to the user.
         * Officially Supported Browsers: Chrome, all Chromium-based browsers, Safari, Firefox
         * @param {boolean} action -> TRUE (Default) if the method should send notification to #center-banner.
         * @returns {boolean} -> If the browser is supported.
         * */
        checkBrowser: (action = true) => {
            if (navigator.userAgent.search("Chrome") >= 0) {
                return true;
            } else if (navigator.userAgent.search("Safari") >= 0) {
                return true;
            } else if (navigator.userAgent.search("Firefox") >= 0) {
                return true;
            }
            if (!action) {
                return false;
            }
            GlobalScript.displayNotification("Warning", "Goliath does not support this browser. Goliath may continue to function but not all features may work.", GlobalScript.BannerTypes()["alert-danger"], "center-banner");
            const footerElementText = $("#footer-text");
            footerElementText.addClass("font-weight-bold")
                .addClass("text-danger")
                .html(footerElementText.html() + ' [Unsupported]')
                .attr("title", footerElementText.attr("title").toString() + ' <div class="font-weight-bold text-danger">WARNING: You are using an unsupported browser. Not all features may function correctly.</div>');
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
        },
        /**
         * Renders the loading screen.
         */
        renderLoading: () => {
            // Some info
            console.log("Submit!");
        },
        /**
         * Returns the time zone of the user.
         */
        getTimeZone: () => {
            const date = new Date();
            return date.toLocaleTimeString();
        }
    };
})();