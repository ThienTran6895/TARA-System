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
    public class CustomerFieldRepository : ICustomerFieldRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.CustomerFieldRepository");
        public IEnumerable<CustomerField> GetAll(string fieldcode = null, string fieldname = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@FieldCode", fieldcode, DbType.String);
                param.Add("@FieldName", fieldname, DbType.String);
                var r = DALHelpers.Query<CustomerField>("telesales_GetAllCustomerField @PageSize,@Page,@FieldCode,@FieldName", new UserLogin(), param).ToList();
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: GetAll, Detail:" + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetCustomerFieldDatasource(DataSourceRequest dsRequest, string fieldcode = null, string fieldname = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@FieldCode", fieldcode, DbType.String);
                param.Add("@FieldName", fieldname, DbType.String);
                var r = DALHelpers.Query<CustomerField>("telesales_GetAllCustomerField @PageSize,@Page,@FieldCode,@FieldName", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerFieldDatasource, Detail:" + ex.ToString());
                return null;
            }
        }

        public int AddNewCustomerField(CustomerField customerField)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@FieldCode", customerField.FieldCode, DbType.String);
                param.Add("@FieldName", customerField.FieldName, DbType.String);
                param.Add("@DataTypeID", customerField.DataTypeID, DbType.Int32);
                param.Add("@ControlTypeID", customerField.ControlTypeID, DbType.Int32);
                param.Add("@Order", customerField.Order, DbType.Int32);
                var r = DALHelpers.Query<int>("telesales_AddNewCustomerField @FieldCode,@FieldName,@DataTypeID,@ControlTypeID,@Order", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: AddNewCustomerField, Detail:" + ex.ToString());
                return 0;
            }
        }

        public int UpdateCustomerField(CustomerField customerField)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", customerField.CustomerFieldID, DbType.Int32);
                param.Add("@FieldCode", customerField.FieldCode, DbType.String);
                param.Add("@FieldName", customerField.FieldName, DbType.String);
                param.Add("@DataTypeID", customerField.DataTypeID, DbType.Int32);
                param.Add("@ControlTypeID", customerField.ControlTypeID, DbType.Int32);
                param.Add("@Order", customerField.Order, DbType.Int32);
                var r = DALHelpers.Execute("telesales_UpdateCustomerField", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: UpdateCustomerField, Detail:" + ex.ToString());
                return 0;
            }
        }

        public int DeleteCustomerField(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", id, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteCustomerField", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: DeleteCustomerField, Detail:" + ex.ToString());
                return 0;
            }
        }
        
        
        public CustomerFieldDTO GetCustomerField(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Int32);
                var r = DALHelpers.Query<CustomerFieldDTO>("telesales_GetCustomerFieldById @Id",
                    new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerField, Detail:" + ex.ToString());
                return null;
            }
        }

        public IEnumerable<CustomerField> GetCustomerFieldByProject(int projectID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                var r = DALHelpers.Query<CustomerField>("telesales_GetCustomerFieldByProject @ProjectID", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerFieldByProject, Detail:" + ex.ToString());
                return null;
            }
        }

        public IEnumerable<CustomerFieldDTO> GetCustomerFieldAndValueByCustomerID(Guid customerID, int projectID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerID", customerID, DbType.Guid);
                param.Add("@ProjectID", projectID, DbType.Int32);
                var r = DALHelpers.Query<CustomerFieldDTO>("telesales_GetCustomerFieldAndValueByCustomerID @CustomerID,@ProjectID", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerFieldAndValueByCustomerID, Detail:" + ex.ToString());
                return null;
            }
        }

        public IEnumerable<CustomerFieldDTO> GetCustomerFieldAndValueByMobilePhone(string phone, Guid customerId, int projectID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Phone", phone, DbType.String);
                param.Add("@customerId", customerId, DbType.Guid);
                param.Add("@ProjectID", projectID, DbType.Int32);
                var r = DALHelpers.Query<CustomerFieldDTO>("telesales_GetCustomerFieldAndValueByPhone @Phone,@customerId,@ProjectID", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerFieldAndValueByMobilePhone, Detail:" + ex.ToString());
                return null;
            }
        }

        public IEnumerable<CustomerFieldDTO> GetCustomerFieldAndValueByCustomerExistID(Guid customerExistID, int projectID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerExistID", customerExistID, DbType.Guid);
                param.Add("@ProjectID", projectID, DbType.Int32);
                var r = DALHelpers.Query<CustomerFieldDTO>("telesales_GetCustomerFieldAndValueByCustomerExistID @CustomerExistID,@ProjectID", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerFieldAndValueByCustomerExistID, Detail:" + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetCustomerFieldNotInForProjectDatasource(DataSourceRequest dsRequest, int projectID, string fieldname = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@FieldName", fieldname, DbType.String);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<CustomerField>("telesales_GetAllCustomerFieldNotInForProject @ProjectID,@FieldName,@PageSize,@Page", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerFieldNotInForProjectDatasource, Detail:" + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetCustomerFieldForProjectDatasource(DataSourceRequest dsRequest, int projectID, string fieldname = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@FieldName", fieldname, DbType.String);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<CustomerField>("telesales_GetAllCustomerFieldForProject @ProjectID,@FieldName,@PageSize,@Page", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetCustomerFieldForProjectDatasource, Detail:" + ex.ToString());
                return null;
            }
        }
    }
}
