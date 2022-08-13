using MB.Common.Helpers;
using MB.OMS.Telesale.Domain.Interface;
using MB.Web.Core;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Controller
{
    public class HomeController : MBController
    {
        [Dependency]
        public IEwayAPIService ewayAPIService { get; set; }

        [Authorize]
        public ActionResult Index()
        {
            /*
            // Send information to Eway and only if this is EWAY PROJECT
            var fields = new NameValueCollection();
            fields.Add("click_id", "5a20fc833a787d004116f05a");
            fields.Add("offer_id", "tinhhaubien-test");
            fields.Add("advertiser_offer_id", "4");
            fields.Add("transaction_id", "9688");
            fields.Add("status_message", "");
            if (CommonHelper.CurrentProject().ToString() == ConfigurationManager.AppSettings["EWayProjectID"])
                ewayAPIService.EwayExportData(fields,1);

            // Sending cancaelation message
            fields["status_message"] = "Hủy vì lý do test";
            if (CommonHelper.CurrentProject().ToString() == ConfigurationManager.AppSettings["EWayProjectID"])
                ewayAPIService.EwayExportData(fields, -1);
            // End Eway
            */

            return RedirectToAction("Login", "User", new { area = "Account" });
        }
    }
}
