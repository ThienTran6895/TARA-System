using MB.Web.Core.Validations;

namespace MB.OMS.Account.Domain.Model
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public int UserRole { get; set; }

        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int Total { get; set; }
    }
}
