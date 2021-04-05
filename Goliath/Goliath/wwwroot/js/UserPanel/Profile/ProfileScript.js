/*
 * Ajax // Manage clicking the profile
 * nav items.
 * When a nav item is clicked we load a partial view.
 */
$(window).ready(() => {
    $(".list-group .list-group-item").click((e) => {
        $(".list-group .list-group-item").removeClass("active");
        $(".list-group .list-group-item").removeClass("font-weight-bold");
        $(e.target).addClass("active");
        $(e.target).addClass("font-weight-bold");
    });
    $("#profile-nav-general").click(() => {
        $("#view").load("/UserPanel/GetModule?partialName=Profile/_General");
    });
    $("#profile-nav-colors").click(() => {
        $("#view").load("/UserPanel/GetModule?partialName=Profile/_Colors");
    });
    $("#profile-nav-security").click(() => {
        $("#view").load("/UserPanel/GetModule?partialName=Profile/_Security");
    });
    $("#profile-nav-manage-data").click(() => {
        $("#view").load("/UserPanel/GetModule?partialName=Profile/_ManageData");
    });
    $("#profile-nav-other").click(() => {
        $("#view").load("/UserPanel/GetModule?partialName=Profile/_Other");
    });
});