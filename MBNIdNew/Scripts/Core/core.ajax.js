// implement helpers for core js

//extend with helper namespace to provice privite variable and method
golbal.extendns(core, 'ajax');
(function ()
{
    // deleare privite variable and method for core
}).apply(core.ajax);

// extend with helper objects
golbal.extend(core, {
    ajax: {
        //Post a data to server and response the JSON data.
        //If server handled request and data successful, the callback function will invoke with the response data from Server (JSON type).
        //The default Timeout for each request is 3s.
        //If Server was not able to handle request (timeout, abort, url not found...), the ALERT will show up with the correct error status and message.
        post: function (url, data, callback, isShowProcess)
        {
            $("input").val(function (_, value) {
                return $.trim(value);
            });

            isShowProcess = typeof isShowProcess == 'undefined' ? true : false;
            //validate input params.
            if (!url)
            {
                alert("Invalid Url");
                return false;
            }

            //post data to server.
            $.ajax({
                url: url,
                type: "POST",
                data: data,
                traditional: true,
                timeout: 300000,
                cache: false,
                beforeSend: function ()
                {
                    if (isShowProcess)
                        kendo.ui.progress($("body"), true);
                },
                success: function (response)
                {
                    if (callback)
                    {
                        callback(response);
                    }
                },
                complete: function (obj, status)
                {
                    kendo.ui.progress($("body"), false);
                }
            });
        },
        get: function (url, elementId, callback, isShowProcess)
        {
            isShowProcess = typeof isShowProcess == 'undefined' ? true : false;
            //validate input params.
            if (!url || !elementId)
            {
                alert('Please input URL and Element');
                return false;
            }
            //get data from server.
            $.ajax({
                url: url,
                type: "GET",
                timeout: 300000,
                cache: false,
                beforeSend: function ()
                {
                    if (isShowProcess)
                        kendo.ui.progress($("body"), true);
                },
                success: function (response)
                {
                    $('#' + elementId).html(response);

                    if (callback)
                    {
                        callback(response);
                    }
                },
                complete: function (obj, status)
                {
                    kendo.ui.progress($("body"), false);
                }
            })
        },
    }
});

$(document).ajaxError(function (event, xhr, settings, thrownError) {
    if (xhr.status == 403) {
        window.location.href = window.location.href;
    }
    else {
        alert("Invalid request." + thrownError);
    }
});