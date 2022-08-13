using MB.Web.Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class Customer
    {
        [DisplayName("Khách hàng")]
        public Guid CustomerID { get; set; }

        [DisplayName("Nguồn dữ liệu")]
        [GreaterThanAttribute(MinValue = "0", ErrorMessage = "Vui lòng chọn {0}")]
        public int SourceID { get; set; }

        [DisplayName("Điện thoại di động ")]
        [RequiredAttribute(ErrorMessage = "{0} không được để trống")]
        [RegExpressionAttribute(@"^[0-9]{0,500}$", ErrorMessage = "{0} chỉ được nhập số")]
        [MaxLengthAttribute(15, ErrorMessage = "{0} không quá 15 kí tự")]
        public string MobilePhone { get; set; }     

        [DisplayName("Ngày tạo ")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Người tạo ")]
        public Guid CreatedBy { get; set; }

        [DisplayName("Ngày cập nhật ")]
        public DateTime UpdatedDate { get; set; }

        [DisplayName("Người cập nhật ")]
        public Guid UpdatedBy { get; set; }

        [DisplayName("Hiển thị ")]
        public bool Visiable { get; set; }

        public bool IsDeleted { get; set; }

        public int Total { get; set; }
    }
}
