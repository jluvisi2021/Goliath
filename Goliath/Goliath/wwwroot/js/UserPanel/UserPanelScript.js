"use strict";

/**
 *
 * Namespace for executing JavaScript methods for
 * the UserPanel controller.
 *
 */
const UserPanelScript = (() => {
    return {
        /**
         * Update nav for userpanel clicks.
         * @param {string} name
         */
        updateNav: (name) => {
            $(document).ready(() => {
                $(".nav-item").removeClass("active");
                $("#" + name + "-nav").addClass("active");
            });
        }
    };
})();