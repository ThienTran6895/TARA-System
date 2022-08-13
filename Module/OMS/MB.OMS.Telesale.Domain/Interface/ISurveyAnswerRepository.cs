using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface ISurveyAnswerRepository
    {
        int AddNewSurveyAnswer(SurveyAnswer surveyAnswer);
        int DeleteSurveyAnswer(int CallLogID);
    }
}
