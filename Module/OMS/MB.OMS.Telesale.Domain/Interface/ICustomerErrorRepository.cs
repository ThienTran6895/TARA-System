using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface ICustomerErrorRepository
    {
        IEnumerable<CustomerError> GetAll(int? SourceID = null, int? ProjectID = null, string mobilePhone = null, bool? visiable = null, bool? isDeleted = null);
        DataSourceResult GetCustomerDatasource(DataSourceRequest dsRequest, int? sourceID = null, int? ProjectID = null, string mobilePhone = null, bool? visiable = null, bool? isDeleted = null);
        Guid AddNewCustomer(CustomerError customer);
        int UpdateCustomer(CustomerError customer);
        int DeleteCustomer(Guid id);
        CustomerError GetCustomer(Guid id);
        int DeleteCustomerErrorBySourceID(int SourceID);
        DataSourceResult GetAllFieldValueDatasource(DataSourceRequest dsRequest, int? projectID = null, int? sourceID = null, string phone = null, bool? visiable = null);

        IEnumerable<object> GetAllFieldValue(int? projectID = null, int? sourceID = null, string phone = null, bool? visiable = null);
    }
}
