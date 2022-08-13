using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IProjectUsersRepository
    {
        IEnumerable<ProjectUsers> GetAll(int? projectID = null, Guid? usersId = null);
        int AddNewProjectUsers(ProjectUsers projectUsers);
        int DeleteProjectUsers(int projectID, Guid usersId);
    }
}
