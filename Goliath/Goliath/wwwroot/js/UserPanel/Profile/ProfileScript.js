"use strict";
const ProfileScript = (() => {
    /*
     * Ajax // Manage clicking the profile
     * nav items.
     * When a nav item is clicked we load a partial view.
     */
    $(document).ready(() => {
        const listItems = $(".list-group .list-group-item");
        const viewDiv = $("#view");
        listItems.click((e) => {
            listItems.removeClass("active").removeClass("font-weight-bold");
            $(e.target).addClass("active").addClass("font-weight-bold");
        });
        $("#profile-nav-general").click(() => {
            viewDiv.load("/UserPanel/GetModule?partialName=Profile/_General");
        });
        $("#profile-nav-colors").click(() => {
            viewDiv.load("/UserPanel/GetModule?partialName=Profile/_Colors");
        });
        $("#profile-nav-security").click(() => {
            viewDiv.load("/UserPanel/GetModule?partialName=Profile/_Security");
        });
        $("#profile-nav-manage-data").click(() => {
            viewDiv.load("/UserPanel/GetModule?partialName=Profile/_ManageData");
        });
        $("#profile-nav-other").click(() => {
            viewDiv.load("/UserPanel/GetModule?partialName=Profile/_Other");
        });
        $("#submit-settings").click(() => {
            $("#confirm-modal").load("/UserPanel/GetModule?partialName=Profile/_ConfirmSettingsModal");
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
            if (prev === "" || prev === null) {
                return false;
            }
            if (prev != curr) {
                return true;
            }
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
                settingUpdates.append(`
            <li class='list-group-item'>
                - Change Background Color from ` + dbValue + ` to ` + backgroundColor + `</li>`);
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
                settingUpdates.append(`
        <li class='list-group-item'>
            - Change Email from ` + dbValue + ` to ` + email + `</li>`);
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
                settingUpdates.append(`
        <li class='list-group-item'>
            - Change Phone Number to ` + phone + `</li>`);
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
                settingUpdates.append(`
        <li class='list-group-item'>
            - Change Account Password.` + `</li>`);
            }
        }
    }
})();
