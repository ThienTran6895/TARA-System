using MB.Web.Core.Validations;

namespace MB.OMS.Telesale.Domain.Model
{ 
    public class EwayCustomerModel
    {
        public string projectId { get; set; }
        public string accesstoken { get; set; }
        public string phone { get; set; }
        public string name { get; set; }
        public string product_id { get; set; }
        public string click_id { get; set; }
        public string transaction_id { get; set; }
        public string contact_time { get; set; }
        public string offer_id { get; set; }
    }
}
