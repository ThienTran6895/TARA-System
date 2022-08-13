using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.OMS.Account.Domain.Model;

namespace MB.OMS.Account.Domain.Interface
{
    public interface IUserService
    {
        List<User> GetAllActiveUsers();
    }
}
