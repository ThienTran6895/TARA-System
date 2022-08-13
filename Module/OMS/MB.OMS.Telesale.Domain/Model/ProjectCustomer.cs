using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class ProjectCustomer
    {
        public int ProjectID { get; set; }
        public Guid CustomerID { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid UpdatedBy { get; set; }
        public Guid CallBy { get; set; }
        public bool IsCall { get; set; }
    }
}
