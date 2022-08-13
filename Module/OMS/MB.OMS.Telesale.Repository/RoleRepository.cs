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
    public class RoleRepository : IRoleRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.RoleRepository");
        public IEnumerable<Roles> GetAll(string roleName = null, bool? isVisible = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@RoleName", roleName, DbType.String);
                param.Add("@IsVisible", isVisible, DbType.Boolean);
                var r = DALHelpers.Query<Roles>("telesales_GetAllRoles @PageSize,@Page,@RoleName,@IsVisible", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetRoleDatasource(DataSourceRequest dsRequest, string roleName = null, bool? isVisible = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@RoleName", roleName, DbType.String);
                param.Add("@IsVisible", isVisible, DbType.Boolean);
                var r = DALHelpers.Query<Roles>("telesales_GetAllRoles @PageSize,@Page,@RoleName,@IsVisible", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetRoleDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public RoleDTO GetRoleById(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Int32);
                var r = DALHelpers.Query<RoleDTO>("telesales_GetRoleById @Id", new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetRoleById, Detail: " + ex.ToString());
                return null;
            }
        }

        public int AddNewRole(Roles role)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@RoleName", role.RoleName, DbType.String);
                param.Add("@IsVisible", role.IsVisible, DbType.Boolean);
                var r = DALHelpers.Query<int>("telesales_AddNewRole @RoleName,@IsVisible", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewRole, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int UpdateRole(Roles role)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", role.Id, DbType.Int32);
                param.Add("@RoleName", role.RoleName, DbType.String);
                param.Add("@IsVisible", role.IsVisible, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateRole", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error UpdateRole, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteRole(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteRole", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteRole, Detail: " + ex.ToString());
                return 0;
            }
        }
    }
}
