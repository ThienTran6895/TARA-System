using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IProjectCustomerRepository
    {
        IEnumerable<ProjectCustomer> GetProjectCustomer(int? projectID = null, Guid? customerID = null);
        int AddNewProjectCustomer(ProjectCustomer projectCustomer);
        int UpdateProjectCustomer(ProjectCustomer projectCustomer);
        int DeleteProjectCustomer(int projectID, Guid customerID);
        int DeleteProjectCustomerNew(Guid customerID, int? projectID = null);

    }
}
