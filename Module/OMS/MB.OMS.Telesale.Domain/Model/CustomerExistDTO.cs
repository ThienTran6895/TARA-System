using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Domain.Model
{
    public class CustomerExistDTO : CustomerExist
    {
        public CustomerExistDTO()
        {
            AvailableSource = new List<SelectListItem>();
            AvailableProject = new List<SelectListItem>();
            AvailableCustomerFieldValue = new List<CustomerFieldDTO>();
        }
        public List<SelectListItem> AvailableSource { get; set; }
        public List<SelectListItem> AvailableProject { get; set; }
        public List<CustomerFieldDTO> AvailableCustomerFieldValue { get; set; }
        public CustomerDTO Customer { get; set; }
    }
}
