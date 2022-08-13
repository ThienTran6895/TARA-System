using DAL.Core;
using Dapper;
using MB.Common;
using MB.OMS.Telesale.Domain.Interface;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Repository
{
    public class QuestionsSurveyRepository : IQuestionsSurveyRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.QuestionsSurveyRepository");
        public IEnumerable<QuestionsSurvey> GetAll(int projectID, int? questionID = null, int? surveyID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@QuestionID", questionID, DbType.Int32);
                param.Add("@SurveyID", surveyID, DbType.Int32);
                var r = DALHelpers.Query<QuestionsSurvey>("telesales_GetQuestionsSurvey @ProjectID,@QuestionID,@SurveyID", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }
        }

        public int AddNewQuestionsSurvey(QuestionsSurvey questionsSurvey)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", questionsSurvey.ProjectID, DbType.Int32);
                param.Add("@QuestionID", questionsSurvey.QuestionID, DbType.Int32);
                param.Add("@SurveyID", questionsSurvey.SurveyID, DbType.Int32);
                param.Add("@NextQuestionID", questionsSurvey.NextQuestionID, DbType.Int32);
                param.Add("@DisplayOrder", questionsSurvey.DisplayOrder, DbType.Int32);
                var r = DALHelpers.Execute("telesales_AddNewQuestionsSurvey", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewQuestionsSurvey, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int UpdateQuestionsSurvey(QuestionsSurvey questionsSurvey)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", questionsSurvey.ProjectID, DbType.Int32);
                param.Add("@QuestionID", questionsSurvey.QuestionID, DbType.Int32);
                param.Add("@SurveyID", questionsSurvey.SurveyID, DbType.Int32);
                param.Add("@NextQuestionID", questionsSurvey.NextQuestionID, DbType.Int32);
                param.Add("@DisplayOrder", questionsSurvey.DisplayOrder, DbType.Int32);
                var r = DALHelpers.Execute("telesales_UpdateQuestionsSurvey", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error UpdateQuestionsSurvey, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteQuestionsSurvey(int projectID, int questionID, int? surveyID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@QuestionID", questionID, DbType.Int32);
                param.Add("@SurveyID", surveyID, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteQuestionsSurvey", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteQuestionsSurvey, Detail: " + ex.ToString());
                return 0;
            }
        }
    }
}
