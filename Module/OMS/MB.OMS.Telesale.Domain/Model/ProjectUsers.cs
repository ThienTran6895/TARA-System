using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class ProjectUsers
    {
        public int ProjectID { get; set; }

        public Guid UserId { get; set; }
        // sondt - 2019 -- bo sung cho AUTOCALL
        public string AgentCode { get; set; }
    }
}
