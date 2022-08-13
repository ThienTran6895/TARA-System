using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.OMS.Telesale.Domain.Model;
using MB.Common.Kendoui;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IQuestionRepository
    {
        IEnumerable<Question> GetAll(string name = null, string code = null, bool? visiable = null);
        DataSourceResult GetQuestionDatasource(DataSourceRequest dsRequest, string name = null, string code = null, bool? visiable = null);
        int AddNewQuestion(Question question);
        int UpdateQuestion(Question question);
        int DeleteQuestion(int id);
        QuestionDTO GetQuestionById(int id);
        IEnumerable<QuestionDTO> GetQuestionByProjectId(int? projectId = null, bool? visiable = null);
        DataSourceResult GetQuestionForProjectDatasource(DataSourceRequest dsRequest, int projectID, string name = null, bool? visiable = null);
        DataSourceResult GetQuestionNotInForProjectDatasource(DataSourceRequest dsRequest, int projectID, string name = null, bool? visiable = null);
    }
}
