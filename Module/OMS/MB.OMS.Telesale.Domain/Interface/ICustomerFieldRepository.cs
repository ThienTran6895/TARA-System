using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface ICustomerFieldRepository
    {
        IEnumerable<CustomerField> GetAll(string fieldcode = null, string fieldname = null);
        DataSourceResult GetCustomerFieldDatasource(DataSourceRequest dsRequest, string fieldcode = null, string fieldname = null);
        int AddNewCustomerField(CustomerField customerField);
        int UpdateCustomerField(CustomerField customerField);
        int DeleteCustomerField(int id);
        CustomerFieldDTO GetCustomerField(int id);
        IEnumerable<CustomerField> GetCustomerFieldByProject(int projectID);
        IEnumerable<CustomerFieldDTO> GetCustomerFieldAndValueByCustomerID(Guid customerID, int projectID);
        IEnumerable<CustomerFieldDTO> GetCustomerFieldAndValueByMobilePhone(string phone, Guid customerId, int projectID);
        IEnumerable<CustomerFieldDTO> GetCustomerFieldAndValueByCustomerExistID(Guid customerExistID, int projectID);
        DataSourceResult GetCustomerFieldNotInForProjectDatasource(DataSourceRequest dsRequest, int projectID, string fieldname = null);
        DataSourceResult GetCustomerFieldForProjectDatasource(DataSourceRequest dsRequest, int projectID, string fieldname = null);
    }
}
