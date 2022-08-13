using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IUserRepository
    {
        IEnumerable<UserDTO> GetAll(string userName = null, string fullName = null, string email = null, string phone = null, bool? visible = null);
        DataSourceResult GetUserDatasource(DataSourceRequest dsRequest, string userName = null, string fullName = null, string email = null, string phone = null, bool? visible = null);
        DataSourceResult GetUserByProjectDatasource(DataSourceRequest dsRequest, int? projectID = null, string userName = null, string fullName = null);
        DataSourceResult GetUserNotInProjectDatasource(DataSourceRequest dsRequest, int? projectID = null, string userName = null, string fullName = null);
        IEnumerable<User> GetUserByProject(int? projectID = null, string userName = null, string fullName = null);
        Guid AddNewUser(User user);
        int UpdateUser(User user);
        int DeleteUser(Guid id);
        UserDTO GetUserById(Guid id);
        IEnumerable<User> CheckUserByUsername(string username);
        IEnumerable<Projects> GetProjectByUser(Guid? usersId = null);
    }
}
