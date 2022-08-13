using MB.Web.Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MB.Common.Helpers;

namespace MB.OMS.Telesale.Domain.Model
{
    public class Survey
    {
        public Survey()
        {
            Visiable = true;
        }
        public int SurveyID { get; set; }

        public string NoiDungNgan
        {
            get { return CommonHelper.CutString(50, SurveyContent); }
            set { ;}
        }
        [DisplayName("Câu trả lời ")]
        [Required(ErrorMessage = "{0} không được để trống")]
        [AllowHtml]
        public string SurveyContent { get; set; }

        [DisplayName("Mã câu trả lời ")]
        public string Code { get; set; }

        [DisplayName("Ngày tạo ")]
        public DateTime CreatedDate { get; set; }

        [DisplayName("Người tạo ")]
        public Guid CreatedBy { get; set; }

        [DisplayName("Ngày cập nhật ")]
        public DateTime UpdatedDate { get; set; }

        [DisplayName("Người cập nhật ")]
        public Guid UpdatedBy { get; set; }

        [DisplayName("Loại hiển thị ")]
        public int SurveyType { get; set; }

        [DisplayName("Hiển thị ")]
        public bool Visiable { get; set; }
        public bool IsDeleted { get; set; }

        public int Total { get; set; }
    }
}
