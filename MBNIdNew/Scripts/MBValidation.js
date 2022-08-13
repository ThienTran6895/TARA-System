
(function ($, kendo) {

    function extractParams(input, ruleName) {
        var params = {},
        index,
        data = input.data(),
        length = ruleName.length,
        rule,
        key;

        for (key in data) {
            rule = key.toLowerCase();
            index = rule.indexOf(ruleName);
            if (index > -1) {
                rule = rule.substring(index + length, key.length);
                if (rule) {
                    params[rule] = data[key];
                }
            }
        }
        return params;
    }

    function getMessage(input, ruleName) {
        return input.attr("data-val-" + ruleName);
    }

    //var validators = $("#myform").kendoValidator().data('kendoValidator');

    $.extend(true, kendo.ui.validator, {
        messageLocators: {
            mvcLocator: {
                decorate: function (message, fieldName) {
                    message
                        .addClass("boarding-val-container")
                        .attr("data-valmsg-for", fieldName || "");
                }
            }
        },
        rules: {
            format: function (input, params) {
                if (input.filter("[data-val-asformat]").length) {
                    var regex = new RegExp(input.attr("data-val-asformat-regex"));
                    if (input.val() != "") {
                        if (input.attr("data-val-asformat-type") == "Time") {
                            var time = $(input).data('kendoTimePicker').value();
                            return time != null;
                        }
                        else if (input.attr("data-val-asformat-type") == "Number") {
                            return DetectNumber(input.val());
                        }
                        return regex.test(input.val());
                    }
                }
                return true;
            },
            urlvalidation: function (input, param) {
                if (input.filter("[data-val-urlvalidation]").length) {
                    var regex = new RegExp(input.attr("data-val-urlvalidation-regex"));
                    if (input.val() != "" && !regex.test(input.val().toLowerCase())) {
                        return false;
                    }
                    return true;
                }
                return true;
            },
            notgreaterthan: function (input, params) {
                if (input.filter("[data-val-notgreaterthan]").length && input.val() != "") {

                    params = extractParams(input, "notgreaterthan");
                    return parseFloat(input.val()) <= parseFloat(params["refvalue"]);
                }
                return true;
            },
            minvalue: function (input, params) {
                if (input.filter("[data-val-minvalue]").length) {
                    var maxValue = input.attr("data-val-minvalue-value");
                    maxValue = maxValue.replace(/,/g, '');
                    if (parseFloat(input.val()) < parseFloat(maxValue)) {
                        return false;
                    }
                    return true;
                }
                return true;
            },
            maxvalue: function (input, params) {
                if (input.filter("[data-val-maxvalue]").length) {
                    var maxValue = input.attr("data-val-maxvalue-value");
                    maxValue = maxValue.replace(/,/g, '');
                    if (parseFloat(input.val()) > parseFloat(maxValue)) {
                        return false;
                    }
                    return true;
                }
                return true;
            },
            greaterthanvalue: function (input, params) {
                if (input.filter("[data-val-greaterthanvalue]").length) {
                    var minValue = input.attr("data-val-greaterthanvalue-value");
                    minValue = minValue.replace(/,/g, '');
                    if (input.val() != "" && parseFloat(input.val()) <= parseFloat(minValue)) {
                        return false;
                    }
                    return true;
                }
                return true;
            },

            equalvalue: function (input, params) {
                if (input.filter("[data-val-equalvalue]").length) {
                    var equal = input.attr("data-val-equalvalue-equalvalue");
                    if (input.val() == equal) {
                        return true;
                    } else {
                        return false;
                    }
                }
                return true;
            },
            equalvalues: function (input, params) {
                if (input.filter("[data-val-equalvalues]").length) {
                    params = extractParams(input, "equalvalues");
                    var lstValue = params['refvalue'].split("|");
                    var isEqual = false;
                    for (i = 0; i < lstValue.length; i++) {
                        if (input.val() == lstValue[i]) {
                            isEqual = true;
                            break;
                        }
                    }
                    if (!isEqual) {
                        return false;
                    }
                    return true;
                }
                return true;
            },
            emailvalidation: function (input, param) {
                if (input.filter("[data-val-emailvalidation]").length) {
                    var regex = new RegExp(input.attr("data-val-emailvalidation-regex"));
                    if (input.val() != "" && !regex.test(input.val())) {
                        return false;
                    }
                    return true;
                }
                return true;
            },
            datemustbepriorthancurrentdate: function (input, param) {
                if (input.filter("[data-val-datemustbepriorthancurrentdate]").length) {
                    var inputValue = input.val();
                    if (inputValue == null || inputValue == undefined || inputValue == "") {
                        return true;
                    }

                    return !DateMustBeEqualOrGreaterThanCurrentDate(input);
                }
                return true;
            },
            datemustbeequalorpriorthancurrentdate: function (input, param) {
                if (input.filter("[data-val-datemustbeequalorpriorthancurrentdate]").length) {
                    return DateMustBeEqualOrLessThanCurrentDate(input);
                }
                return true;
            },
            datemustbeequalorgreaterthancurrentdate: function (input, param) {
                if (input.filter("[data-val-datemustbeequalorgreaterthancurrentdate]").length) {
                    var inputValue = input.val();
                    if (inputValue == null || inputValue == undefined || inputValue == "") {
                        return true;
                    }

                    return DateMustBeEqualOrGreaterThanCurrentDate(input);
                }
                return true;
            },
            minlength: function (input, params) {
                if (input.filter("[data-val-minlength]").length) {
                    var length = input.attr("data-val-minlength-value");
                    if (input.val() != "" && input.val().length < parseInt(length)) {
                        return false;
                    }
                    return true;
                }
                return true;
            },
            maxlength: function (input, params) {
                if (input.filter("[data-val-maxlength]").length) {
                    var length = input.attr("data-val-maxlength-value");
                    if (input.val() != "" && input.val().length > parseInt(length)) {
                        return false;
                    }
                    return true;
                }
                return true;
            },

            required: function (input, params) {
                if (input.filter("[data-val-required]").length) {
                    if (input.val() != null && input.val().trim() == "")
                        return false;
                    return true;
                }
                return true;
            },
            comparepropertyvalidaton: function (input, params) {
                if (input.filter("[data-val-comparepropertyvalidaton]").length) {
                    var isValid = true;
                    params = extractParams(input, "comparepropertyvalidaton");

                    var operator = params["validationoperator"];
                    var refproperty = $("#" + params["refproperty"]).val() != null ? parseFloat($("#" + params["refproperty"]).val()) : 0;

                    if (input.val() != null) {
                        switch (operator) {
                            case "GreaterThan":
                                if (parseFloat(input.val()) <= refproperty)
                                    isValid = false;
                                break;
                            case "GreaterThanOrEqual":
                                if (parseFloat(input.val()) < refproperty)
                                    isValid = false;
                                break;
                            case "LessThan":
                                if (parseFloat(input.val()) >= refproperty)
                                    isValid = false;
                                break;
                            case "LessThanOrEqual":
                                if (parseFloat(input.val()) > refproperty)
                                    isValid = false;
                                break;
                            case "EqualTo":
                                if (parseFloat(input.val()) != refproperty)
                                    isValid = false;
                                break;
                        }
                    }
                    return isValid;

                }
                return true;
            },
        },

        messages: {
            format: function (input) {
                return getMessage(input, "asformat");
            },
            urlvalidation: function (input) {
                return getMessage(input, "urlvalidation");
            },
            notgreaterthan: function (input) {
                return getMessage(input, "notgreaterthan");
            },
            minvalue: function (input) {
                return getMessage(input, "minvalue");
            },
            maxvalue: function (input) {
                return getMessage(input, "maxvalue");
            },
            greaterthanvalue: function (input) {
                return getMessage(input, "greaterthanvalue");
            },
            equalvalue: function (input) {
                return getMessage(input, "equalvalue");
            },
            equalvalues: function (input) {
                return getMessage(input, "equalvalues");
            },
            equallength: function (input) {
                return getMessage(input, "equallength");
            },
            emailvalidation: function (input) {
                return getMessage(input, "emailvalidation");
            },
            datemustbepriorthancurrentdate: function (input) {
                return getMessage(input, "datemustbepriorthancurrentdate");
            },
            datemustbeequalorpriorthancurrentdate: function (input) {
                return getMessage(input, "datemustbeequalorpriorthancurrentdate");
            },
            datemustbeequalorgreaterthancurrentdate: function (input) {
                return getMessage(input, "datemustbeequalorgreaterthancurrentdate");
            },
            minlength: function (input) {
                return getMessage(input, "minlength");
            },
            maxlength: function (input) {
                return getMessage(input, "maxlength");
            },
            required: function (input) {
                return getMessage(input, "required");
            },
            comparepropertyvalidaton: function (input) {
                return getMessage(input, "comparepropertyvalidaton");
            }
        }
    });


})(jQuery, kendo);


