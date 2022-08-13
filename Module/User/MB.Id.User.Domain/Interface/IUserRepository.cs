using System.Collections.Generic;
using MB.Common.Kendoui;
using MB.OMS.Account.Domain.Model;
using System;

namespace MB.OMS.Account.Domain.Interface
{
    public interface IUserRepository
    {
        IEnumerable<ApplicationUser> GetAll();
        ApplicationUser GetUser(string userName);
        DataSourceResult GetUserDatasource(DataSourceRequest dsRequest);
        int AddNewUser(ApplicationUser user);
        IEnumerable<Permisions> GetAllPermissionForUserID(Guid userIds);
    }
}
