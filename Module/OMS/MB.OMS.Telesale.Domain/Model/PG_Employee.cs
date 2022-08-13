using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class PG_Employee
    {
        [DisplayName("Họ tên nhân viên")]
        public string FullName { get; set; }

        [DisplayName("Mã cửa hàng")]
        public string Code { get; set; }

        [DisplayName("Tên cửa hàng")]
        public string StoreName { get; set; }

        [DisplayName("Tên QLTT")]
        public string ManagerName { get; set; }

        public string SourceName { get; set; }
    }
}
