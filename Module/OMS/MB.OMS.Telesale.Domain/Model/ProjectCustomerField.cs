using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class ProjectCustomerField
    {
        public int ProjectID { get; set; }
        public int CustomerFieldID { get; set; }

        [DisplayName("Ngày tạo: ")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Người tạo: ")]
        public Guid CreateBy { get; set; }
        public bool IsActive { get; set; }
        public bool IsEdit { get; set; }

        public int Total { get; set; }
      
    }
}
