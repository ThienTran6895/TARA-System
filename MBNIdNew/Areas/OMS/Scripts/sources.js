var saveSources = $.extend(saveSources, {
    init: function () {
        $("#btnSave").on('click', saveSources.saveOnClick);
    },

    saveOnClick: function (ev) {
        core.control.validatesave(ev);
    }
});

$(document).ready(function () {
    saveSources.init();
});