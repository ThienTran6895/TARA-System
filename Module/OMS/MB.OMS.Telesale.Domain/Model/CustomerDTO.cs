using MB.OMS.Common.Domain.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Domain.Model
{
    public class CustomerDTO : Customer
    {
        public CustomerDTO()
        {
            AvailableSource = new List<SelectListItem>();
            AvailableCustomerFieldValue = new List<CustomerFieldDTO>();
            AvailableProject = new List<SelectListItem>();
            AvailableUser = new List<SelectListItem>();
            AvailableCustomer = new List<SelectListItem>();
        }
        public List<SelectListItem> AvailableSource { get; set; }

        public List<CustomerFieldDTO> AvailableCustomerFieldValue { get; set; }

        public List<SelectListItem> AvailableProject { get; set; }

        public List<SelectListItem> AvailableCustomer { get; set; }

        [DisplayName("Dự án ")]
        public int ProjectID { get; set; }

        public int SourceID { get; set; }

        public string SourceName { get; set; }

        public string DateFrom { get; set; }

        public string DateEnd { get; set; }

        //[DisplayName("Dự án ")]
        //public int SearchProjectID { get; set; }

        [DisplayName("Họ tên khách hàng ")]
        public string FullName { get; set; }


        [DisplayName("Danh sách điện thoại viên")]
        public List<Guid> UserId { get; set; }
        public List<SelectListItem> AvailableUser { get; set; }

        [DisplayName("Số lượng KH muốn phân công")]
        public int CountCustomer { get; set; }

        public string ProjectName { get; set; }

        [DisplayName("Điện thoại viên cần chuyển")]
        public Guid UserId1 { get; set; }

        [DisplayName("Điện thoại viên được chuyển")]
        public Guid UserId2 { get; set; }

        [DisplayName("Danh sách khách hàng")]
        public List<Guid> ListCustomerID { get; set; }
    }

}
