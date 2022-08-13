using DAL.Core;
using Dapper;
using MB.Common;
using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Interface;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Repository
{
    public class StatusCallRepository : IStatusCallRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.StatusCallRepository");
        public IEnumerable<StatusCall> GetAll(string name = null, int? statusID = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@Name", name, DbType.String);
                param.Add("@StatusID", statusID, DbType.Int32);
                param.Add("@Visiable", visiable, DbType.Boolean);
                var r = DALHelpers.Query<StatusCall>("telesales_GetAllStatusCall @PageSize,@Page,@Name,@StatusID,@Visiable", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetStatusCallDatasource(DataSourceRequest dsRequest, string name = null, int? statusID = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@Name", name, DbType.String);
                param.Add("@StatusID", statusID, DbType.Int32);
                param.Add("@Visiable", visiable, DbType.Boolean);
                var r = DALHelpers.Query<StatusCall>("telesales_GetAllStatusCall @PageSize,@Page,@Name,@StatusID,@Visiable", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetStatusCallDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public int AddNewStatusCall(StatusCall statusCall)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Name", statusCall.Name, DbType.String);
                param.Add("@StatusID", statusCall.StatusID, DbType.Int32);
                param.Add("@Visiable", statusCall.Visiable, DbType.Boolean);
                var r = DALHelpers.Query<int>("telesales_AddNewStatusCall @Name,@StatusID,@Visiable", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewStatusCall, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int UpdateStatusCall(StatusCall statusCall)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@StatusCallID", statusCall.StatusCallID, DbType.Int32);
                param.Add("@Name", statusCall.Name, DbType.String);
                param.Add("@StatusID", statusCall.StatusID, DbType.Int32);
                param.Add("@Visiable", statusCall.Visiable, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateStatusCall", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error UpdateStatusCall, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteStatusCall(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@StatusCallID", id, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteStatusCall", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteStatusCall, Detail: " + ex.ToString());
                return 0;
            }
        }

        public StatusCallDTO GetStatusCall(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Int32);
                var r = DALHelpers.Query<StatusCallDTO>("telesales_GetStatusCallById @Id", new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetStatusCall, Detail: " + ex.ToString());
                return null;
            }
        }

        public IEnumerable<StatusCall> GetStatusCallByStatusId(int StatusId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@statusId", StatusId, DbType.Int32);
                var r = DALHelpers.Query<StatusCallDTO>("telesales_GetStatusCallByStatusId @statusId", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetStatusCall, Detail: " + ex.ToString());
                return null;
            }
        }

        public IEnumerable<Status> GetAllStatus()
        {
            try
            {
                var r = DALHelpers.Query<Status>("telesales_GetAllStatus", new UserLogin()).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllStatus, Detail: " + ex.ToString());
                return null;
            }
        }

        public IEnumerable<StatusCall> GetStatusCallByProject(int projectID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                var r = DALHelpers.Query<StatusCall>("telesales_GetAllStatusCallForProject @ProjectID", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetStatusCallByProject, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetStatusCallForProjectDatasource(DataSourceRequest dsRequest, int projectID, string name = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@Name", name, DbType.String);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<StatusCall>("telesales_GetAllStatusCallForProject @ProjectID,@Name,@PageSize,@Page", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetStatusCallForProjectDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetStatusCallNotInProjectDatasource(DataSourceRequest dsRequest, int projectID, string name = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@Name", name, DbType.String);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<StatusCall>("telesales_GetAllStatusCallNotInProject @ProjectID,@Name,@PageSize,@Page", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetStatusCallNotInProjectDatasource, Detail: " + ex.ToString());
                return null;
            }
        }
    }
}
