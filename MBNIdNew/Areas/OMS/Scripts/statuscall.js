var saveStatusCall = $.extend(saveStatusCall, {
    init: function () {
        $("#btnSave").on('click', saveStatusCall.saveOnClick);
    },

    saveOnClick: function (ev) {
        core.control.validatesave(ev);
    }
});

$("#btnDelete").click(function () {

    var title = "Xác nhận xóa!";
    var text = "Bạn có chắc chắn xóa không?";
    swal({
        title: title,
        text: text,
        type: "warning",
        showCancelButton: true,
        cancelButtonText: "Hủy",
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Xác nhận!",
        closeOnConfirm: false,
        showLoaderOnConfirm: true,
    }, function (isConfirm) {
        if (isConfirm) {
            $.ajax({
                url: "/OMS/StatusCall/DeleteStatusCall/",
                data: { id: $("#StatusID").val() },
                method: "POST",
                success: function (data) {
                    if (data.Result == "success") {
                        swal({
                            title: data.Title,
                            text: data.Message,
                            type: "success"
                        }, function () {
                            window.location.href = data.URLList;
                        });
                    }
                    else if (data.Result == "failed") {
                        swal({ title: data.Title, text: data.Message, type: "error" });
                    }
                }, error: function () {
                    swal({
                        title: "Không thể xóa!",
                        text: "Có lỗi phát sinh khi xóa, vui lòng thử lại.",
                        type: "error",
                        timer: 2000, showConfirmButton: false
                    });
                }
            })
        }
    });
});

$(document).ready(function () {
    saveStatusCall.init();
});