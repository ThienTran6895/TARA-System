using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Model
{
    public class ListSurverAnswer
    {
        public ListSurverAnswer() {
            SurveyAnswer = new List<SurveyAnswer>();
        }
        public int CallLogID { get; set; }
        public List<SurveyAnswer> SurveyAnswer { set; get; }
       
    }

}
