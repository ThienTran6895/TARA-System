using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class CustomerFieldValue
    {
        public int CustomerFieldID { get; set; }
        public Guid CustomerID { get; set; }
        public string FieldValue { get; set; }
    }
    public class CustomerValue
    {
        public string CustomerID { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }
}
