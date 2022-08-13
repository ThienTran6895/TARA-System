using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
    }
}
