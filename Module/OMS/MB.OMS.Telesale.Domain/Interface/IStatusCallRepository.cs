using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IStatusCallRepository
    {
        IEnumerable<StatusCall> GetAll(string name = null, int? statusID = null, bool? visiable = null);
        DataSourceResult GetStatusCallDatasource(DataSourceRequest dsRequest, string name = null, int? statusID = null, bool? visiable = null);
        int AddNewStatusCall(StatusCall statusCall);
        int UpdateStatusCall(StatusCall statusCall);
        int DeleteStatusCall(int id);
        StatusCallDTO GetStatusCall(int id);
        IEnumerable<Status> GetAllStatus();
        // sondt - 2019
        IEnumerable<StatusCall> GetStatusCallByStatusId(int StatusId);
        IEnumerable<StatusCall> GetStatusCallByProject(int projectID);
        DataSourceResult GetStatusCallForProjectDatasource(DataSourceRequest dsRequest, int projectID, string name = null);
        DataSourceResult GetStatusCallNotInProjectDatasource(DataSourceRequest dsRequest, int projectID, string name = null);
    }
}
