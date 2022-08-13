using MB.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Domain.Model
{
    public class StatusCallDTO : StatusCall
    {
        public StatusCallDTO()
        {
            AvailableStatus = new List<SelectListItem>();
        }
        public List<SelectListItem> AvailableStatus { get; set; }
    }
}
