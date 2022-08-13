using DAL.Core;
using Dapper;
using MB.Common;
using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Interface;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MB.OMS.Telesale.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.CustomerRepository");
        public IEnumerable<Customer> GetAll(int? sourceID = null, string mobilePhone = null, bool? visiable = null, bool? isDeleted = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@MobilePhone", mobilePhone, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@IsDeleted", isDeleted, DbType.Boolean);
                var r = DALHelpers.Query<Customer>("telesales_GetAllCustomer @PageSize,@Page,@SourceID,@MobilePhone,@Visiable,@IsDeleted", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetAll, Detail:" + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetCustomerDatasource(DataSourceRequest dsRequest, int? sourceID = null, string mobilePhone = null, bool? visiable = null, bool? isDeleted = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", 1000, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@MobilePhone", mobilePhone, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@IsDeleted", isDeleted, DbType.Boolean);
                var r = DALHelpers.Query<Customer>("telesales_GetAllCustomer @PageSize,@Page,@SourceID,@MobilePhone,@Visiable,@IsDeleted", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerDatasource, Detail:" + ex.ToString());
                return null;
            }
        }

        public Guid AddNewCustomer(Customer customer)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerID", customer.CustomerID, DbType.Guid);
                param.Add("@SourceID", customer.SourceID, DbType.Int32);
                param.Add("@MobilePhone", customer.MobilePhone, DbType.String);
                param.Add("@Visiable", customer.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", customer.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Query<Guid>("telesales_AddNewCustomer @CustomerID,@SourceID,@MobilePhone,@Visiable,@IsDeleted", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: AddNewCustomer, Detail:" + ex.ToString());
                return Guid.Empty;
            }
        }

        public int UpdateCustomer(Customer customer)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerID", customer.CustomerID, DbType.Guid);
                param.Add("@SourceID", customer.SourceID, DbType.Int32);
                param.Add("@MobilePhone", customer.MobilePhone, DbType.String);
                param.Add("@Visiable", customer.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", customer.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateCustomer", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: UpdateCustomer, Detail:" + ex.ToString());
                return 0;
            }
        }

        public int DeleteCustomer(Guid id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerID", id, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCustomer",
                    new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: DeleteCustomer, Detail:" + ex.ToString());
                return 0;
            }
        }
        public int DeleteCustomerNew(Guid id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerID", id, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCustomerNew",
                    new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: DeleteCustomerNew, Detail:" + ex.ToString());
                return 0;
            }
        }
        public CustomerDTO GetCustomer(Guid id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Guid);
                var r = DALHelpers.Query<CustomerDTO>("telesales_GetCustomerById @Id",
                    new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomer, Detail:" + ex.ToString());
                return null;
            }
        }
        public CustomerDTO GetCustomerByCall(string phoneNumber, int projectId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@phoneNumber", phoneNumber, DbType.String);
                param.Add("@projectId", projectId, DbType.Int32);

                var r = DALHelpers.Query<CustomerDTO>("telesales_GetCustomerByCallSurvey @phoneNumber, @projectId",
                    new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: telesales_GetCustomerByCallSurvey, Detail:" + ex.ToString());
                return null;
            }
        }

        public CallSurvey GetCustomerCallSurvey(Guid id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Guid);
                var r = DALHelpers.Query<CallSurvey>("telesales_GetCustomerById @Id",
                    new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerCallSurvey, Detail:" + ex.ToString());
                return null;
            }
        }

        public CallSurvey GetCustomerByMobilePhone(string mobilePhone)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@MobilePhone", mobilePhone, DbType.String);
                var r = DALHelpers.Query<CallSurvey>("telesales_GetCustomerByMobilePhone @MobilePhone",
                    new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerByMobilePhone, Detail:" + ex.ToString());
                return null;
            }
        }

        public CallSurvey GetCustomerByMobilePhone(string mobilePhone, int sourceId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@MobilePhone", mobilePhone, DbType.String);
                param.Add("@sourceId", sourceId, DbType.Int32);

                var r = DALHelpers.Query<CallSurvey>("telesales_GetCustomerByMobilePhone @MobilePhone,@sourceId",
                    new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerByMobilePhone, Detail:" + ex.ToString());
                return null;
            }
        }

        public CallSurvey GetCustomerByCallId(Int64 callId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@callId", callId, DbType.Int64);

                var r = DALHelpers.Query<CallSurvey>("telesales_GetCustomerByCallId @MobilePhone,@sourceId",
                    new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerByMobilePhone, Detail:" + ex.ToString());
                return null;
            }
        }




        public DataSourceResult GetAllCustomerFieldValueByProjectDatasource(DataSourceRequest dsRequest, int projectID, int? sourceID = null, Guid? callBy = null, string phone = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@CallBy", callBy, DbType.Guid);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<object>("telesales_ProjectCustomerGetAllFieldValueForName @ProjectID,@SourceID,@CallBy,@Phone,@Visiable,@PageSize,@Page", new UserLogin(), param).AsEnumerable();
                int total = 0;
                if (r.Count() != 0)
                    foreach (IDictionary<string, object> rp in r)
                    {
                        total = Convert.ToInt32(rp.FirstOrDefault(x => x.Key == "Total").Value);
                        break;
                    }

                return new DataSourceResult() { Data = r, Total = total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllCustomerFieldValueByProjectDatasource, Detail: " + ex.ToString());
                return new DataSourceResult() { Data = null, Total = 0 };
            }
        }

        /// <summary>
        /// Lấy danh sách Customer và CustomerFieldValue
        /// </summary>
        /// <param name="dsRequest"></param>
        /// <param name="projectID"></param>
        /// <param name="sourceID"></param>
        /// <param name="phone"></param>
        /// <param name="visiable"></param>
        /// <returns></returns>
        public DataSourceResult GetAllFieldValueDatasource(DataSourceRequest dsRequest, int? projectID = null, int? sourceID = null, string phone = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<object>("telesales_CustomerGetAllFieldValue @ProjectID,@SourceID,@Phone,@Visiable,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                int total = 0;
                if (r.Count() != 0)
                    foreach (IDictionary<string, object> rp in r)
                    {
                        total = Convert.ToInt32(rp.FirstOrDefault(x => x.Key == "Total").Value);
                        break;
                    }

                return new DataSourceResult() { Data = r, Total = total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllFieldValueDatasource, Detail: " + ex.ToString());
                return null;
            }
        }
        /// <summary>
        /// Lấy danh sách Customer và CustomerFieldValue
        /// </summary>
        /// <param name="dsRequest"></param>
        /// <param name="projectID"></param>
        /// <param name="sourceID"></param>
        /// <param name="phone"></param>
        /// <param name="visiable"></param>
        /// <returns></returns>
        public DataSourceResult GetAllFieldValueDatasource(DataSourceRequest dsRequest, int? projectID = null, int? sourceID = null, string phone = null, string dateFrom = null, string dateEnd = null, bool ? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@DateFrom", dateFrom, DbType.DateTime);
                param.Add("@DateEnd", dateEnd, DbType.DateTime);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<object>("telesales_CustomerGetAllFieldValue @ProjectID,@SourceID,@Phone,@DateFrom,@DateEnd,@Visiable,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                int total = 0;
                if (r.Count() != 0)
                    foreach (IDictionary<string, object> rp in r)
                    {
                        total = Convert.ToInt32(rp.FirstOrDefault(x => x.Key == "Total").Value);
                        break;
                    }

                return new DataSourceResult() { Data = r, Total = total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllFieldValueDatasource, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<object> GetAllFieldValue(int? projectID = null, int? sourceID = null, string phone = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                var r = DALHelpers.Query<object>("telesales_CustomerGetAllFieldValue @ProjectID,@SourceID,@Phone,@Visiable,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllFieldValue, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<object> GetAllFieldValue(int? projectID = null, int? sourceID = null, string phone = null, string dateFrom = null, string dateEnd = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@DateFrom", dateFrom, DbType.DateTime);
                param.Add("@DateEnd", dateEnd, DbType.DateTime);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                var r = DALHelpers.Query<object>("telesales_CustomerGetAllFieldValue @ProjectID,@SourceID,@Phone,@DateFrom,@DateEnd,@Visiable,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllFieldValue, Detail: " + ex.ToString());
                return null;
            }
        }

        public IEnumerable<object> GetCustomerFieldValues(int? projectID = null, int? sourceID = null, string phone = null, string dateFrom = null, string dateEnd = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@DateFrom", dateFrom, DbType.DateTime);
                param.Add("@DateEnd", dateEnd, DbType.DateTime);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                var r = DALHelpers.Query<object>("spa_telesales_CustomerFieldValue_GetAll @ProjectID,@SourceID,@Phone,@DateFrom,@DateEnd,@Visiable,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllFieldValue, Detail: " + ex.ToString());
                return null;
            }
        }


        /// <summary>
        /// Lấy danh sách Customer và CustomerFieldValue theo Project
        /// </summary>
        /// <param name="dsRequest"></param>
        /// <param name="projectID"></param>
        /// <param name="sourceID"></param>
        /// <param name="phone"></param>
        /// <param name="visiable"></param>
        /// <returns></returns>
        public DataSourceResult ProjectCustomerGetAllFieldValueDatasource(DataSourceRequest dsRequest, int projectID, int? sourceID = null, string phone = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<object>("telesales_ProjectCustomerGetAllFieldValue @ProjectID,@SourceID,@Phone,@Visiable,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                int total = 0;
                if (r.Count() != 0)
                    foreach (IDictionary<string, object> rp in r)
                    {
                        total = Convert.ToInt32(rp.FirstOrDefault(x => x.Key == "Total").Value);
                        break;
                    }

                return new DataSourceResult() { Data = r, Total = total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ProjectCustomerGetAllFieldValueDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Lấy danh sách Customer và CustomerFieldValue chưa được phân công cho Project nào
        /// </summary>
        /// <param name="dsRequest"></param>
        /// <param name="projectID"></param>
        /// <param name="sourceID"></param>
        /// <param name="phone"></param>
        /// <param name="visiable"></param>
        /// <returns></returns>
        public DataSourceResult CustomerGetAllFieldValueNotInProjectDatasource(DataSourceRequest dsRequest, int projectID, int? sourceID = null, string phone = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<object>("telesales_CustomerGetAllFieldValueNotInProject @SourceID,@ProjectID,@Phone,@Visiable,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                int total = 0;
                if (r.Count() != 0)
                    foreach (IDictionary<string, object> rp in r)
                    {
                        total = Convert.ToInt32(rp.FirstOrDefault(x => x.Key == "Total").Value);
                        break;
                    }

                return new DataSourceResult() { Data = r, Total = total };
            }
            catch (Exception ex)
            {
                string tmp = "Page:" + dsRequest.Page + "|PageSize:" + dsRequest.PageSize + "|projectID:" + projectID + "|sourceID:" + sourceID + "|phone:" + phone + "|visiable:"+ visiable;
                logger.Info(tmp);
                logger.Fatal("Error CustomerGetAllFieldValueNotInProjectDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetAllCustomerInProjectNotDivDTVDatasource(DataSourceRequest dsRequest, int projectID, int? sourceID = null, string phone = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<CustomerDTO>("telesales_GetAllCustomerInProjectNotDivDTV @ProjectID,@SourceID,@Phone,@Visiable,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllCustomerInProjectNotDivDTVDatasource, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<CustomerDTO> CustomerGetAllFieldValueNotInProject(int projectID, int? sourceID = null, string phone = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                var r = DALHelpers.Query<CustomerDTO>("telesales_CustomerGetAllFieldValueNotInProject @SourceID,@ProjectID,@Phone,@Visiable", new UserLogin(), param).AsEnumerable();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error CustomerGetAllFieldValueNotInProject, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<CustomerDTO> ProjectCustomerGetAllFieldValue(int projectID, int? sourceID = null, string phone = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                var r = DALHelpers.Query<CustomerDTO>("telesales_ProjectCustomerGetAllFieldValue @ProjectID,@SourceID,@Phone,@Visiable,@PageSize,@Page", new UserLogin(), param).ToList();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ProjectCustomerGetAllFieldValue, Detail: " + ex.ToString());
                return null;
            }
        }

        public IEnumerable<CustomerDTO> GetAllCustomerByIsCallNot(int projectID, int? sourceID = null, string phone = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                var r = DALHelpers.Query<CustomerDTO>("telesales_GetAllCustomerByIsCallNot @ProjectID,@SourceID,@Phone,@Visiable,@PageSize,@Page", new UserLogin(), param).ToList();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllCustomerByIsCallNot, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<CustomerDTO> GetAllCustomerBySourceID(int SourceID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@SourceID", SourceID, DbType.Int32);
                var r = DALHelpers.Query<CustomerDTO>("telesales_GetAllCustomerBySourceID @SourceID", new UserLogin(), param).ToList();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllCustomerBySourceID, Detail: " + ex.ToString());
                return null;
            }
        }

        #region sondt - for AUTOCALL
        public void UpdateProjectCustomer( Customer customer, int projectId, Guid callBy, bool isCall)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectId, DbType.Int32);
                param.Add("@CustomerID", customer.CustomerID, DbType.Guid);
                param.Add("@CallBy", callBy, DbType.Guid);
                param.Add("@IsCall", isCall, DbType.Boolean);
                DALHelpers.Execute("spa_telesales_ProjectCustomer_UpdateCall", new UserLogin(), param);
            }
            catch (Exception ex)
            {
                logger.Fatal("Error UpdateProjectCustomer, Detail: " + ex.ToString());
                throw ex;
            }
        }
        #endregion
    }
}
