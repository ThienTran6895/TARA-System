using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface ICustomerFieldValueRepository
    {
        IEnumerable<CustomerFieldValue> GetAll(int? customerFieldID = null, Guid? customerID = null);

        int AddNewCustomerFieldValue(CustomerFieldValue customerFieldValue);

        int UpdateCustomerFieldValue(CustomerFieldValue customerFieldValue);

        int DeleteCustomerFieldValue(int customerFieldID, Guid customerID);

        int DeleteCustomerFieldAllValue(Guid id);
    }
}
