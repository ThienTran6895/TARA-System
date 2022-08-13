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
    public class ProjectsRepository : IProjectsRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.ProjectsRepository");
        public IEnumerable<Projects> GetAll(int? campaignID = null, string code = null, string name = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@CampaignID", campaignID, DbType.Int32);
                param.Add("@Code", code, DbType.String);
                param.Add("@Name", name, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                var r = DALHelpers.Query<Projects>("telesales_GetAllProject @PageSize,@Page,@CampaignID,@Code,@Name,@Visiable", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetProjectsDatasource(DataSourceRequest dsRequest, int? campaignID = null, string code = null, string name = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@CampaignID", campaignID, DbType.Int32);
                param.Add("@Code", code, DbType.String);
                param.Add("@Name", name, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                var r = DALHelpers.Query<Projects>("telesales_GetAllProject @PageSize,@Page,@CampaignID,@Code,@Name,@Visiable", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetProjectsDatasource, Detail: " + ex.ToString());
                return null;
            }
        }
        public DataSourceResult GetProjectsByUserDatasource(Guid? usersId = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UsersId", usersId, DbType.Guid);
                var r = DALHelpers.Query<Projects>("telesales_GetProjectByUser @UsersId", new UserLogin(), param).ToList();
                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetProjectByUser, Detail: " + ex.ToString());
                return null;
            }
        }

        public int AddNewProjects(Projects projects)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CampaignID", projects.CampaignID, DbType.Int32);
                param.Add("@Code", projects.Code, DbType.String);
                param.Add("@Name", projects.Name, DbType.String);
                param.Add("@Greeting", projects.Greeting, DbType.String);
                param.Add("@Conclusion", projects.Conclusion, DbType.String);
                param.Add("@TotalPlan", projects.TotalPlan, DbType.Int32);
                param.Add("@TotalTarget", projects.TotalTarget, DbType.Int32);
                param.Add("@MonthlyPlan", projects.MonthlyPlan, DbType.Int32);
                param.Add("@MonthlyTarget", projects.MonthlyTarget, DbType.Int32);
                param.Add("@DailyPlan", projects.DailyPlan, DbType.Int32);
                param.Add("@DailyTarget", projects.DailyTarget, DbType.Int32);
                param.Add("@Visiable", projects.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", projects.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Query<int>("telesales_AddNewProject @CampaignID,@Code,@Name,@Greeting,@Conclusion,@TotalPlan,@TotalTarget,@MonthlyPlan,@MonthlyTarget,@DailyPlan,@DailyTarget,@Visiable,@IsDeleted", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewProjects, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int UpdateProjects(Projects projects)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projects.ProjectID, DbType.Int32);
                param.Add("@CampaignID", projects.CampaignID, DbType.Int32);
                param.Add("@Code", projects.Code, DbType.String);
                param.Add("@Name", projects.Name, DbType.String);
                param.Add("@Greeting", projects.Greeting, DbType.String);
                param.Add("@Conclusion", projects.Conclusion, DbType.String);
                param.Add("@TotalPlan", projects.TotalPlan, DbType.Int32);
                param.Add("@TotalTarget", projects.TotalTarget, DbType.Int32);
                param.Add("@MonthlyPlan", projects.MonthlyPlan, DbType.Int32);
                param.Add("@MonthlyTarget", projects.MonthlyTarget, DbType.Int32);
                param.Add("@DailyPlan", projects.DailyPlan, DbType.Int32);
                param.Add("@DailyTarget", projects.DailyTarget, DbType.Int32);
                param.Add("@Visiable", projects.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", projects.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateProject", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error UpdateProjects, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteProjects(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", id, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteProject", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteProjects, Detail: " + ex.ToString());
                return 0;
            }
        }

        public ProjectsDTO GetProjectsById(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Int32);
                var r = DALHelpers.Query<ProjectsDTO>("telesales_GetProjectById @Id",
                    new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetProjectsById, Detail: " + ex.ToString());
                return null;
            }
        }
    }
}
