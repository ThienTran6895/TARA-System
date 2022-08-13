using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface ICallRepository
    {
        int AddNewCall(Call call);

        int UpdateCall(Call call);

        Call GetCallByID(int id);

        DataSourceResult GetAllCallSuccess(DataSourceRequest dsRequest, int projectID, Guid? DTV = null);       

        DataSourceResult GetAllCallNotSuccess(DataSourceRequest dsRequest, int projectID, int StatusCallID, string MobilePhone, Guid? DTV = null);

        DataSourceResult GetAllCallNotSuccessByDTVandMobilePhone(DataSourceRequest dsRequest, int projectID, string MobilePhone, Guid? DTV = null);


        DataSourceResult GetAllCallNumberFail(DataSourceRequest dsRequest, int projectID, Guid? DTV = null);

        DataSourceResult GetAllCallReCall(DataSourceRequest dsRequest, int projectID, Guid? DTV = null);

        DataSourceResult GetAllCallPotential(DataSourceRequest dsRequest, int projectID, Guid? DTV = null);

        int DeleteCallByCustomer(int projectID, Guid customerID);

        int DeleteCallByCustomerNew(Guid customerID, int? projectID = null);

        IEnumerable<Call> GetCallByProjectAndCustomer(int projectId, Guid customerId);
    }
}
