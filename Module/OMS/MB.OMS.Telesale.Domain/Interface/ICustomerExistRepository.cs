using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface ICustomerExistRepository
    {
        IEnumerable<CustomerExist> GetAll(int? sourceID = null, Guid? customerID = null, int? projectID = null, string phone = null, int? rowExist = null, bool? visiable = null, bool? isDeleted = null);

        DataSourceResult GetCustomerExistDatasource(DataSourceRequest dsRequest, int? sourceID = null, Guid? customerID = null, int? projectID = null, string phone = null, int? rowExist = null, bool? visiable = null, bool? isDeleted = null);

        Guid AddNewCustomerExist(CustomerExist customerExist);

        int UpdateCustomerExist(CustomerExist customerExist);

        int DeleteCustomerExist(Guid id);

        int DeleteCustomerExistBySourceID(int SourceID);

        CustomerExistDTO GetCustomerExist(Guid id);

        DataSourceResult GetAllFieldValueDatasource(DataSourceRequest dsRequest, int? projectID = null, int? sourceID = null, string phone = null, bool? visiable = null);

        IEnumerable<object> GetAllFieldValue(int? projectID = null, int? sourceID = null, string phone = null, bool? visiable = null);
    }
}
