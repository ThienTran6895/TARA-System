using DAL.Core;
using Dapper;
using MB.Common;
using MB.Common.Kendoui;
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
    public class SurveyRepository : ISurveyRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.SurveyRepository");
        public IEnumerable<Survey> GetAll(string surveyContent = null, string code = null, int? surveyType = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@SurveyContent", surveyContent, DbType.String);
                param.Add("@Code", code, DbType.String);
                param.Add("@SurveyType", surveyType, DbType.Int32);
                param.Add("@Visiable", visiable, DbType.Boolean);
                var r = DALHelpers.Query<Survey>("telesales_GetAllSurvey @PageSize,@Page,@SurveyContent,@Code,@SurveyType,@Visiable", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }

        }

        public DataSourceResult GetSurveyDatasource(DataSourceRequest dsRequest, string surveyContent = null, string code = null, int? surveyType = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@SurveyContent", surveyContent, DbType.String);
                param.Add("@Code", code, DbType.String);
                param.Add("@SurveyType", surveyType, DbType.Int32);
                param.Add("@Visiable", visiable, DbType.Boolean);
                var r = DALHelpers.Query<Survey>("telesales_GetAllSurvey @PageSize,@Page,@SurveyContent,@Code,@SurveyType,@Visiable", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetSurveyDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public int AddNewSurvey(Survey survey)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@SurveyContent", survey.SurveyContent, DbType.String);
                param.Add("@Code", survey.Code, DbType.String);
                param.Add("@SurveyType", survey.SurveyType, DbType.Int32);
                param.Add("@Visiable", survey.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", survey.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Query<int>("telesales_AddNewSurvey @SurveyContent,@Code,@SurveyType,@Visiable,@IsDeleted", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewSurvey, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int UpdateSurvey(Survey survey)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@SurveyID", survey.SurveyID, DbType.Int32);
                param.Add("@SurveyContent", survey.SurveyContent, DbType.String);
                param.Add("@Code", survey.Code, DbType.String);
                param.Add("@SurveyType", survey.SurveyType, DbType.Int32);
                param.Add("@Visiable", survey.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", survey.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateSurvey", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error UpdateSurvey, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteSurvey(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@SurveyID", id, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteSurvey", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteSurvey, Detail: " + ex.ToString());
                return 0;
            }
        }

        public SurveyDTO GetSurvey(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Int32);
                var r = DALHelpers.Query<SurveyDTO>("telesales_GetSurveyById @Id", new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetSurvey, Detail: " + ex.ToString());
                return null;
            }
        }

        public IEnumerable<SurveyDTO> GetSurveyByQuestionId(int projectID, int questionID, string codeSurveys = null, string survey = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@QuestionID", questionID, DbType.Int32);
                param.Add("@CodeSurveys", codeSurveys, DbType.String);
                param.Add("@Survey", survey, DbType.String);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                var r = DALHelpers.Query<SurveyDTO>("telesales_SurveyGetAllForQuestions @ProjectID,@QuestionID,@CodeSurveys,@Survey,@PageSize,@Page", new UserLogin(), param).ToList();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetSurveyByQuestionId, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<SurveyDTO> GetSurveyByQuestionIdTK(int projectID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                var r = DALHelpers.Query<SurveyDTO>("telesales_SurveyGetAllForQuestionsTKCT @ProjectID", new UserLogin(), param).ToList();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetSurveyByQuestionIdTK, Detail: " + ex.ToString());
                return null;
            }
        }
        public DataSourceResult GetAllForQuestionsDatasource(DataSourceRequest dsRequest, int projectID, int questionID, string codeSurveys = null, string survey = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@QuestionID", questionID, DbType.Int32);
                param.Add("@CodeSurveys", codeSurveys, DbType.String);
                param.Add("@Survey", survey, DbType.String);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<SurveyDTO>("telesales_SurveyGetAllForQuestions @ProjectID,@QuestionID,@CodeSurveys,@Survey,@PageSize,@Page", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllForQuestionsDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetAllNotInQuestionsSurveyDatasource(DataSourceRequest dsRequest, int projectID, int questionID, string codeSurveys = null, string survey = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@QuestionID", questionID, DbType.Int32);
                param.Add("@CodeSurveys", codeSurveys, DbType.String);
                param.Add("@Survey", survey, DbType.String);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<Survey>("telesales_SurveyGetAllNotInQuestionsSurvey @ProjectID,@QuestionID,@CodeSurveys,@Survey,@PageSize,@Page", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllNotInQuestionsSurveyDatasource, Detail: " + ex.ToString());
                return null;
            }
        }
    }
}
