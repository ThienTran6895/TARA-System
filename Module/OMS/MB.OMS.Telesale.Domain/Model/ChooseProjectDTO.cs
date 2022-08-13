using MB.Web.Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Domain.Model
{
    public class ChooseProjectDTO
    {
        public ChooseProjectDTO()
        {
            AvailableProject = new List<SelectListItem>();
        }
        public List<SelectListItem> AvailableProject { get; set; }

        [DisplayName("Dự án ")]
        //[RequiredAttribute(ErrorMessage = "{0} không được để trống")]
        public int ProjectID { get; set; }
    }
}
