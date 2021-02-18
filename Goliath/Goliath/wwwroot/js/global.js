
$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip()
});

function testjQuery() {
    $(document).ready(function () {
        console.log("jQuery has been found and enabled.");
       
    });
}

function redirectButtonClick(id, url) {
    $(document).ready(function () {
        $("input[id$="+id+"]").click(function () {
            window.location.href = url;
        });
    });
}
