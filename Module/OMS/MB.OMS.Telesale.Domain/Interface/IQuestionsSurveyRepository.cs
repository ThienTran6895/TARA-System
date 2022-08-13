using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IQuestionsSurveyRepository
    {
        IEnumerable<QuestionsSurvey> GetAll(int projectID, int? questionID = null, int? surveyID = null);
        int AddNewQuestionsSurvey(QuestionsSurvey questionsSurvey);
        int UpdateQuestionsSurvey(QuestionsSurvey questionsSurvey);
        int DeleteQuestionsSurvey(int projectID, int questionID, int? surveyID = null);
    }
}
