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
    public class RolePermisionRepository : IRolePermisionRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.RolePermisionRepository");
        public IEnumerable<RolePermision> GetAll(int? roleId = null, int? permisionId = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@RoleId", roleId, DbType.Int32);
                param.Add("@PermisionId", permisionId, DbType.Int32);
                var r = DALHelpers.Query<RolePermision>("telesales_GetRolePermision @RoleId,@PermisionId", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }
        }

        public int AddNewRolePermision(RolePermision rolePermision)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@RoleId", rolePermision.RoleId, DbType.Int32);
                param.Add("@PermisionId", rolePermision.PermisionId, DbType.Int32);
                var r = DALHelpers.Execute("telesales_AddNewRolePermision", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewRolePermision, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteRolePermision(int roleId, int permisionId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@RoleId", roleId, DbType.Int32);
                param.Add("@PermisionId", permisionId, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteRolePermision", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteRolePermision, Detail: " + ex.ToString());
                return 0;
            }
        }
    }
}
