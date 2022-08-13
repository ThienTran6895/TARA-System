using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace MB.Common
{
    public class UserLogin
    {
        public UserLogin()
        {
            UserId = "395DE479-4E2F-44F2-BC2D-822B1D582EE5";
            //UserId = HttpContext.Current.User.Identity.GetUserId();
            AppId = 1;
        }

        public string UserId { get; set; }
        public int AppId { get; set; }
    }
}
