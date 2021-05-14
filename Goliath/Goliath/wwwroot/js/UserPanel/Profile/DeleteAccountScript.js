$(document).ready(() => {
    $("#input-feedback").keyup(() => {
        if ($("#input-feedback").val().length == 0) {
            $("#text-counter").text("Your feedback helps improve the experience for others!");
        } else {
            $("#text-counter").text("Character Limit: " + ($("#input-feedback").val().length) + "/300");
        }
    });
});