function SpecialFormatDateMustBeEqualOrGreaterThanCurrentDate(input) {
    if (input.attr('data-val-specialformatdateequalorgreaterthancurrentdate') == null) {
        return true;
    }
    var inputValue = input.val();
    if (inputValue == null || inputValue == undefined || inputValue == "") {
        return true;
    }
    var dateformat = input.attr('data-val-specialformatdateequalorgreaterthancurrentdate-dateformat');
    if (dateformat != null && dateformat.length != 0 && dateformat != "") {
        if (dateformat == "MM/YY" && inputValue.length == 5) {
            try {
                var arr = inputValue.split("/");
                var currentDate = new Date();
                var currentyear = currentDate.getFullYear() % 100;
                var currentmonth = currentDate.getMonth() + 1; // added +1 because javascript counts month from 0 

                var year = parseInt(arr[1]);
                var month = parseInt(arr[0]);
                if (year != NaN && year <= currentyear) {
                    if (year < currentyear) {
                        return false;
                    } else if (month < currentmonth) {
                        return false;
                    }
                }
            } catch (e) { }
        }
    }

    return true;
}


Array.prototype.unique = function () {
    var r = new Array();
    o: for (var i = 0, n = this.length; i < n; i++) {
        for (var x = 0, y = r.length; x < y; x++) {
            if (r[x] == this[i])
                continue o;
        }
        r[r.length] = this[i];
    }
    return r;
}

