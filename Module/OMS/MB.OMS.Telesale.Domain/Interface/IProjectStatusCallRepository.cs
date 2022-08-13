using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IProjectStatusCallRepository
    {
        IEnumerable<ProjectStatusCall> GetAll(int? projectID = null, int? statusID = null);
        int AddNewProjectStatusCall(ProjectStatusCall projectStatusCall);
        int DeleteProjectStatusCall(int projectID, int statusID);
    }
}
