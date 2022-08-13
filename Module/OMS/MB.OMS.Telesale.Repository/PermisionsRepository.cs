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
    public class PermisionsRepository : IPermisionsRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.PermisionsRepository");
        public IEnumerable<PermisionsDTO> GetPermisionsByParent(int? parent = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Parent", parent, DbType.Int32);
                var r = DALHelpers.Query<PermisionsDTO>("telesales_GetPermisionsByParent @Parent", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetPermisionsByParent, Detail: " + ex.ToString());
                return null;
            }
        }

        public PermisionsDTO GetPermisionsById(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Int32);
                var r = DALHelpers.Query<PermisionsDTO>("telesales_GetPermisionsById @Id",
                    new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetPermisionsById, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetAllPermisionsInRolesByParentDatasource(DataSourceRequest dsRequest, int roleID, string permisionName = null, bool? isVisible = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@RoleID", roleID, DbType.Int32);
                param.Add("@PermisionName", permisionName, DbType.String);
                param.Add("@IsVisible", isVisible, DbType.Boolean);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<Permisions>("telesales_GetAllPermisionsInRolesByParent @RoleID,@PermisionName,@IsVisible,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllPermisionsInRolesByParentDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetAllPermisionsNotInRolesByParentDatasource(DataSourceRequest dsRequest, int roleID, string permisionName = null, bool? isVisible = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@RoleID", roleID, DbType.Int32);
                param.Add("@PermisionName", permisionName, DbType.String);
                param.Add("@IsVisible", isVisible, DbType.Boolean);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<Permisions>("telesales_GetAllPermisionsNotInRolesByParent @RoleID,@PermisionName,@IsVisible,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllPermisionsNotInRolesByParentDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public IEnumerable<PermisionsDTO> GetAllPermisionsInRolesByParent(int roleID, string permisionName = null, bool? isVisible = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@RoleID", roleID, DbType.Int32);
                param.Add("@PermisionName", permisionName, DbType.String);
                param.Add("@IsVisible", isVisible, DbType.Boolean);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                var r = DALHelpers.Query<PermisionsDTO>("telesales_GetAllPermisionsInRolesByParent @RoleID,@PermisionName,@IsVisible,@PageSize,@Page", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllPermisionsInRolesByParent, Detail: " + ex.ToString());
                return null;
            }
        }
    }
}
