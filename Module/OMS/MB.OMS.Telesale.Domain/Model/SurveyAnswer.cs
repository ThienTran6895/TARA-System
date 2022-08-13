using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class SurveyAnswer
    {
        public int SurveyAnswerID { get; set; }
        public int CallLogID { get; set; }
        public int QuestionID { get; set; }
        public int SurveyID { get; set; }
        public string SurveyContent { get; set; }
    }
}
