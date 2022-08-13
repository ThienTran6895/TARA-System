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
    public class Campaigns
    {
        public int CampaignID { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [DisplayName("Tên chiến dịch: ")]
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public Guid UpdatedBy { get; set; }

        [DisplayName("Hiển thị ")]
        public bool Visiable { get; set; }
        public bool IsDeleted { get; set; }

        public int Total { get; set; }
    }
}
