using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class CallLog
    {
        public int CallLogID { get; set; }
        public int CallID { get; set; }
        public int ProjectID { get; set; }
        public Guid UserId { get; set; }
        public Guid CustomerID { get; set; }
        public int StatusCallID { get; set; }
        public DateTime? RecallDate { get; set; }
        public string Note { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool IsSuccess { get; set; }
        public string MobilePhone { get; set; }
        public string StatusName { get; set; }
        public int StatusID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public int Total { get; set; }
        // sondt - 2019 -- bo sung cho AUTOCALL
        public Int64 CisId { get; set; }
        public string AgentCode { get; set; }

    }

    public class UpdateCallLogFormModel
    {
        public CallLog CallLog { get; set; }
        public List<CustomerValue> CustomerInfo { get; set; }
    }
}
