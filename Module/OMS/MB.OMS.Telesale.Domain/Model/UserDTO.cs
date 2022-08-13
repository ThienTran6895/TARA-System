using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Domain.Model
{
    public class UserDTO : User
    {
        public UserDTO()
        {
            AvailableGenders = new List<SelectListItem>();
            AvailableRoles = new List<Roles>();
            AvailableProjects = new List<Projects>();
            Visible = true;
        }

        [DisplayName("Họ tên ")]
        public string FullName { get; set; }

        public List<SelectListItem> AvailableGenders { get; set; }
        public List<Roles> AvailableRoles { get; set; }
        public int[] SelectedRoleIds { get; set; }
        public List<Projects> AvailableProjects { get; set; }
        public int[] SelectedProjectIds { get; set; }
    }
}
