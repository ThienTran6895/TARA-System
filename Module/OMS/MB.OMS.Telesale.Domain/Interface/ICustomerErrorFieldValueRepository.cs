using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MB.OMS.Telesale.Domain.Interface
{
    public interface ICustomerErrorFieldValueRepository
    {
        IEnumerable<CustomerErrorFieldValue> GetAll(int? customerFieldID = null, Guid? CustomerErrorID = null);

        int AddNewCustomerErrorFieldValue(CustomerErrorFieldValue customerErrorFieldValue);

        int UpdateCustomerErrorFieldValue(CustomerErrorFieldValue customerFieldValue);

        int DeleteCustomerErrorFieldValue(int customerFieldID, Guid CustomerErrorID);
        int DeleteCustomerErrorFieldAllValue(Guid CustomerErrorID);
    }
}
