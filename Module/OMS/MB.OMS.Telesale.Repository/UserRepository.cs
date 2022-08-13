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
    public class UserRepository : IUserRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.UserRepository");
        public IEnumerable<UserDTO> GetAll(string userName = null, string fullName = null, string email = null, string phone = null, bool? visible = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@UserName", userName, DbType.String);
                param.Add("@FullName", fullName, DbType.String);
                param.Add("@Email", email, DbType.String);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@Visible", visible, DbType.Boolean);
                var r = DALHelpers.Query<UserDTO>("telesales_GetAllUser @PageSize,@Page,@UserName,@FullName,@Email,@Phone,@Visible", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetUserDatasource(DataSourceRequest dsRequest, string userName = null, string fullName = null, string email = null, string phone = null, bool? visible = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@UserName", userName, DbType.String);
                param.Add("@FullName", fullName, DbType.String);
                param.Add("@Email", email, DbType.String);
                param.Add("@Phone", phone, DbType.String);
                param.Add("@Visible", visible, DbType.Boolean);
                var r = DALHelpers.Query<UserDTO>("telesales_GetAllUser @PageSize,@Page,@UserName,@FullName,@Email,@Phone,@Visible", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetUserDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetUserByProjectDatasource(DataSourceRequest dsRequest, int? projectID = null, string userName = null, string fullName = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@UserName", userName, DbType.String);
                param.Add("@FullName", fullName, DbType.String);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<UserDTO>("telesales_ProjectGetAllUser @ProjectID,@UserName,@FullName,@PageSize,@Page", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetUserByProjectDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetUserNotInProjectDatasource(DataSourceRequest dsRequest, int? projectID = null, string userName = null, string fullName = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@UserName", userName, DbType.String);
                param.Add("@FullName", fullName, DbType.String);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                var r = DALHelpers.Query<UserDTO>("telesales_ProjectGetAllUserNotInProject @ProjectID,@UserName,@FullName,@PageSize,@Page", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetUserNotInProjectDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public IEnumerable<User> GetUserByProject(int? projectID = null, string userName = null, string fullName = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@UserName", userName, DbType.String);
                param.Add("@FullName", fullName, DbType.String);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                var r = DALHelpers.Query<UserDTO>("telesales_ProjectGetAllUser @ProjectID,@UserName,@FullName,@PageSize,@Page", new UserLogin(), param).ToList();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetUserByProject, Detail: " + ex.ToString());
                return null;
            }
        }

        public Guid AddNewUser(User user)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", Guid.NewGuid(), DbType.Guid);
                param.Add("@UserName", user.UserName, DbType.String);
                param.Add("@Password", user.Password, DbType.String);
                param.Add("@FirstName", user.FirstName, DbType.String);
                param.Add("@LastName", user.LastName, DbType.String);
                param.Add("@Gender", user.Gender, DbType.Boolean);
                param.Add("@BirthDay", user.BirthDay, DbType.DateTime);
                param.Add("@Address", user.Address, DbType.String);
                param.Add("@Email", user.Email, DbType.String);
                param.Add("@Phone", user.Phone, DbType.String);
                param.Add("@ImageUrl", user.ImageUrl, DbType.String);
                param.Add("@Visible", user.Visible, DbType.Boolean);
                param.Add("@IsDeleted", user.IsDelete, DbType.Boolean);
                var r = DALHelpers.Query<Guid>("telesales_AddNewUser @Id,@UserName,@Password,@FirstName,@LastName,@Gender,@BirthDay,@Address,@Email,@Phone,@ImageUrl,@Visible,@IsDeleted", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewUser, Detail: " + ex.ToString());
                return Guid.Empty;
            }
        }

        public int UpdateUser(User user)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", user.Id, DbType.Guid);
                param.Add("@UserName", user.UserName, DbType.String);
                param.Add("@Password", user.Password, DbType.String);
                param.Add("@FirstName", user.FirstName, DbType.String);
                param.Add("@LastName", user.LastName, DbType.String);
                param.Add("@Gender", user.Gender, DbType.Boolean);
                param.Add("@BirthDay", user.BirthDay, DbType.DateTime);
                param.Add("@Address", user.Address, DbType.String);
                param.Add("@Email", user.Email, DbType.String);
                param.Add("@Phone", user.Phone, DbType.String);
                param.Add("@ImageUrl", user.ImageUrl, DbType.String);
                param.Add("@Visible", user.Visible, DbType.Boolean);
                param.Add("@IsDeleted", user.IsDelete, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateUser", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error UpdateUser, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int DeleteUser(Guid id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Guid);
                var r = DALHelpers.Execute("telesales_DeleteUser", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteUser, Detail: " + ex.ToString());
                return 0;
            }
        }

        public UserDTO GetUserById(Guid id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Guid);
                var r = DALHelpers.Query<UserDTO>("telesales_GetUserById @Id", new UserLogin(), param).First();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetUserById, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<User> CheckUserByUsername(string username)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Username", null, DbType.Int32);
                var r = DALHelpers.Query<User>("telesales_CheckUserByUsername @Username", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<Projects> GetProjectByUser(Guid? usersId = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@UsersId", usersId, DbType.Guid);
                var r = DALHelpers.Query<Projects>("telesales_GetProjectByUser @UsersId", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetProjectByUser, Detail: " + ex.ToString());
                return null;
            }
        }

    }
}
