using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAll(int? sourceID = null, string mobilePhone = null, bool? visiable = null, bool? isDeleted = null);
        DataSourceResult GetCustomerDatasource(DataSourceRequest dsRequest, int? sourceID = null, string mobilePhone = null, bool? visiable = null, bool? isDeleted = null);
        Guid AddNewCustomer(Customer customer);
        int UpdateCustomer(Customer customer);
        int DeleteCustomer(Guid id);

        int DeleteCustomerNew(Guid id);
        CustomerDTO GetCustomer(Guid id);

        CallSurvey GetCustomerCallSurvey(Guid id);
        CallSurvey GetCustomerByMobilePhone(string mobilePhone);

        DataSourceResult GetAllCustomerFieldValueByProjectDatasource(DataSourceRequest dsRequest, int projectID, int? sourceID = null, Guid? callBy = null, string phone = null, bool? visiable = null);

        DataSourceResult GetAllFieldValueDatasource(DataSourceRequest dsRequest, int? projectID = null, int? sourceID = null, string phone = null, bool? visiable = null);
        DataSourceResult GetAllFieldValueDatasource(DataSourceRequest dsRequest, int? projectID = null, int? sourceID = null, string phone = null, string dateFrom = null, string dateEnd = null, bool ? visiable = null);

        DataSourceResult ProjectCustomerGetAllFieldValueDatasource(DataSourceRequest dsRequest, int projectID, int? sourceID = null, string phone = null, bool? visiable = null);

        DataSourceResult CustomerGetAllFieldValueNotInProjectDatasource(DataSourceRequest dsRequest, int projectID, int? sourceID = null, string phone = null, bool? visiable = null);

        DataSourceResult GetAllCustomerInProjectNotDivDTVDatasource(DataSourceRequest dsRequest, int projectID, int? sourceID = null, string phone = null, bool? visiable = null);

        IEnumerable<CustomerDTO> ProjectCustomerGetAllFieldValue(int projectID, int? sourceID = null, string phone = null, bool? visiable = null);

        IEnumerable<CustomerDTO> GetAllCustomerByIsCallNot(int projectID, int? sourceID = null, string phone = null, bool? visiable = null);

        IEnumerable<CustomerDTO> CustomerGetAllFieldValueNotInProject(int projectID, int? sourceID = null, string phone = null, bool? visiable = null);
        IEnumerable<object> GetAllFieldValue(int? projectID = null, int? sourceID = null, string phone = null, bool? visiable = null);
        IEnumerable<object> GetAllFieldValue(int? projectID = null, int? sourceID = null, string phone = null, string dateFrom = null, string dateEnd = null, bool? visiable = null);
        IEnumerable<CustomerDTO> GetAllCustomerBySourceID(int SourceID);
        IEnumerable<object> GetCustomerFieldValues(int? projectID = null, int? sourceID = null, string phone = null, string dateFrom = null, string dateEnd = null, bool? visiable = null);

        CallSurvey GetCustomerByMobilePhone(string mobilePhone, int sourceId);

        void UpdateProjectCustomer(Customer customer, int projectId, Guid callBy, bool isCall);
    }
}
