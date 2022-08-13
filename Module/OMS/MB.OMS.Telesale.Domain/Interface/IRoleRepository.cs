using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IRoleRepository
    {
        IEnumerable<Roles> GetAll(string roleName = null, bool? isVisible = null);
        DataSourceResult GetRoleDatasource(DataSourceRequest dsRequest, string roleName = null, bool? isVisible = null);
        RoleDTO GetRoleById(int id);
        int AddNewRole(Roles role);
        int UpdateRole(Roles role);
        int DeleteRole(int id);
    }
}
