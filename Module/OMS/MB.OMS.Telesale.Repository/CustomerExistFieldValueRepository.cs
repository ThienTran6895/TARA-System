using DAL.Core;
using Dapper;
using MB.Common;
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
    public class CustomerExistFieldValueRepository : ICustomerExistFieldValueRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.CustomerExistFieldValueRepository");
        public IEnumerable<CustomerExistFieldValue> GetAll(int? customerFieldID = null, Guid? customerExistID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", customerFieldID, DbType.Int32);
                param.Add("@CustomerExistID", customerExistID, DbType.Guid);
                var r = DALHelpers.Query<CustomerExistFieldValue>("telesales_GetCustomerExistFieldValue @CustomerFieldID,@CustomerExistID", new UserLogin(), param).ToList();
                return r;
            }
            catch(Exception ex)
            {
                 
                logger.Fatal("Error: GetAll, Detail: " + ex.ToString());
                return null;
            }
        
        }
        public IEnumerable<CustomerExistFieldValue> GetAllCustomerExistByCustomerID(Guid? customerID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@SourceID", null, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                param.Add("@ProjectID", null, DbType.Int32);
                param.Add("@Phone", null, DbType.Int32);
                param.Add("@RowExist", null, DbType.Int32);
                param.Add("@Visiable", null, DbType.Int32);
                param.Add("@IsDeleted", null, DbType.Int32);
                var r = DALHelpers.Query<CustomerExistFieldValue>("telesales_GetCustomerExistFieldValue @PageSize,@Page,@SourceID,@CustomerID,@ProjectID,@Phone,@RowExist,@Visiable,@IsDeleted ", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {

                logger.Fatal("Error: GetAllCustomerExistByCustomerID, Detail: " + ex.ToString());
                return null;
            }
        }
        public int AddNewCustomerExistFieldValue(CustomerExistFieldValue customerExistFieldValue)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", customerExistFieldValue.CustomerFieldID, DbType.Int32);
                param.Add("@CustomerExistID", customerExistFieldValue.CustomerExistID, DbType.Guid);
                param.Add("@FieldValue", customerExistFieldValue.FieldValue, DbType.String);
                var r = DALHelpers.Execute("telesales_AddNewCustomerExistFieldValue", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {

                logger.Fatal("Error: AddNewCustomerExistFieldValue, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int UpdateCustomerExistFieldValue(CustomerExistFieldValue customerExistFieldValue)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", customerExistFieldValue.CustomerFieldID, DbType.Int32);
                param.Add("@CustomerExistID", customerExistFieldValue.CustomerExistID, DbType.Guid);
                param.Add("@FieldValue", customerExistFieldValue.FieldValue, DbType.String);
                var r = DALHelpers.Execute("telesales_UpdateCustomerExistFieldValue", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {

                logger.Fatal("Error: UpdateCustomerExistFieldValue, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteCustomerExistFieldValue(int customerFieldID, int customerExistID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", customerFieldID, DbType.Int32);
                param.Add("@CustomerExistID", customerExistID, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCustomerExistFieldValue", new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: DeleteCustomerExistFieldValue, Detail: " + ex.ToString());
                return 0;
            }
        }
        public int DeleteCustomerExistFieldAllValue(Guid customerExistID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerExistID", customerExistID, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCustomerExistFieldAllValue", new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: DeleteCustomerExistFieldAllValue, Detail: " + ex.ToString());
                return 0;
            }
        }
    }
}
