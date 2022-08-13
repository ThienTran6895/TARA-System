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
    public class CallRepository : ICallRepository
    {

        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.CallRepository");
        public int AddNewCall(Call call)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", call.ProjectID, DbType.Int32);
                param.Add("@UserIds", call.UserId, DbType.Guid);
                param.Add("@CustomerID", call.CustomerID, DbType.Guid);
                param.Add("@StatusCallID", call.StatusCallID, DbType.Int32);
                param.Add("@IsSuccess", call.IsSuccess, DbType.Boolean);
                var r = DALHelpers.Query<int>("telesales_AddNewCall @ProjectID,@UserIds,@CustomerID,@StatusCallID,@IsSuccess", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error AddNewCall, Detail: " + ex.ToString());
                return 0;
            }
        }
        public int UpdateCall(Call call)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CallID", call.CallID, DbType.Int32);
                param.Add("@ProjectID", call.ProjectID, DbType.Int32);
                param.Add("@UserIds", call.UserId, DbType.Guid);
                param.Add("@CustomerID", call.CustomerID, DbType.Guid);
                param.Add("@StatusCallID", call.StatusCallID, DbType.Int32);
                param.Add("@IsSuccess", call.IsSuccess, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateCall", new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error UpdateCall, Detail: " + ex.ToString());
                return 0;
            }
        }
        public Call GetCallByID(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Int32);
                var r = DALHelpers.Query<Call>("telesales_GetCallById @Id", new UserLogin(), param).First();
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error GetCallByID, Detail: " + ex.ToString());
                return null;
            }
        }
        public DataSourceResult GetAllCallSuccessByDTVDatasource(DataSourceRequest dsRequest,int projectID, Guid? DTV = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@DTV", DTV, DbType.Guid);
                param.Add("@ProjectID", projectID, DbType.Int32);
                var r = DALHelpers.Query<Campaigns>("telesales_GetAllCallSuccessByDTV @DTV,@ProjectID,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch(Exception ex)
            {
                logger.Fatal("Error GetAllCallSuccessByDTVDatasource, Detail: " + ex.ToString());
                return null;
            }
        }
        public DataSourceResult GetAllCallSuccess(DataSourceRequest dsRequest, int projectID, Guid? DTV = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@DTV", DTV, DbType.Guid);
                var r = DALHelpers.Query<Call>("telesales_GetAllCallSuccessByDTV @ProjectID,@DTV,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch(Exception ex)
            {
                logger.Fatal("Error GetAllCallSuccess, Detail: " + ex.ToString());
                return null;
            }
        }      

        public DataSourceResult GetAllCallNotSuccess(DataSourceRequest dsRequest, int projectID,int StatusCallID, string MobilePhone, Guid? DTV = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@DTV", DTV, DbType.Guid);
                param.Add("@StatusCallID", StatusCallID, DbType.Int32);
                param.Add("@MobilePhone", MobilePhone, DbType.String);
                var r = DALHelpers.Query<Call>("spa_telesales_GetAllCallNotSuccessByDTV @ProjectID,@DTV,@StatusCallID,@MobilePhone,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllCallNotSuccess, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetAllCallNotSuccessByDTVandMobilePhone(DataSourceRequest dsRequest, int projectID, string MobilePhone, Guid? DTV = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@DTV", DTV, DbType.Guid);
                param.Add("@MobilePhone", MobilePhone, DbType.String);
                var r = DALHelpers.Query<Call>("telesales_GetAllCallNotSuccessByDTVandMobilePhone @ProjectID,@DTV,@MobilePhone,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllCallNotSuccess, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetAllCallNumberFail(DataSourceRequest dsRequest, int projectID, Guid? DTV = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@DTV", DTV, DbType.Guid);
                var r = DALHelpers.Query<Call>("telesales_GetAllCallNumberFailByDTV @ProjectID,@DTV,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch(Exception ex)
            {
                logger.Fatal("Error GetAllCallNumberFail, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetAllCallReCall(DataSourceRequest dsRequest, int projectID, Guid? DTV = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@DTV", DTV, DbType.Guid);
                var r = DALHelpers.Query<Call>("telesales_GetAllCallReCallByDTV @ProjectID,@DTV,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllCallReCall, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetAllCallPotential(DataSourceRequest dsRequest, int projectID, Guid? DTV = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@DTV", DTV, DbType.Guid);
                var r = DALHelpers.Query<Call>("telesales_GetAllCallPotentialByDTV @ProjectID,@DTV,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllCallPotential, Detail: " + ex.ToString());
                return null;
            }
        }
        public int DeleteCallByCustomer(int projectID, Guid customerID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCallByCustomer", new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error DeleteCallByCustomer, Detail: " + ex.ToString());
                return 0;
            }
        }
        public int DeleteCallByCustomerNew(Guid customerID, int? projectID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCallByCustomerNew", new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error DeleteCallByCustomerNew, Detail: " + ex.ToString());
                return 0;
            }
        }

        public IEnumerable<Call> GetCallByProjectAndCustomer(int projectId, Guid customerId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectId, DbType.Int32);
                param.Add("@CustomerID", customerId, DbType.Guid);
                var r = DALHelpers.Query<Call>("telesales_GetCallByProjectIDAndCustomerID @ProjectID,@CustomerID", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCallByProjectAndCustomer, Detail:" + ex.ToString());
                return null;
            }
        }
    }
}
