using MB.Web.Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class CustomerExist
    {
        public Guid CustomerExistID { get; set; }

        [DisplayName("Nguồn ")]
        [GreaterThanAttribute(MinValue = "0", ErrorMessage = "Vui lòng chọn {0}")]
        public int SourceID { get; set; }
        public Guid CustomerID { get; set; }

        [DisplayName("Dự án ")]
        public int ProjectID { get; set; }

        [DisplayName("Điện thoại di động ")]
        public string Phone { get; set; }
        public int RowExist { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public bool Visiable { get; set; }
        public bool IsDeleted { get; set; }

        public int Total { get; set; }
    }
}
