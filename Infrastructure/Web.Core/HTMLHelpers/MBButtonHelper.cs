using System;
using System.Web.Mvc;
using MB.Web.Core.Controls;

namespace MB.Web.Core.HTMLHelpers
{
    public static partial class MBControlExtensions
    {
        public static MvcHtmlString MBButton(this HtmlHelper htmlHelper, string controlId, string value, object htmlAttributes = null, string classDefault = null, bool enable = true, MBInnerIcon innerIcon = null)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            MBButton mvcControl = new MBButton(controlId, value, attributes, classDefault, innerIcon);
            if (!enable)
            {
                attributes.Add("disabled", "disabled");
            }
            if (mvcControl == null)
            {
                throw new ArgumentNullException("MBButton");
            }

            return mvcControl.Html(htmlHelper.ViewContext);
        }

        public static MvcHtmlString MBSubmitButton(this HtmlHelper htmlHelper, string controlId, string value, object htmlAttributes = null, string classDefault = null, bool enable = true, MBInnerIcon innerIcon = null)
        {
            var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
            if (!enable)
            {
                attributes.Add("disabled", "disabled");
            }
            MBSubmitButton mvcControl = new MBSubmitButton(controlId, value, attributes, classDefault, innerIcon);
            if (mvcControl == null)
            {
                throw new ArgumentNullException("mvcControl", "MBSubmitButton");
            }

            return mvcControl.Html(htmlHelper.ViewContext);
        }
    }
}
