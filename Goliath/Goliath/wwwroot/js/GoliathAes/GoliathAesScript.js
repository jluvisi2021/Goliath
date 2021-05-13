"use strict";
const AesToolScript = (() => {
    return {
        copyJsonString: () => {
            const copyText = $("#json");
            const $temp = $("<input>");
            $("body").append($temp);
            $temp.val($(copyText).html()).select();
            document.execCommand("copy");
            $temp.remove();
            alert("JSON string has been copied to clipboard.");
        }
    }
})();