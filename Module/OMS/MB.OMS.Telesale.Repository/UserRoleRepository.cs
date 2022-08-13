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
    public class UserRoleRepository : IUserRoleRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.UserRoleRepository");
        public IEnumerable<UserRole> GetAll(Guid? userIds = null, int? roleId = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserIds", userIds, DbType.Guid);
                param.Add("@RoleId", roleId, DbType.Int32);
                var r = DALHelpers.Query<UserRole>("telesales_GetUserRole @UserIds,@RoleId", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }
        }

        public int AddNewUserRole(UserRole userRole)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserIds", userRole.UserId, DbType.Guid);
                param.Add("@RoleId", userRole.RoleId, DbType.Int32);
                var r = DALHelpers.Execute("telesales_AddNewUserRole", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewUserRole, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteUserRole(Guid userIds, int roleId)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UserIds", userIds, DbType.Guid);
                param.Add("@RoleId", roleId, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteUserRole", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteUserRole, Detail: " + ex.ToString());
                return 0;
            }
        }
    }
}
