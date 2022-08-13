using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface ISurveyRepository
    {
        IEnumerable<Survey> GetAll(string surveyContent = null, string code = null, int? surveyType = null, bool? visiable = null);
        DataSourceResult GetSurveyDatasource(DataSourceRequest dsRequest, string surveyContent = null, string code = null, int? surveyType = null, bool? visiable = null);
        int AddNewSurvey(Survey survey);
        int UpdateSurvey(Survey survey);
        int DeleteSurvey(int id);
        SurveyDTO GetSurvey(int id);
        IEnumerable<SurveyDTO> GetSurveyByQuestionId(int projectID, int questionID, string codeSurveys = null, string survey = null);
        DataSourceResult GetAllForQuestionsDatasource(DataSourceRequest dsRequest, int projectID, int questionID, string codeSurveys = null, string survey = null);
        DataSourceResult GetAllNotInQuestionsSurveyDatasource(DataSourceRequest dsRequest, int projectID, int questionID, string codeSurveys = null, string survey = null);
        IEnumerable<SurveyDTO> GetSurveyByQuestionIdTK(int projectID);
    }
}
