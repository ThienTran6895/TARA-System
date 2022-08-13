using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class PermisionsDTO : Permisions
    {
        public PermisionsDTO()
        {
            AvailablePermisions = new List<Permisions>();
        }
        public List<Permisions> AvailablePermisions { get; set; }
    }
}
