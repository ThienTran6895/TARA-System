using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IUserRoleRepository
    {
        IEnumerable<UserRole> GetAll(Guid? userIds = null, int? roleId = null);
        int AddNewUserRole(UserRole userRole);
        int DeleteUserRole(Guid userIds, int roleId);
    }
}
