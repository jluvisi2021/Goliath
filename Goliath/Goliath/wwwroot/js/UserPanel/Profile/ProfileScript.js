"use strict";
const ProfileScript = (() => {
    /*
     * Ajax // Manage clicking the profile
     * nav items.
     * When a nav item is clicked we load a partial view.
     */
    $(document).ready(() => {
        const listItems = $(".list-group .list-group-item");
        let showSecurity = false;
        listItems.click((e) => {
            listItems.removeClass("active").removeClass("font-weight-bold");
            $(e.target).addClass("active").addClass("font-weight-bold");
        });

        $("#submit-settings").click(() => {
            $("#confirm-modal").load("/UserPanel/GetModule?partialName=Profile/_ConfirmSettingsModal");
        });
        $("#submit-verify-phone-form").click(() => {
            GlobalScript.loadCaptcha("verify-phone-form");
        });
        if (GlobalScript.checkBrowser(false)) {
            $("#browser-supported").addClass("font-weight-bold").addClass("text-success").text("Yes");
        } else {
            $("#browser-supported").addClass("font-weight-bold").addClass("text-danger").text("No");
        }
        $("#profile-timezone").text(GlobalScript.getTimeZone());
        $("#show-account-security").click(() => {
            if (showSecurity) {
                $("#account-security").css({ "display": "none" });
                $("#show-account-security-icon")
                    .removeClass("fa-chevron-circle-up")
                    .addClass("fa-chevron-circle-down");

                showSecurity = false;
            } else {
                $("#account-security").css({ "display": "unset" });
                $("#show-account-security-icon")
                    .removeClass("fa-chevron-circle-down")
                    .addClass("fa-chevron-circle-up");
                showSecurity = true;
            }
        });
    });
    return {
        /**
         * Checks if a value is different between the database and the form.
         * @param {string} prev
         * @param {string} curr
         * @returns {boolean} If the values are different.
         */
        valueChanged: (prev, curr) => {
            if (prev === "" || prev === null || typeof prev === "undefined") {
                return false;
            }
            if (prev != curr) {
                return true;
            }
            return false;
        },
        /**
         * Checks the background value in the settings form and
         * compares it with the database. If they are different then it adds
         * an item to the settings confirm modal.
         * @param {string} dbValue
         */
        backgroundUpdated: (dbValue) => {
            const settingUpdates = $("#settings-updates");
            const backgroundColor = $("#bg-color-setting").val();
            if (ProfileScript.valueChanged(backgroundColor, dbValue)) {
                const id = GlobalScript.createUUID();
                // Add a list element to the list.
                settingUpdates.append("<li id=" + id + " class='list-group-item'></li>");
                // Get decoded HTML (Anti-XSS)
                $("#" + id).text("- Change Background Color from " + dbValue + " to " + backgroundColor);
            }
        },
        /**
         * Checks the dark theme value in the settings form and
         * compares it with the database. If they are different then it adds
         * an item to the settings confirm modal.
         * @param {string} dbValue
         */
        themeUpdated: (dbValue) => {
            const settingUpdates = $("#settings-updates");
            const lightTheme = $("#theme-setting");
            const darkTheme = $("#theme-setting-2");
            if (lightTheme.prop("checked") === true) {
                if (ProfileScript.valueChanged(lightTheme.val(), dbValue)) {
                    // Not vulnerable to XSS because no JS elements are being appended.
                    settingUpdates.append(`
        <li class='list-group-item'>
            - Change Dark Theme to Disabled.` + `</li>`);
                }
            } else {
                if (ProfileScript.valueChanged(darkTheme.val(), dbValue)) {
                    settingUpdates.append(`
        <li class='list-group-item'>
            - Change Dark Theme to Enabled.` + `</li>`);
                }
            }
        },

        /**
         * Checks the email value in the settings form and
         * compares it with the database. If they are different then it adds
         * an item to the settings confirm modal.
         * @param {string} dbValue
         */
        emailUpdated: (dbValue) => {
            const settingUpdates = $("#settings-updates");
            const email = $("#email-setting").val();

            if (ProfileScript.valueChanged(email, dbValue)) {
                // Create a UUID to append the list element.
                const id = GlobalScript.createUUID();
                // Add a list element to the list.
                settingUpdates.append("<li id=" + id + " class='list-group-item'></li>");
                // Get decoded HTML (Anti-XSS)
                $("#" + id).text("- Change Email from " + dbValue + " to " + email);
            }
        },
        /**
         * Gets if the logout threshold was updated.
         * @param {any} dbValue
         */
        logoutThresholdUpdated: (dbValue) => {
            const settingUpdates = $("#settings-updates");
            const threshold = $("#logout-threshold-setting").val();
            if (threshold === "") {
                return;
            }
            if (ProfileScript.valueChanged(threshold, dbValue)) {
                // Create a UUID to append the list element.
                const id = GlobalScript.createUUID();
                // Add a list element to the list.
                settingUpdates.append("<li id=" + id + " class='list-group-item'></li>");
                // Get decoded HTML (Anti-XSS)
                $("#" + id).text("- Change Logout Threshold from " + dbValue + " to " + threshold);
            }
        },
        /**
         * Checks the phone number value in the settings form and
         * compares it with the database. If they are different then it adds
         * an item to the settings confirm modal.
         * @param {string} dbValue
         */
        phoneUpdated: (dbValue) => {
            const settingUpdates = $("#settings-updates");
            const phone = $("#phone-setting").val();
            if (ProfileScript.valueChanged(phone, dbValue)) {
                const id = GlobalScript.createUUID();
                // Add a list element to the list.
                settingUpdates.append("<li id=" + id + " class='list-group-item'></li>");
                // Get decoded HTML (Anti-XSS)
                $("#" + id).text("- Change Phone number to " + phone);
            }
        },
        /**
         * Checks to see if the password field has been updated and if it has
         * then it adds an item to the settings confirm modal.
         */
        passwordUpdated: () => {
            const settingUpdates = $("#settings-updates");
            const password = $("#password-setting").val();
            if (password != null && password != "") {
                // Not vulnerable to XSS because no JS elements are appended.
                settingUpdates.append(`
        <li class='list-group-item'>
            - Change Account Password.` + `</li>`);
            }
        }
    };
})();