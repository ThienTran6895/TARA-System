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
    public class CustomerFieldValueRepository : ICustomerFieldValueRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.CustomerFieldValueRepository");
        public IEnumerable<CustomerFieldValue> GetAll(int? customerFieldID = null, Guid? customerID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", customerFieldID, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                var r = DALHelpers.Query<CustomerFieldValue>("telesales_GetCustomerFieldValueById @CustomerFieldID,@CustomerID", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: GetAll, Detail:" + ex.ToString());
                return null;
            }
        }

        public int AddNewCustomerFieldValue(CustomerFieldValue customerFieldValue)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", customerFieldValue.CustomerFieldID, DbType.Int32);
                param.Add("@CustomerID", customerFieldValue.CustomerID, DbType.Guid);
                param.Add("@FieldValue", customerFieldValue.FieldValue, DbType.String);
                var r = DALHelpers.Execute("telesales_AddNewCustomerFieldValue", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: AddNewCustomerFieldValue, Detail:" + ex.ToString());
                return 0;
            }
        }

        public int UpdateCustomerFieldValue(CustomerFieldValue customerFieldValue)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", customerFieldValue.CustomerFieldID, DbType.Int32);
                param.Add("@CustomerID", customerFieldValue.CustomerID, DbType.Guid);
                param.Add("@FieldValue", customerFieldValue.FieldValue, DbType.String);
                var r = DALHelpers.Execute("telesales_UpdateCustomerFieldValue", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: UpdateCustomerFieldValue, Detail:" + ex.ToString());
                return 0;
            }
        }

        public int DeleteCustomerFieldValue(int customerFieldID, Guid customerID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", customerFieldID, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCustomerFieldValue", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: DeleteCustomerFieldValue, Detail:" + ex.ToString());
                return 0;
            }
        }
        public int DeleteCustomerFieldAllValue(Guid id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerID", id, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCustomerFieldAllValue", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: DeleteCustomerFieldAllValue, Detail:" + ex.ToString());
                return 0;
            }
        }
    }
}
