using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class User
    {
        public Guid Id { get; set; }

        [DisplayName("Tên đăng nhập ")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string UserName { get; set; }

        [DisplayName("Mật khẩu ")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Họ ")]
        public string LastName { get; set; }

        [DisplayName("Tên ")]
        public string FirstName { get; set; }

        [DisplayName("Giới tính ")]
        public bool Gender { get; set; }

        [DisplayName("Ngày sinh ")]
        public string BirthDay { get; set; }

        [DisplayName("Địa chỉ ")]
        public string Address { get; set; }

        [DisplayName("Email ")]
        public string Email { get; set; }

        [DisplayName("Điện thoại ")]
        public string Phone { get; set; }

        [DisplayName("Hình đại diện ")]
        public string ImageUrl { get; set; }

        [DisplayName("Ngày tạo ")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Người tạo ")]
        public Guid CreatedBy { get; set; }

        [DisplayName("Ngày cập nhật ")]
        public DateTime UpdatedDate { get; set; }

        [DisplayName("Người cập nhật ")]
        public Guid UpdatedBy { get; set; }

        public bool IsDelete { get; set; }

        [DisplayName("Kích hoạt ")]
        public bool Visible { get; set; }

        public int Total { get; set; }
    }
}
