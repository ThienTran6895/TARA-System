using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.Web.Core.Validations;
using System.Web.Mvc;
using System.ComponentModel;
using MB.Common.Helpers;

namespace MB.OMS.Telesale.Domain.Model
{
    public class Question
    {
        public Question()
        {
            Visiable = true;
        }

        public int QuestionID { get; set; }

        [DisplayName("Mã câu hỏi ")]
        public string Code { get; set; }

        [Required(ErrorMessage = "{0} không được để trống")]
        [DisplayName("Tên câu hỏi ")]
        [AllowHtml]
        public string Name { get; set; }

        public string TieuDeNgan
        {
            get { return CommonHelper.CutString(50, Name); }
            set { ;}
        }
        public int NextQuestionID { get; set; }

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
