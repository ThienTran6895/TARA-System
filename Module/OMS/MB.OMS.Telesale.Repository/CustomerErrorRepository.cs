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
    public class CustomerErrorRepository : ICustomerErrorRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.CustomerErrorRepository");
        public IEnumerable<CustomerError> GetAll(int? SourceID = null, int? ProjectID = null, string mobilePhone = null, bool? visiable = null, bool? isDeleted = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerErrorID", null, DbType.Guid);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@SourceID", SourceID, DbType.Int32);
                param.Add("@Project", ProjectID, DbType.Int32);
                param.Add("@MobilePhone", mobilePhone, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@IsDeleted", isDeleted, DbType.Boolean);
                var r = DALHelpers.Query<CustomerError>("telesales_GetAllCustomerError @CustomerErrorID,@PageSize,@Page,@SourceID,@Project,@MobilePhone,@Visiable,@IsDeleted", new UserLogin(), param).ToList();
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: GetAll, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetCustomerDatasource(DataSourceRequest dsRequest, int? sourceID = null, int? ProjectID = null, string mobilePhone = null, bool? visiable = null, bool? isDeleted = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerErrorID", null, DbType.Guid);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@Project", ProjectID, DbType.Int32);
                param.Add("@MobilePhone", mobilePhone, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@IsDeleted", isDeleted, DbType.Boolean);
                var r = DALHelpers.Query<CustomerError>("telesales_GetAllCustomerError @CustomerErrorID,@PageSize,@Page,@SourceID,@Project,@MobilePhone,@Visiable,@IsDeleted", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public Guid AddNewCustomer(CustomerError customer)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerErrorID", customer.CustomerErrorID, DbType.Guid);
                param.Add("@SourceID", customer.SourceID, DbType.Int32);
                param.Add("@ProjectID", customer.ProjectID, DbType.Int32);
                param.Add("@Phone", customer.Phone, DbType.String);
                param.Add("@RowError", customer.RowError, DbType.Int32);
                param.Add("@Visiable", customer.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", customer.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Query<Guid>("telesales_AddNewCustomerError @CustomerErrorID,@SourceID,@ProjectID,@Phone,@RowError,@Visiable,@IsDeleted", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: AddNewCustomer, Detail: " + ex.ToString());
                return Guid.Empty;
            }
        }

        public int UpdateCustomer(CustomerError customer)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerErrorID", customer.CustomerErrorID, DbType.Guid);
                param.Add("@SourceID", customer.SourceID, DbType.Int32);
                param.Add("@ProjectID", customer.ProjectID, DbType.Int32);
                param.Add("@MobilePhone", customer.Phone, DbType.String);
                param.Add("@RowError", customer.RowError, DbType.Int32);
                param.Add("@Visiable", customer.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", customer.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateCustomerError", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: UpdateCustomer, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteCustomer(Guid id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerErrorID", id, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCustomerErrorNew",
                    new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: DeleteCustomer, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteCustomerErrorBySourceID(int SourceID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@SourceID", SourceID, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteCustomerErrorBySourceID",
                    new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: DeleteCustomerErrorBySourceID, Detail: " + ex.ToString());
                return 0;
            }
        }
        
        public CustomerError GetCustomer(Guid id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerErrorID", id, DbType.Guid);
                CustomerError obj = new CustomerError();
                obj = DALHelpers.Query<CustomerError>("telesales_GetCustomerErrorById @CustomerErrorID",
                    new UserLogin(), param).First();
                return obj;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: GetCustomer, Detail: " + ex.ToString());
                return null;
            }
        }

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
                var r = DALHelpers.Query<object>("telesales_CustomerErrorGetAllFieldValue @ProjectID,@SourceID,@Phone,@Visiable,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                int total = 0;
                if (r.Count() != 0)
                    foreach (IDictionary<string, object> rp in r)
                    {
                        total = Convert.ToInt32(rp.FirstOrDefault(x => x.Key == "Total").Value);
                        break;
                    }

                return new DataSourceResult() { Data = r, Total = total };
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: GetAllFieldValueDatasource, Detail: " + ex.ToString());
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
                var r = DALHelpers.Query<object>("telesales_CustomerErrorGetAllFieldValue @ProjectID,@SourceID,@Phone,@Visiable,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: GetAllFieldValue, Detail: " + ex.ToString());
                return null;
            }
        }
    }
}
