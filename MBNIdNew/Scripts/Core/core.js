
if (typeof (parent.core) == 'object') {
    core = parent.core;
}
else {
    var core = core || {};

    var core = jQuery.extend(core, {


        control: {

            // Initialize control scripts
            init: function () {
                this.maskAllInputs();
            },
            reWidthTextInput: function (textInput) {
                var textLength = textInput.val().length;
                if (textLength + 1 > 20)
                    textInput.attr("cols", textInput.val().length + 1);
                else {
                    textInput.attr("cols", 20);
                }
            },

            //-- Mask all numeric mask with max-length
            maskAllInputs: function () {
                // AsFormatAttribute:
                $("input[data-val-asformat-mask]").each(function (i, e) {
                    var mask = $(e).attr("data-val-asformat-mask");
                    $(e).mask(mask);
                });

                $("input[data-val-asformat-type='Time'],input[data-val-asformat-type='Date']").each(function (i, e) {
                    $(e).attr("data-val-date", $(e).attr("data-val-asformat"));
                });
            },

            maskInput: function (input, isCalled) {
                typeof isCalled == "undefined" ? false : isCalled;
                var type = input.attr('display-masked');
                var maxLength = input.attr('max-length');
                if (maxLength == undefined) {
                    maxLength = input.attr('maxlength');
                }
                if (type != '' && type != undefined) {
                    type = type.toLowerCase();
                    switch (type) {
                        case 'phone':
                            input.mask("(000) 000-0000");
                            break;
                        case 'date':
                            input.mask("00/00/0000");
                            break;
                        case 'ssn':
                            input.mask("000-00-0000");
                            break;
                        default:
                    }
                }
            },

            validatesave: function (ev, callback, url) {

                $("input").val(function (_, value) {
                    return $.trim(value);
                });

                var frm = $(ev.target).closest('form');

                var validator = frm.kendoValidator().data("kendoValidator");

                if (!validator.validate())
                    return false;
                else {
                    if (typeof url == "undefined") {
                        url = frm.attr("action");
                    }
                    $.ajax({
                        url: url,
                        type: "POST",
                        data: frm.serialize(),
                        success:
                            function (res) {
                                if (res.Result == "success") {
                                    if (callback) {
                                        callback(res.Data);
                                    }
                                    else {
                                        swal({ title: res.Title, text: res.Message, type: "success" }, function () {
                                            if (res.URLList != null)
                                                window.location.href = res.URLList;
                                            else
                                                window.location.href = res.URLEdit;
                                        });
                                    }                                   
                                }
                                else {
                                    swal({ title: res.Title, text: res.Message, type: "error", confirmButtonText: "Đóng" });
                                }
                            },
                        error: function (data) {
                            swal({ title: "Lỗi", text: "Hệ thống đang xảy ra lỗi, vui lòng thao tác lại!", type: "error", confirmButtonText: "Đóng" });
                            return false;
                        }
                    });
                }
                return true;
            },

            validateOnly: function (ev) {
                var frm = $(ev.target).closest('form');
                var validator = frm.kendoValidator({
                    messages: {
                        date: core.resources.global.InvalidDate //Override date message default
                    }
                }).data("kendoValidator");
                return validator.validate();
            }
        }
    });
}

