using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.Common;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Domain.Model
{
    public class SurveyDTO : Survey
    {
        public SurveyDTO()
        {
            AvailableNextQuestion = new List<SelectListItem>();
        }
        public List<SelectListItem> AvailableNextQuestion { get; set; }
        public TypeSurvey AvailableTypeSurvey { get; set; }

        public int QuestionID { get; set; }

        [DisplayName("Câu hỏi tiếp theo ")]
        public int NextQuestionID { get; set; }

        [DisplayName("Nội dung ")]
        public string SearchSurveyContent1 { get; set; }

        [DisplayName("Mã ")]
        public string SearchCode1 { get; set; }

        [DisplayName("Nội dung ")]
        public string SearchSurveyContent2 { get; set; }

        [DisplayName("Mã ")]
        public string SearchCode2 { get; set; }

        [DisplayName("Thứ tự hiển thị ")]
        public int DisplayOrder { get; set; }

        public int ProjectID { get; set; }
    }
}
