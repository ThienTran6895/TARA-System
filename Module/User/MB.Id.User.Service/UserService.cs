using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.OMS.Account.Domain.Interface;
using MB.OMS.Account.Domain.Model;

namespace MB.OMS.Account.Service
{
    public class UserService : IUserService
    {

        public IUserRepository userRepository { get; set; }
        public List<User> GetAllActiveUsers()
        {
            throw new NotImplementedException();
        }
    }
}
