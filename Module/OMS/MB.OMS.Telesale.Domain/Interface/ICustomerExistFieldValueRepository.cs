using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface ICustomerExistFieldValueRepository
    {
        IEnumerable<CustomerExistFieldValue> GetAll(int? customerFieldID = null, Guid? customerExistID = null);
        int AddNewCustomerExistFieldValue(CustomerExistFieldValue customerExistFieldValue);
        int UpdateCustomerExistFieldValue(CustomerExistFieldValue customerExistFieldValue);
        int DeleteCustomerExistFieldValue(int customerFieldID, int customerExistID);
        int DeleteCustomerExistFieldAllValue(Guid customerExistID);
        IEnumerable<CustomerExistFieldValue> GetAllCustomerExistByCustomerID(Guid? customerID = null);
    }
}
