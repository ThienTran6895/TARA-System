using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MB.Web.Core.HTMLHelpers
{
    public static partial class MBControlExtensions
    {
        public static MvcForm MBForm(this HtmlHelper html, string action, string controller, IDictionary<string, object> htmlAttributes = null)
        {
            if (htmlAttributes == null)
                htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.Add("data-role", "mbform");
            var form = html.BeginForm(action, controller, FormMethod.Post, htmlAttributes);
            html.ViewContext.Writer.Write(html.AntiForgeryToken().ToHtmlString());
            return form;
        }

    }
}
