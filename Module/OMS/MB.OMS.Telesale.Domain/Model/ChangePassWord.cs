using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MB.OMS.Telesale.Domain.Model
{
    public class ChangePassWord
    {
        [DisplayName("Mật khẩu cũ ")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [DataType(DataType.Password)]
        public string OldPassWord { get; set; }

        [DisplayName("Mật khẩu mới ")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [DataType(DataType.Password)]
        public string NewPassWord { get; set; }

        [DisplayName("Nhập lại mật khẩu mới")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [DataType(DataType.Password)]
        public string ReplayPassWord { get; set; }
    }
}
