using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class QuestionsSurvey
    {
        public int ProjectID {get; set;}
        public int QuestionID { get; set; }
        public int SurveyID { get; set; }
        public int NextQuestionID {get; set;}
        public int DisplayOrder { get; set; }
    }
}
