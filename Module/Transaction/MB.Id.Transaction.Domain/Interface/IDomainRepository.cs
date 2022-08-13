using System.Collections.Generic;
using MB.OMS.Transaction.Domain.Model;

namespace MB.OMS.Transaction.Domain.Interface
{
    interface IDomainRepository
    {
        List<DomainName> GetAllDomainNames();
    }
}
