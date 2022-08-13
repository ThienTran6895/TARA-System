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
    public class SurveyAnswerRepository : ISurveyAnswerRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.SurveyAnswerRepository");
        public int AddNewSurveyAnswer(SurveyAnswer surveyAnswer)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CallLogID", surveyAnswer.CallLogID, DbType.Int32);
                param.Add("@QuestionID", surveyAnswer.QuestionID, DbType.Int32);
                param.Add("@SurveyID", surveyAnswer.SurveyID, DbType.Int32);
                param.Add("@SurveyContent", surveyAnswer.SurveyContent, DbType.String);
                var r = DALHelpers.Query<int>("telesales_AddNewSurveyAnswer @CallLogID,@QuestionID,@SurveyID,@SurveyContent", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewSurveyAnswer, Detail: " + ex.ToString());
                return 0;
            }
        }
        public int DeleteSurveyAnswer(int CallLogID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CallLogID", CallLogID, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteSurveyAnswer", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteSurveyAnswer, Detail: " + ex.ToString());
                return 0;
            }
        }

        
    }
}
