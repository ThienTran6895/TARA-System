using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Account.Domain.Model
{
    public class Permisions
    {
        public int Id { get; set; }
        public string PermisionName { get; set; }
        public int PermisionType { get; set; }
        public int? ActionType { get; set; }
        public int? Parent { get; set; }
        public bool IsVisible { get; set; }
    }
}
