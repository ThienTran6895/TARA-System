using log4net;
using MB.Common;
using MB.OMS.Telesale.Domain.Interface;
using MB.OMS.Telesale.Domain.Model;
using MB.Web.Core;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MB.Common.Helpers;
using MB.Common.Kendoui;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System.Net;
using System.Web;
using Newtonsoft.Json;

namespace MB.OMS.Telesale.Controller
{

	public class APIController : MBController
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.API");
        #region Fields
        [Dependency]
        public ICampaignsRepository campaignsRepository { get; set; }

        [Dependency]
        public IProjectsRepository projectsRepository { get; set; }

        [Dependency]
        public ISourcesRepository sourcesRepository { get; set; }

        [Dependency]
        public IStatusCallRepository statusCallRepository { get; set; }

        [Dependency]
        public ICustomerRepository customerRepository { get; set; }

        [Dependency]
        public ISurveyRepository surveyRepository { get; set; }

        [Dependency]
        public ICustomerFieldRepository customerFieldRepository { get; set; }

        [Dependency]
        public ICustomerFieldValueRepository customerFieldValueRepository { get; set; }

        [Dependency]
        public ICustomerExistRepository customerExistRepository { get; set; }

        [Dependency]
        public ICustomerExistFieldValueRepository customerExistFieldValueRepository { get; set; }

        [Dependency]
        public ICustomerErrorRepository customerErrorRepository { get; set; }

        [Dependency]
        public ICustomerErrorFieldValueRepository customerErrorFieldValueRepository { get; set; }
        #endregion

        [AllowAnonymous]
        public ActionResult PullCustomerData()
		{
			// Parsing request data
			try
			{
                var request = new EwayCustomerModel();
                request.accesstoken = Request.QueryString["accesstoken"];
                request.projectId = Request.QueryString["projectid"];
                request.phone = Request.QueryString["phone"];
                request.name = HttpUtility.HtmlEncode(Request.QueryString["name"]);
                request.product_id = Request.QueryString["product_id"];
                request.click_id = Request.QueryString["click_id"];
                request.transaction_id = Request.QueryString["transaction_id"];
                request.contact_time = Request.QueryString["contact_time"];
                request.offer_id = Request.QueryString["offer_id"];

                logger.Info(string.Format("API call at {0}",JsonConvert.SerializeObject(request)));

                // Validate the secret key & project id
                var secretKeys = ConfigurationManager.AppSettings["SecretKeys"].Split(';');
                var project = projectsRepository.GetProjectsById(int.Parse(request.projectId)) != null;
                if (!secretKeys.Contains(request.accesstoken) || !project)
                {
                    logger.Warn(string.Format("API forbiden access at, orignal request data {0}\n", JsonConvert.SerializeObject(request)));
                    return Json(new 
                    {
                        Result = "0",
                        Message = "Failed: Unable to add new customer"
                    }, JsonRequestBehavior.AllowGet);
                }

                // Pull the data and return
                if (customerRepository.GetAll(mobilePhone: request.phone).FirstOrDefault() == null)
                {
                    // Insert Customer
                    var model = new Customer();
                    model.CustomerID = Guid.NewGuid();
                    model.SourceID = Convert.ToInt32(ConfigurationManager.AppSettings["EWaySourceID"]);
                    model.MobilePhone = request.phone;
                    model.Visiable = true;
                    model.IsDeleted = false;
                    var result = customerRepository.AddNewCustomer(model);

                    if (result == Guid.Empty)
                    {
                        logger.Fatal(string.Format("Unable to add {0}: {1}", request.name, request.phone));
                        return Json(new 
                        {
                            Result = "0",
                            Message = "Failed: Unable to add new customer"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    logger.Info(string.Format("Customer created {0}\n", JsonConvert.SerializeObject(request)));

                    //Insert CustomerFieldValue

                    // Fullname - 7
                    var customerFieldValue = new CustomerFieldValue();
                    customerFieldValue.CustomerID = result;
                    customerFieldValue.CustomerFieldID = 7;
                    customerFieldValue.FieldValue = HttpUtility.UrlDecode(request.name);
                    customerFieldValueRepository.AddNewCustomerFieldValue(customerFieldValue);

                    // Phone - 4
                    customerFieldValue = new CustomerFieldValue();
                    customerFieldValue.CustomerID = result;
                    customerFieldValue.CustomerFieldID = 4;
                    customerFieldValue.FieldValue = HttpUtility.UrlDecode(request.phone);
                    customerFieldValueRepository.AddNewCustomerFieldValue(customerFieldValue);

                    // 2092  - offer_id - Sản phẩm
                    customerFieldValue = new CustomerFieldValue();
                    customerFieldValue.CustomerID = result;
                    customerFieldValue.CustomerFieldID = 2092;
                    customerFieldValue.FieldValue = HttpUtility.UrlDecode(request.offer_id);
                    customerFieldValueRepository.AddNewCustomerFieldValue(customerFieldValue);

                    // 2093  -  product_id - Mã sản phẩm
                    customerFieldValue = new CustomerFieldValue();
                    customerFieldValue.CustomerID = result;
                    customerFieldValue.CustomerFieldID = 2093;
                    customerFieldValue.FieldValue = HttpUtility.UrlDecode(request.product_id);
                    customerFieldValueRepository.AddNewCustomerFieldValue(customerFieldValue);

                    // 2094  -  click_id - click_id
                    customerFieldValue = new CustomerFieldValue();
                    customerFieldValue.CustomerID = result;
                    customerFieldValue.CustomerFieldID = 2094;
                    customerFieldValue.FieldValue = request.click_id;
                    customerFieldValueRepository.AddNewCustomerFieldValue(customerFieldValue);

                    // 2095  -  transaction_id - transaction_id
                    customerFieldValue = new CustomerFieldValue();
                    customerFieldValue.CustomerID = result;
                    customerFieldValue.CustomerFieldID = 2095;
                    customerFieldValue.FieldValue = request.transaction_id;
                    customerFieldValueRepository.AddNewCustomerFieldValue(customerFieldValue);

                    // 2096  -  contact_time - Thời gian đặt hàng
                    customerFieldValue = new CustomerFieldValue();
                    customerFieldValue.CustomerID = result;
                    customerFieldValue.CustomerFieldID = 2096;
                    customerFieldValue.FieldValue = request.contact_time;
                    customerFieldValueRepository.AddNewCustomerFieldValue(customerFieldValue);

                    logger.Info(string.Format("Customer fields created {0}\n", JsonConvert.SerializeObject(request)));

                    return Json(new
                    {
                        Result = "1",
                        Message = "Success: Customer " + request.phone + " added"
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new 
                    {
                        Result = "0",
                        Message = "Failed: Phone number already exits"
                    }, JsonRequestBehavior.AllowGet);
                }

                
			}
			catch (Exception ex)
			{
				logger.Fatal(string.Format("API exception at {0}: \n{1}", ex.Message , ex.StackTrace));
                return Json(new 
                {
                    Result = "0",
                    Message =  ex.Message
                }, JsonRequestBehavior.AllowGet);
            }

		}
	}
}
