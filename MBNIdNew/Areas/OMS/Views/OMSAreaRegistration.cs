using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMSTeleSale.Areas.OMS.Views
{
    public class OMSAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "OMS";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "OMS_default",
                "OMS/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                 new string[] { "MB.OMS.Telesale.Controller" }
            );

           // context.MapRoute(
           //    "OMS_LandingPage",
           //    "{controller}/{action}/{id}",
           //    new { controller= "Home",action = "Index", id = UrlParameter.Optional },
           //     new string[] { "MB.OMS.Telesale.Controller" }
           //);
        }
    }
}