function SpecialFormatDateMustBeEqualOrGreaterThanRefDate(input) {
    if (input.attr('data-val-specialformatdatemustbepriorthanrefdate') != null
        || input.attr('data-val-specialformatdateequalorgreaterthanrefdate') != null) {
        // make sure entered datas is validated format correctly at previous data attribute.
        var inputValue = input.val();
        var refpropertyName = input.attr('data-val-specialformatdateequalorgreaterthanrefdate-refdateproperty');
        if (refpropertyName == null || refpropertyName == "") {
            return true;
        }
        var refProperty = $("input[id=" + refpropertyName + "]");
        if (refProperty == null || refProperty == undefined) {
            return true;
        }
        var refValue = refProperty.val();
        if (inputValue == null || inputValue == "") {
            return true;
        }

        if (refValue == null || refValue == "") {
            return true;
        }
        var dateformat = input.attr('data-val-specialformatdateequalorgreaterthanrefdate-dateformat');
        if (dateformat != null && dateformat.length != 0
            && dateformat != "") {
            if (dateformat == "MM/YY") {
                try {
                    var dateSeperate = inputValue.split("/");
                    var refDateSeperate = refValue.split("/");

                    var refyear = parseInt(refDateSeperate[1]);
                    var refmonth = parseInt(refDateSeperate[0]); // added +1 because javascript counts month from 0 

                    var year = parseInt(dateSeperate[1]);
                    var month = parseInt(dateSeperate[0]);
                    if (year != NaN && refyear != NaN && year <= refyear) {
                        if (year < refyear) {
                            return false;
                        } else if (month < refmonth) {
                            return false;
                        }
                    }
                } catch (e) {

                }

            } else {
                return false;
            }


        }
    }


    return true;
}

function SpecialFormatDateMustBePriorThanRefDate(input) {
    if (input.attr('data-val-specialformatdatemustbepriorthanrefdate') != null
        || input.attr('data-val-specialformatdateequalorgreaterthanrefdate') != null) {
        // make sure entered datas is validated format correctly at previous data attribute.
        var inputValue = input.val();
        var refpropertyName = input.attr('data-val-specialformatdatemustbepriorthanrefdate-refdateproperty');
        if (refpropertyName == null || refpropertyName == "") {
            return true;
        }
        var refProperty = $("input[id=" + refpropertyName + "]");
        if (refProperty == null || refProperty == undefined) {
            return true;
        }
        var refValue = refProperty.val();
        if (inputValue == null || inputValue == "") {
            return true;
        }

        if (refValue == null || refValue == "") {
            return true;
        }
        var dateformat = input.attr('data-val-specialformatdatemustbepriorthanrefdate-dateformat');
        if (dateformat != null && dateformat.length != 0
            && dateformat != "" && dateformat.length == inputValue.length) {
            if (dateformat == "MM/YY") {
                try {
                    var dateSeperate = inputValue.split("/");
                    var refDateSeperate = refValue.split("/");

                    var refyear = parseInt(refDateSeperate[1]);
                    var refmonth = parseInt(refDateSeperate[0]); // added +1 because javascript counts month from 0 

                    var year = parseInt(dateSeperate[1]);
                    var month = parseInt(dateSeperate[0]);
                    if (year != NaN && refyear != NaN && year >= refyear) {
                        if (year > refyear) {
                            return false;
                        } else if (month > refmonth) {
                            return false;
                        }
                    }
                } catch (e) {

                }

            } else {
                return false;
            }


        }
    }


    return true;
}

