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
    public class ProjectStatusCallRepository : IProjectStatusCallRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.ProjectStatusCallRepository");
        public IEnumerable<ProjectStatusCall> GetAll(int? projectID = null, int? statusID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@StatusID", statusID, DbType.Int32);
                var r = DALHelpers.Query<ProjectStatusCall>("telesales_GetProjectStatusCall @ProjectID,@StatusID", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }
        }

        public int AddNewProjectStatusCall(ProjectStatusCall projectStatusCall)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectStatusCall.ProjectID, DbType.Int32);
                param.Add("@StatusCallID", projectStatusCall.StatusCallID, DbType.Int32);
                var r = DALHelpers.Execute("telesales_AddNewProjectStatusCall", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewProjectStatusCall, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteProjectStatusCall(int projectID, int statusCallID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@StatusCallID", statusCallID, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteProjectStatusCall", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteProjectStatusCall, Detail: " + ex.ToString());
                return 0;
            }
        }

    }
}
