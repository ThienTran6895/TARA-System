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
    public class ProjectUsersRepository : IProjectUsersRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.ProjectUsersRepository");
        public IEnumerable<ProjectUsers> GetAll(int? projectID = null, Guid? usersId = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@UsersId", usersId, DbType.Guid);
                var r = DALHelpers.Query<ProjectUsers>("telesales_GetProjectUsers @ProjectID,@UsersId", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }
        }

        public int AddNewProjectUsers(ProjectUsers projectUsers)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectUsers.ProjectID, DbType.Int32);
                param.Add("@UsersId", projectUsers.UserId, DbType.Guid);
                // sondt - 2019 - bo sung cho AUTOCALL
                param.Add("@AgentCode", projectUsers.AgentCode, DbType.Int64);
                var r = DALHelpers.Execute("telesales_AddNewProjectUsers", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewProjectUsers, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteProjectUsers(int projectID, Guid usersId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@UsersId", usersId, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteProjectUsers", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteProjectUsers, Detail: " + ex.ToString());
                return 0;
            }
        }
    }
}