function DateMustBeEqualOrGreaterThanCurrentDate(input) {
    var inputValue = input.val();
    if (inputValue == null || inputValue == undefined || inputValue == "") {
        return true;
    }

    inputValue = inputValue.toString('MM/dd/yyyy');
    var today;
    var currentDate = new Date();
    var year = currentDate.getFullYear();
    var month = currentDate.getMonth() + 1; // added +1 because javascript counts month from 0 
    var day = currentDate.getDate();

    if (day < 10) {
        day = '0' + day;
    }
    if (month < 10) {
        month = '0' + month;
    }
    today = month + '/' + day + '/' + year;

    inputValue = new Date(inputValue);
    today = new Date(today);

    if (inputValue - today < 0) {
        return false;
    }

    return true;
}

function DateMustBeEqualOrLessThanCurrentDate(input) {
    var inputValue = input.val();
    if (inputValue == null || inputValue == "") {
        return true;
    }

    inputValue = inputValue.toString('MM/dd/yyyy');
    var today;
    var currentDate = new Date();
    var year = currentDate.getFullYear();
    var month = currentDate.getMonth() + 1; // added +1 because javascript counts month from 0 
    var day = currentDate.getDate();

    if (day < 10) {
        day = '0' + day;
    }
    if (month < 10) {
        month = '0' + month;
    }
    today = month + '/' + day + '/' + year;

    inputValue = new Date(inputValue);
    today = new Date(today);

    if (inputValue - today > 0) {
        return false;
    }

    return true;
}

function RoundNumber(text, decimal) {
    return Math.round(parseFloat(text) * Math.pow(10, decimal)) / Math.pow(10, decimal);
}

function HideErrorMsgUniquespecialflag(inputs) {
    var inputsval = [];
    for (var i = 0; i < inputs.length; i++) {
        var _input = $("input[id='" + inputs[i] + "']");
        if (_input.val().length > 0) {
            inputsval.push($("input[id='" + inputs[i] + "']").val());
        }
    }
    if (inputsval.length == inputsval.unique().length) {
        for (var i = 0; i < inputs.length; i++) {
            $("span[data-valmsg-for='" + inputs[i] + "']").hide();
        }
    }
}

function DetectNumber(val) {
    var arrayNumber = val.split(".");
    var leftHandPattern = "^(0|[-]?[1-9][0-9]*)$";
    var rightHandPattern = "^[0-9]+$";
    var regexLeftHand = new RegExp(leftHandPattern);
    var regexRightHand = new RegExp(rightHandPattern);
    if (arrayNumber.length > 2)
        return false;
    if (arrayNumber.length == 1) {
        return regexLeftHand.test(val);
    }
    else {
        if (arrayNumber[0] == "")
            return regexRightHand.test(arrayNumber[1]); //Regex.IsMatch(arrayNumber[1], rightHandPattern);
        else if (arrayNumber[1] == "")
            return false;
        else {
            return regexLeftHand.test(arrayNumber[0].replace("-", "")) && regexRightHand.test(arrayNumber[1]); //Regex.IsMatch(arrayNumber[0].Replace("-", ""), leftHandPattern) &&
            //Regex.IsMatch(arrayNumber[1], rightHandPattern);
        }
    }
}



$(window).load(function () {
    // AsFormatAttribute:
    $("input[data-val-asformat-mask]").each(function (i, e) {
        var mask = $(e).attr("data-val-asformat-mask");
        $(e).mask(mask);
    });

    $("input[data-val-asformat-type='Time'],input[data-val-asformat-type='Date']").each(function (i, e) {
        $(e).attr("data-val-date", $(e).attr("data-val-asformat"));
    });
});
