using MB.Web.Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class CustomerField 
    {
        public int CustomerFieldID { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [DisplayName("Mã thuộc tính ")]
        public string FieldCode { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [DisplayName("Tên thuộc tính ")]
        public string FieldName { get; set; }

        public int? DataTypeID { get; set; }

        public int? ControlTypeID { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public Guid UpdatedBy { get; set; }

        [DisplayName("Thứ tự hiển thị ")]
        public int Order { get; set; }

        public int Total { get; set; }
        
    }
}
