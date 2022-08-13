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
    public class CallLogRepository : ICallLogRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.CallLogRepository");
        public int AddNewCallLog(CallLog callLog)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CallID", callLog.CallID, DbType.Int32);
                param.Add("@ProjectID", callLog.ProjectID, DbType.Int32);
                param.Add("@UserIds", callLog.UserId, DbType.Guid);
                param.Add("@CustomerID", callLog.CustomerID, DbType.Guid);
                param.Add("@StatusCallID", callLog.StatusCallID, DbType.Int32);
                param.Add("@RecallDate", callLog.RecallDate, DbType.DateTime);
                param.Add("@Note", callLog.Note, DbType.String);
                param.Add("@IsSuccess", callLog.IsSuccess, DbType.Boolean);
                // sondt 2019 - BO SUNG CHO AUTO CALL
                param.Add("@AgentCode", callLog.AgentCode, DbType.String);
                param.Add("@CisId", callLog.CisId, DbType.Int64);
                var r = DALHelpers.Query<int>("telesales_AddNewCallLog @CallID,@ProjectID,@UserIds,@CustomerID,@StatusCallID,@RecallDate,@Note,@IsSuccess,@AgentCode,@CisId", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewCallLog, Detail: "+ex.ToString());
                return 0;
            }
        }

        public IEnumerable<CallLog> GetAllReCall(Guid userID, int projectID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@DTV", userID, DbType.Guid);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                var r = DALHelpers.Query<CallLog>("telesales_GetAllReCallByDTV @ProjectID,@DTV,@PageSize,@Page", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllReCall, Detail: " + ex.ToString());
                return null;
            }
        }
        public int DeleteCallLogByCustomer(int projectID, Guid customerID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCallLogByCustomer", new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error DeleteCallLogByCustomer, Detail: " + ex.ToString());
                return 0;
            }
        }
        public int DeleteCallLogByCustomerNew(Guid customerID, int? projectID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCallLogByCustomerNew", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteCallLogByCustomerNew, Detail: " + ex.ToString());
                return 0;
            }
        }

        public CallLog GetCallLogByID(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Int32);
                var r = DALHelpers.Query<CallLog>("telesales_GetCallLogById @Id", new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetCallLogByID, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetAllCallHistoryDatasource(DataSourceRequest dsRequest, int projectID, int? callID = null, Guid? customerID = null)
        {
            logger.InfoFormat("GetAllCallHistoryDatasource {0},{1}", callID.HasValue ? callID.Value.ToString() : "", customerID.ToString());
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@CallID", callID, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<CallLog>("telesales_GetAllCallHistory @ProjectID,@CallID,@CustomerID,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch(Exception ex)
            {
                logger.Fatal("Error GetAllCallHistoryDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public IEnumerable<CallLog> GetAllCallHistory(int projectID, int? callID = null, int? customerID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@CallID", callID, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                var r = DALHelpers.Query<CallLog>("telesales_GetAllCallHistory @ProjectID,@CallID,@CustomerID,@PageSize,@Page", new UserLogin(), param).ToList();
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error GetAllCallHistory, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<CallLog> GetCallLogByCustomerID(Guid customerID )
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerID", customerID, DbType.Guid);

                var r = DALHelpers.Query<CallLog>("telesales_GetCallLogByCustomerID @CustomerID", new UserLogin(), param).ToList();
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error GetCallLogByCustomerID, Detail: " + ex.ToString());
                return null;
            }
        }
        //Lấy danh sách khách hàng của 1 ĐTV chưa được khảo sát hoặc gọi lại
        public DataSourceResult GetCustomerByUserDatasource(DataSourceRequest dsRequest, int projectID, Guid callBy)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@CallBy", callBy, DbType.Guid);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<CallLog>("telesales_GetAllNotCallAndReCall @ProjectID,@CallBy,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch(Exception ex)
            {
                logger.Fatal("Error GetCustomerByUserDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public int UpdateCallLog(CallLog callLog)
        {
            logger.Info("UpdateCallLog");
            try
            {
                var param = new DynamicParameters();
                param.Add("@CallLogID", callLog.CallLogID, DbType.Int32);
                param.Add("@StatusCallID", callLog.StatusCallID, DbType.Int32);
                param.Add("@RecallDate", callLog.RecallDate, DbType.DateTime);
                param.Add("@Note", callLog.Note, DbType.String);
                param.Add("@IsSuccess", callLog.IsSuccess, DbType.Boolean);
                var r = DALHelpers.Query<int>("telesales_UpdateCallLog @CallLogID,@StatusCallID,@RecallDate,@Note,@IsSuccess", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch(Exception ex)
            {
                logger.FatalFormat("Error UpdateCallLog, {0}",ex.ToString());
                throw ex;

            }
        }
    }
}
