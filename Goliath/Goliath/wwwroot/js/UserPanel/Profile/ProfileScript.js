"use strict";
/*
 * Ajax // Manage clicking the profile
 * nav items.
 * When a nav item is clicked we load a partial view.
 */
$(window).ready(() => {
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
});
