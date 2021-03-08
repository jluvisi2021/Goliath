const ProfileNav = (() => {

});
/*
 * Ajax // Manage clicking the profile
 * nav items.
 * When a nav item is clicked we load a partial view.
 */
$(window).ready(() => {
    $(".list-group .list-group-item").click(function (e) {
        $(".list-group .list-group-item").removeClass("active");
        $(e.target).addClass("active");
    });
    $("#profile-nav-general").click(function () {
        $("#view").load("/UserPanel/GetModule?partialName=Profile/_General");     
    });
    $("#profile-nav-colors").click(function () {
        $("#view").load("/UserPanel/GetModule?partialName=Profile/_Colors");     
    });
    $("#profile-nav-security").click(function () {
        $("#view").load("/UserPanel/GetModule?partialName=Profile/_Security");
    });
    $("#profile-nav-manage-data").click(function () {
        $("#view").load("/UserPanel/GetModule?partialName=Profile/_ManageData");
    });
    $("#profile-nav-other").click(function () {
        $("#view").load("/UserPanel/GetModule?partialName=Profile/_Other");
    });
});
