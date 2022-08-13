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
    public class CustomerExistRepository : ICustomerExistRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.CustomerExistRepository");
        public IEnumerable<CustomerExist> GetAll(int? sourceID = null, Guid? customerID = null, int? projectID = null, string phone = null, int? rowExist = null, bool? visiable = null, bool? isDeleted = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@RowExist", rowExist, DbType.Int32);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@IsDeleted", isDeleted, DbType.Boolean);
                var r = DALHelpers.Query<CustomerExist>("telesales_GetAllCustomerExist @PageSize,@Page,@SourceID,@CustomerID,@ProjectID,@Phone,@RowExist,@Visiable,@IsDeleted", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {

                logger.Fatal("Error: GetAll, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetCustomerExistDatasource(DataSourceRequest dsRequest, int? sourceID = null, Guid? customerID = null, int? projectID = null, string phone = null, int? rowExist = null, bool? visiable = null, bool? isDeleted = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@SourceID", sourceID, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@RowExist", rowExist, DbType.Int32);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@IsDeleted", isDeleted, DbType.Boolean);
                var r = DALHelpers.Query<CustomerExist>("telesales_GetAllCustomerExist @PageSize,@Page,@SourceID,@CustomerID,@ProjectID,@Phone,@RowExist,@Visiable,@IsDeleted", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {

                logger.Fatal("Error: GetCustomerExistDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public Guid AddNewCustomerExist(CustomerExist customerExist)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerExistID", customerExist.CustomerExistID, DbType.Guid);
                param.Add("@SourceID", customerExist.SourceID, DbType.Int32);
                param.Add("@CustomerID", customerExist.CustomerID, DbType.Guid);
                param.Add("@ProjectID", customerExist.ProjectID, DbType.Int32);
                param.Add("@Phone", customerExist.Phone, DbType.String);
                param.Add("@RowExist", customerExist.RowExist, DbType.Int32);
                param.Add("@Visiable", customerExist.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", customerExist.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Query<Guid>("telesales_AddNewCustomerExist @CustomerExistID,@SourceID,@CustomerID,@ProjectID,@Phone,@RowExist,@Visiable,@IsDeleted", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {

                logger.Fatal("Error: AddNewCustomerExist, Detail: " + ex.ToString());
                return Guid.Empty;
            }
        }

        public int UpdateCustomerExist(CustomerExist customerExist)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerExistID", customerExist.CustomerExistID, DbType.Guid);
                param.Add("@SourceID", customerExist.SourceID, DbType.Int32);
                param.Add("@CustomerID", customerExist.CustomerID, DbType.Guid);
                param.Add("@ProjectID", customerExist.ProjectID, DbType.Int32);
                param.Add("@Phone", customerExist.Phone, DbType.String);
                param.Add("@RowExist", customerExist.RowExist, DbType.Int32);
                param.Add("@Visiable", customerExist.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", customerExist.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateCustomerExist", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {

                logger.Fatal("Error: UpdateCustomerExist, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteCustomerExist(Guid id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerExistID", id, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCustomerExist", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {

                logger.Fatal("Error: DeleteCustomerExist, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteCustomerExistBySourceID(int SourceID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@SourceID", SourceID, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteCustomerExistBySourceID", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {

                logger.Fatal("Error: DeleteCustomerExistBySourceID, Detail: " + ex.ToString());
                return 0;
            }
        }
        
        public CustomerExistDTO GetCustomerExist(Guid id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Guid);
                var r = DALHelpers.Query<CustomerExistDTO>("telesales_GetCustomerExistById @Id", new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {

                logger.Fatal("Error: GetCustomerExist, Detail: " + ex.ToString());
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
                var r = DALHelpers.Query<object>("telesales_CustomerExistGetAllFieldValue @ProjectID,@SourceID,@Phone,@Visiable,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

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
                var r = DALHelpers.Query<object>("telesales_CustomerExistGetAllFieldValue @ProjectID,@SourceID,@Phone,@Visiable,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return r;
            }
            catch (Exception ex)
            {

                logger.Fatal("Error: GetAllFieldValue, Detail: " + ex.ToString());
                return null;
            }
        }  
    }
}
