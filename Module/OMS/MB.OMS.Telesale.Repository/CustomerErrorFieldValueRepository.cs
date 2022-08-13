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
    public class CustomerErrorFieldValueRepository : ICustomerErrorFieldValueRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.CustomerErrorFieldValueRepository");
        public IEnumerable<CustomerErrorFieldValue> GetAll(int? customerFieldID = null, Guid? CustomerErrorID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", customerFieldID, DbType.Int32);
                param.Add("@CustomerErrorID", CustomerErrorID, DbType.Guid);
                var r = DALHelpers.Query<CustomerErrorFieldValue>("telesales_GetCustomerErrorFieldValue @CustomerFieldID,@CustomerErrorID", new UserLogin(), param).ToList();
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: GetAll, Detail:" + ex.ToString());
                return null;
            }
        }

        public int AddNewCustomerErrorFieldValue(CustomerErrorFieldValue customerErrorFieldValue)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", customerErrorFieldValue.CustomerFieldID, DbType.Int32);
                param.Add("@CustomerErrorID", customerErrorFieldValue.CustomerErrorID, DbType.Guid);
                param.Add("@FieldValue", customerErrorFieldValue.FieldValue, DbType.String);
                var r = DALHelpers.Execute("telesales_AddNewCustomerErrorFieldValue", new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: AddNewCustomerErrorFieldValue, Detail:" + ex.ToString());
                return 0;
            }
        }

        public int UpdateCustomerErrorFieldValue(CustomerErrorFieldValue customerFieldValue)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", customerFieldValue.CustomerFieldID, DbType.Int32);
                param.Add("@CustomerErrorID", customerFieldValue.CustomerErrorID, DbType.Guid);
                param.Add("@FieldValue", customerFieldValue.FieldValue, DbType.String);
                var r = DALHelpers.Execute("telesales_UpdateCustomerErrorFieldValue", new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: UpdateCustomerErrorFieldValue, Detail:" + ex.ToString());
                return 0;
            }
        }

        public int DeleteCustomerErrorFieldValue(int customerFieldID, Guid CustomerErrorID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerFieldID", customerFieldID, DbType.Int32);
                param.Add("@CustomerErrorID", CustomerErrorID, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCustomerErrorFieldValue", new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: DeleteCustomerErrorFieldValue, Detail:" + ex.ToString());
                return 0;
            }
        }
        public int DeleteCustomerErrorFieldAllValue(Guid CustomerErrorID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CustomerErrorID", CustomerErrorID, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteCustomerErrorFieldAllValue", new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error: DeleteCustomerErrorFieldAllValue, Detail:" + ex.ToString());
                return 0;
            }
        }
    }
}
