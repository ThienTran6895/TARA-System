using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Domain.Model
{
    public class CustomerErrorDTO : CustomerError
    {
        public CustomerErrorDTO()
        {
            AvailableSource = new List<SelectListItem>();
            AvailableProject = new List<SelectListItem>();
        }
        public List<SelectListItem> AvailableSource { get; set; }
        public List<SelectListItem> AvailableProject { get; set; }
    }
}
