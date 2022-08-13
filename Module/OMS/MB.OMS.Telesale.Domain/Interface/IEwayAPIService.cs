using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IEwayAPIService
    {
        bool EwayExportData(NameValueCollection fields, int status);
    }
}
