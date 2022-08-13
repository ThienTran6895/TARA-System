var user = $.extend(user, {
    init: function() {
        $("#btnSubmit").on('click', user.saveOnClick);
    },

    saveOnClick: function(ev) {
        core.control.validatesave(ev);
    }
});


$(document).ready(function () {
    user.init();
});