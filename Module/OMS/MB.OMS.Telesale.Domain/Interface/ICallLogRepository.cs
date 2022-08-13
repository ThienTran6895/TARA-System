using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface ICallLogRepository
    {
        int AddNewCallLog(CallLog callLog);
        IEnumerable<CallLog> GetAllReCall(Guid userID, int projectID);
        int DeleteCallLogByCustomer(int projectID, Guid customerID);
        int DeleteCallLogByCustomerNew(Guid customerID, int? projectID = null);
        CallLog GetCallLogByID(int id);
        DataSourceResult GetAllCallHistoryDatasource(DataSourceRequest dsRequest, int projectID, int? callID = null, Guid? customerID = null);
        IEnumerable<CallLog> GetAllCallHistory(int projectID, int? callID = null, int? customerID = null);
        DataSourceResult GetCustomerByUserDatasource(DataSourceRequest dsRequest, int projectID, Guid callBy);
        IEnumerable<CallLog> GetCallLogByCustomerID(Guid customerID);

        int UpdateCallLog(CallLog callLog);
    }
}
