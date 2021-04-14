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
         */
        valueChanged: (prev, curr) => {
            if (prev === "" || prev === null) {
                return false;
            }
            if (prev != curr) {
                return true;
            }
        }
    }
})();