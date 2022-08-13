using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Core;
using MB.OMS.Telesale.Domain.Model;
using MB.OMS.Telesale.Domain.Interface;
using MB.Common;
using MB.Common.Kendoui;
using Dapper;
using System.Data;

namespace MB.OMS.Telesale.Repository
{
    public class QuestionRepository : IQuestionRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.QuestionRepository");
        public IEnumerable<Question> GetAll(string name = null, string code = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@Name", name, DbType.String);
                param.Add("@Code", code, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                var r = DALHelpers.Query<Question>("telesales_GetAllQuestions @PageSize,@Page,@Name,@Code,@Visiable", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetQuestionDatasource(DataSourceRequest dsRequest, string name = null, string code = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@Name", name, DbType.String);
                param.Add("@Code", code, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                var r = DALHelpers.Query<Question>("telesales_GetAllQuestions @PageSize,@Page,@Name,@Code,@Visiable", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetQuestionDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public int AddNewQuestion(Question question)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Code", question.Code, DbType.String);
                param.Add("@Name", question.Name, DbType.String);
                param.Add("@Visiable", question.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", question.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Query<int>("telesales_AddNewQuestion @Code,@Name,@Visiable,@IsDeleted", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewQuestion, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int UpdateQuestion(Question question)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@QuestionID", question.QuestionID, DbType.Int32);
                param.Add("@Code", question.Code, DbType.String);
                param.Add("@Name", question.Name, DbType.String);
                param.Add("@Visiable", question.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", question.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateQuestion", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error UpdateQuestion, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteQuestion(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@QuestionID", id, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteQuestion", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteQuestion, Detail: " + ex.ToString());
                return 0;
            }
        }

        public QuestionDTO GetQuestionById(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Int32);
                var r = DALHelpers.Query<QuestionDTO>("telesales_GetQuestionById @Id", new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetQuestionById, Detail: " + ex.ToString());
                return null;
            }
        }

        public IEnumerable<QuestionDTO> GetQuestionByProjectId(int? projectId = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectId, DbType.Int32);
                param.Add("@Visiable", visiable, DbType.Boolean);
                var r = DALHelpers.Query<QuestionDTO>("telesales_GetAllQuestionsByProject @ProjectID,@Visiable", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetQuestionByProjectId, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetQuestionForProjectDatasource(DataSourceRequest dsRequest, int projectID, string name = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@Name", name, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<QuestionDTO>("telesales_GetAllQuestionForProject @ProjectID,@Name,@Visiable,@PageSize,@Page", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetQuestionForProjectDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetQuestionNotInForProjectDatasource(DataSourceRequest dsRequest, int projectID, string name = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@Name", name, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<Question>("telesales_GetAllQuestionNotInForProject @ProjectID,@Name,@Visiable,@PageSize,@Page", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetQuestionNotInForProjectDatasource, Detail: " + ex.ToString());
                return null;
            }
        }
    }
}
