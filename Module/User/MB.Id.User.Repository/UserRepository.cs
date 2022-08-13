using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DAL.Core;
using MB.Common;
using MB.Common.Kendoui;
using MB.OMS.Account.Domain.Interface;
using MB.OMS.Account.Domain.Model;
using Microsoft.AspNet.Identity;

namespace MB.OMS.Account.Repository
{
    public class UserRepository : IUserRepository
    {
        public IEnumerable<ApplicationUser> GetAll()
        {
            var r = DALHelpers.Query<ApplicationUser>("spa_GetAllUsers", new UserLogin() { UserId = "2978C3E7-B198-47EF-BDF7-A6DF5FEEB7B0", AppId = 1 }).ToList();
            return r;

        }
        public DataSourceResult GetUserDatasource(DataSourceRequest dsRequest)
        {

            var param = new DynamicParameters();
            param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
            param.Add("@Page", dsRequest.Page, DbType.Int32);
            var r = DALHelpers.Query<ApplicationUser>("spa_GetAllUsers @PageSize,@Page", new UserLogin() { UserId = "2978C3E7-B198-47EF-BDF7-A6DF5FEEB7B0", AppId = 1 }, param).AsEnumerable();

            return new DataSourceResult() { Data = r, Total = r.First().Total };

        }

        public int AddNewUser(ApplicationUser user)
        {
            var param = new DynamicParameters();
            param.Add("@UserName", user.UserName, DbType.String);
            param.Add("@Password", user.Password, DbType.String);
            param.Add("@FirstName", user.FirstName, DbType.String);
            param.Add("@LastName", user.LastName, DbType.String);
            var r = DALHelpers.Execute("spa_AddNewUser",
                new UserLogin() {UserId = "2978C3E7-B198-47EF-BDF7-A6DF5FEEB7B0", AppId = 1}, param);
            return r;
        }

        public ApplicationUser GetUser(string userName)
        {
            var param = new DynamicParameters();
            param.Add("@UserName", userName, DbType.String);
            var r = DALHelpers.Query<ApplicationUser>("telesales_GetUserByUsername @UserName",
                new UserLogin() { UserId = "2978C3E7-B198-47EF-BDF7-A6DF5FEEB7B0", AppId = 1 }, param);
            if (r.Count()>0)
                return r.First();
            return null;
        }

        public IEnumerable<Permisions> GetAllPermissionForUserID(Guid userIds)
        {
            var param = new DynamicParameters();
            param.Add("@UserIds", userIds, DbType.Guid);
            var r = DALHelpers.Query<Permisions>("telesales_GetAllPermissionForUserID @UserIds", new UserLogin(), param).ToList();
            return r;
        }
    }
}
