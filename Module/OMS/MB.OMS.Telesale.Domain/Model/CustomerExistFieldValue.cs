using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class CustomerExistFieldValue
    {
        public int CustomerFieldID { get; set; }
        public Guid CustomerExistID { get; set; }
        public string FieldValue { get; set; }
    }
}
