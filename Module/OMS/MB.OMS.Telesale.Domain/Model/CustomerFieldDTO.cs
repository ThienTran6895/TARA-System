using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class CustomerFieldDTO : CustomerField
    {
        public string FieldValue { get; set; }

        public string ProjectName { get; set; }
    }
}
