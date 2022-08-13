using System.Collections.Generic;
using System.Web.Security.AntiXss;

namespace MB.Web.Core.Controls
{
    public class MBButton : MBControlBase
    {
        public MBButton(string controlId, string value, IDictionary<string, object> htmlAttributes, string classDefault, MBInnerIcon innerIcon = null)
            : base("button")
        {
            SetProperty("type", "button");
            this.Name = controlId;
            this.ID = controlId;
            HtmlAttributes = htmlAttributes;
            SetProperty("value", value);
            if (innerIcon != null)
            {
                if (!string.IsNullOrEmpty(innerIcon.Icon))
                {
                    if (string.IsNullOrEmpty(value))
                    {
                        InnerHtml = "<span class='" + innerIcon.Icon + "'></span>";
                    }
                    else
                    {
                        if (innerIcon.IsBefore == false)
                        {
                            InnerHtml = value + "<span class='" + innerIcon.Icon + "'></span>";
                        }
                        else if (innerIcon.IsBefore == true && innerIcon.IsBeforeAfter == true)
                        {
                            InnerHtml = "<span class='" + innerIcon.Icon + "'></span><span>" + value + "</span><span class='" + innerIcon.Icon2 + "'></span>";
                        }
                        else
                        {
                            InnerHtml = "<span class='" + innerIcon.Icon + "'></span><span>" + value + "</span>";
                        }
                    }
                }
            }
            else
            {
                InnerHtml = AntiXssEncoder.HtmlEncode(value, true);
            }

            if (classDefault == null)
            {
                Class = ClassDefault;
            }
            else
            {
                Class = classDefault;
            }
        }

        private static string ClassDefault
        {
            get { return "btn btn-default"; }
        }
    }

    public class MBSubmitButton : MBButton
    {
        public MBSubmitButton(string controlId, string value, IDictionary<string, object> htmlAttributes, string classDefault, MBInnerIcon innerIcon = null)
            : base(controlId, value, htmlAttributes, classDefault, innerIcon)
        {
            SetProperty("type", "submit");
        }
    }
}