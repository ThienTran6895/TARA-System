using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MB.Common;

namespace MB.OMS.Telesale.Domain.Model
{
    public class CallSurvey : CustomerDTO
    {
        public CallSurvey()
        {
            AvailableQuestions = new List<QuestionDTO>();
            AvailableStatus = new List<SelectListItem>();
            AvailableStatusCall = new List<SelectListItem>();
        }

        [DisplayName("Lời chào ")]
        public string CampaignGreeting { get; set; }

        [DisplayName("Lời kết ")]
        public string CampaignConclusion { get; set; }

        public List<QuestionDTO> AvailableQuestions { get; set; }

        [DisplayName("Tình trạng cuộc gọi ")]
        public int StatusCallID { get; set; }
        public List<SelectListItem> AvailableStatusCall { get; set; }

        // sondt - 17/06/2019
        [DisplayName("Nhóm tình trạng ")]
        public int StatusID { get; set; }
        public List<SelectListItem> AvailableStatus { get; set; }


        [DisplayName("Ghi chú ")]
        public string Note { get; set; }

        [DisplayName("Ngày hẹn ")]
        public string RecallDate { get; set; }

        public int CallID { get; set; }

        public string AgentCode { get; set; }

        public Int64 CisId { get; set; }
        public int SourceId { get; set; }
    }
}
