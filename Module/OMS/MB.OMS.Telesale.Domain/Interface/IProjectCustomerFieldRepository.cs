using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IProjectCustomerFieldRepository
    {
        IEnumerable<ProjectCustomerField> GetAll(int? projectID = null, int? customerFieldID = null, bool? isActive = null, bool? isEdit = null);
        DataSourceResult GetProjectCustomerFieldDatasource(DataSourceRequest dsRequest, int? projectID = null, int? customerFieldID = null, bool? isActive = null, bool? isEdit = null);
        int AddNewProjectCustomerField(ProjectCustomerField projectCustomerField);
        int UpdateProjectCustomerField(ProjectCustomerField projectCustomerField);
        int DeleteProjectCustomerField(int? projectID = null, int? customerFieldID = null);
    }
}
