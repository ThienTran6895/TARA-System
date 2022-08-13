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
    public class Projects
    {
        public int ProjectID { get; set; }

        [DisplayName("Chiến dịch ")]
        public int CampaignID { get; set; }       

        [Required(ErrorMessage = "{0} không được để trống")]
        [DisplayName("Mã dự án ")]
        public string Code { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [DisplayName("Tên dự án ")]
        public string Name { get; set; }       

        [DisplayName("Lời chào ")]
        [AllowHtml]
        public string Greeting { get; set; }

        [DisplayName("Lời kết ")]
        [AllowHtml]
        public string Conclusion { get; set; }

        [DisplayName("Tổng số lượng dự kiến")]
        public int TotalPlan { get; set; }

        [DisplayName("Tổng mục tiêu dự kiến")]
        public int TotalTarget { get; set; }

        [DisplayName("Số lượng/tháng dự kiến")]
        public int MonthlyPlan { get; set; }

        [DisplayName("Mục tiêu/tháng dự kiến")]
        public int MonthlyTarget { get; set; }

        [DisplayName("Số lượng/ngày dự kiến")]
        public int DailyPlan { get; set; }

        [DisplayName("Mục tiêu/ngày dự kiến")]
        public int DailyTarget { get; set; }

        [DisplayName("Ngày tạo ")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Người tạo ")]
        public Guid CreatedBy { get; set; }

        [DisplayName("Ngày cập nhật ")]
        public DateTime UpdatedDate { get; set; }

        [DisplayName("Người cập nhật ")]
        public Guid UpdatedBy { get; set; }

        [DisplayName("Hiển thị ")]
        public bool Visiable { get; set; }

        public bool IsDeleted { get; set; }

        public int Total { get; set; }
    }
}
