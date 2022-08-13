using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Domain.Model
{
    public class RoleDTO : Roles
    {
        public RoleDTO()
        {
            AvailablePermision = new List<PermisionsDTO>();
            AvailableAction = new List<Roles>();
        }
        public List<PermisionsDTO> AvailablePermision { get; set; }
        public List<Roles> AvailableAction { get; set; }
    }
}
