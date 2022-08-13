using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace MB.Common.Helpers
{
    public class CookieStore
    {
        public static void SetCookie(string key, string value)
        {
            if (HttpContext.Current.Request.Cookies[key] != null)
            {
                var cookieOld = HttpContext.Current.Request.Cookies[key];
                cookieOld.Value = value;
                HttpContext.Current.Response.Cookies.Add(cookieOld);
            }
            else
            {
                HttpCookie cookie = new HttpCookie(key);
                cookie.Value = value;
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        public static string GetCookie(string key)
        {
            string value = string.Empty;
            HttpCookie cookie = HttpContext.Current.Request.Cookies[key];

            if (cookie != null)
            {
                value = cookie.Value;
            }
            return value;
        }
    }
}
