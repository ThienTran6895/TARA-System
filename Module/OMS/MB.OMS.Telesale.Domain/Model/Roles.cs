using MB.Web.Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class Roles
    {
        public int Id { get; set; }

        [DisplayName("Tên quyền")]
        [RequiredAttribute(ErrorMessage = "{0} không được để trống")]
        public string RoleName { get; set; }

        [DisplayName("Trạng thái ")]
        public bool IsVisible { get; set; }

        public int Total { get; set; }
    }
}
