using MB.Web.Core.Validations;

namespace MB.OMS.Account.Domain.Model
{
    public class UserDTO : User
    {
        public string FullName
        {
            get { return FirstName + " " + LastName; }
        }
    }
}
