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
    public class ProjectCustomerFieldRepository : IProjectCustomerFieldRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.ProjectCustomerFieldRepository");
        public IEnumerable<ProjectCustomerField> GetAll(int? projectID = null, int? customerFieldID = null, bool? isActive = null, bool? isEdit = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@CustomerFieldID", customerFieldID, DbType.Int32);
                param.Add("@IsActive", isActive, DbType.Boolean);
                param.Add("@IsEdit", isEdit, DbType.Boolean);
                var r = DALHelpers.Query<ProjectCustomerField>("telesales_GetProjectCustomerField @PageSize,@Page,@ProjectID,@CustomerFieldID,@IsActive,@IsEdit", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }
        }
        public DataSourceResult GetProjectCustomerFieldDatasource(DataSourceRequest dsRequest, int? projectID = null, int? customerFieldID = null, bool? isActive = null, bool? isEdit = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@CustomerFieldID", customerFieldID, DbType.Int32);
                param.Add("@IsActive", isActive, DbType.Boolean);
                param.Add("@IsEdit", isEdit, DbType.Boolean);
                var r = DALHelpers.Query<ProjectCustomerField>("telesales_GetProjectCustomerField @PageSize,@Page,@ProjectID,@CustomerFieldID,@IsActive,@IsEdit", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetProjectCustomerFieldDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public int AddNewProjectCustomerField(ProjectCustomerField projectCustomerField)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectCustomerField.ProjectID, DbType.Int32);
                param.Add("@CustomerFieldID", projectCustomerField.CustomerFieldID, DbType.Int32);
                param.Add("@IsActive", projectCustomerField.IsActive, DbType.Boolean);
                param.Add("@IsEdit", projectCustomerField.IsEdit, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_AddNewProjectCustomerField", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewProjectCustomerField, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int UpdateProjectCustomerField(ProjectCustomerField projectCustomerField)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectCustomerField.ProjectID, DbType.Int32);
                param.Add("@CustomerFieldID", projectCustomerField.CustomerFieldID, DbType.Int32);
                param.Add("@IsActive", projectCustomerField.IsActive, DbType.Boolean);
                param.Add("@IsEdit", projectCustomerField.IsEdit, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateProjectCustomerField", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error UpdateProjectCustomerField, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteProjectCustomerField(int? projectID = null, int? customerFieldID = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@CustomerFieldID", customerFieldID, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteProjectCustomerField", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteProjectCustomerField, Detail: " + ex.ToString());
                return 0;
            }
        }
    }
}
