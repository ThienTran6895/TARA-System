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
    public class ProjectQuestionsRepository : IProjectQuestionsRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.ProjectQuestionsRepository");
        public IEnumerable<ProjectQuestions> GetAll(int? projectID = null, int? questionID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@QuestionID", questionID, DbType.Int32);
                var r = DALHelpers.Query<ProjectQuestions>("telesales_GetProjectQuestions @ProjectID,@QuestionID", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<ProjectQuestions> GetAllByProjectID(int? projectID = null, int? questionID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@QuestionID", questionID, DbType.Int32);
                var r = DALHelpers.Query<ProjectQuestions>("telesales_GetProjectQuestionsByID @ProjectID,@QuestionID", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllByProjectID, Detail: " + ex.ToString());
                return null;
            }
        }
        public int AddNewProjectQuestions(ProjectQuestions projectQuestions)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectQuestions.ProjectID, DbType.Int32);
                param.Add("@QuestionID", projectQuestions.QuestionID, DbType.Int32);
                param.Add("@DisplayOrder", projectQuestions.DisplayOrder, DbType.Int32);
                var r = DALHelpers.Execute("telesales_AddNewProjectQuestions", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewProjectQuestions, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int UpdateProjectQuestions(ProjectQuestions projectQuestions)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectQuestions.ProjectID, DbType.Int32);
                param.Add("@QuestionID", projectQuestions.QuestionID, DbType.Int32);
                param.Add("@DisplayOrder", projectQuestions.DisplayOrder, DbType.Int32);
                var r = DALHelpers.Execute("telesales_UpdateProjectQuestions", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error UpdateProjectQuestions, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteProjectQuestions(int projectID, int questionID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@QuestionID", questionID, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteProjectQuestions", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteProjectQuestions, Detail: " + ex.ToString());
                return 0;
            }
        }
    }
}
