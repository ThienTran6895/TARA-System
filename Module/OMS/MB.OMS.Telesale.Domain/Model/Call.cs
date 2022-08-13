using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Domain.Model
{
    public class Call
    {
        public Call()
        {
            AvailableProject = new List<SelectListItem>();
            AvailableUser = new List<SelectListItem>();
            AvailableFeild = new List<SelectListItem>();
            AvailableStatus = new List<SelectListItem>();
            AvailableSearch = new List<SelectListItem>();
        }
        public int CallID { get; set; }
        public Guid CustomerID { get; set; }
        public int ProjectID { get; set; }
        public Guid UserId { get; set; }
        public int StatusCallID { get; set; }
        public DateTime UpdateDate { get; set; }
        public bool IsSuccess { get; set; }
        public int Total { get; set; }
        public string MobilePhone { get; set; }

        public List<SelectListItem> AvailableProject { get; set; }

        public List<SelectListItem> AvailableUser { get; set; }
        public List<SelectListItem> AvailableFeild { get; set; }
        public List<SelectListItem> AvailableStatus { get; set; }
        public List<SelectListItem> AvailableSearch { get; set; }

        public string Name { get; set; }

        public string FullName { get; set; }

        public int NumCall { get; set; }

        public string CallBy { get; set; }

        public int CallLogID { get; set; }
        public string FieldName { get; set; }    
        public string Search_Mobile { get; set; }
    }
}
