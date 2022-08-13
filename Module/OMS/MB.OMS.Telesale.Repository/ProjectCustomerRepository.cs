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
    public class ProjectCustomerRepository : IProjectCustomerRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.ProjectCustomerRepository");
        public IEnumerable<ProjectCustomer> GetProjectCustomer(int? projectID = null, Guid? customerID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                var r = DALHelpers.Query<ProjectCustomer>("telesales_GetProjectCustomer @ProjectID,@CustomerID", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetProjectCustomer, Detail: " + ex.ToString());
                return null;
            }
        }

        public int AddNewProjectCustomer(ProjectCustomer projectCustomer)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectCustomer.ProjectID, DbType.Int32);
                param.Add("@CustomerID", projectCustomer.CustomerID, DbType.Guid);
                param.Add("@CallBy", projectCustomer.CallBy, DbType.Guid);
                param.Add("@IsCall", projectCustomer.IsCall, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_AddNewProjectCustomer", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewProjectCustomer, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int UpdateProjectCustomer(ProjectCustomer projectCustomer)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectCustomer.ProjectID, DbType.Int32);
                param.Add("@CustomerID", projectCustomer.CustomerID, DbType.Guid);
                param.Add("@CallBy", projectCustomer.CallBy, DbType.Guid);
                param.Add("@UpdateDate", projectCustomer.UpdatedDate, DbType.DateTime);
                param.Add("@IsCall", projectCustomer.IsCall, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateProjectCustomer", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error UpdateProjectCustomer, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteProjectCustomer(int projectID, Guid customerID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteProjectCustomer", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteProjectCustomer, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteProjectCustomerNew( Guid customerID, int? projectID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@CustomerID", customerID, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteProjectCustomerNew", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteProjectCustomerNew, Detail: " + ex.ToString());
                return 0;
            }
        }

       
    }
}
