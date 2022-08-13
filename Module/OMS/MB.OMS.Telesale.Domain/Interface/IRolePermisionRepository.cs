using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IRolePermisionRepository
    {
        IEnumerable<RolePermision> GetAll(int? roleId = null, int? permisionId = null);
        int AddNewRolePermision(RolePermision rolePermision);
        int DeleteRolePermision(int roleId, int permisionId);
    }
}
