using MB.Web.Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class Sources
    {
        public int SourceID { get; set; }
        
        [DisplayName("Tên nguồn ")]
        [Required(ErrorMessage = "{0} không được để trống")]
        public string Name { get; set; }
        public string Link { get; set; }

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

        public int Total { get; set; }
    }
}
