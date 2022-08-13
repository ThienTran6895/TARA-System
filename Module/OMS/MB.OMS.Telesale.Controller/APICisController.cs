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
using System.IO;

namespace MB.OMS.Telesale.Controller
{

	public partial class APIController : MBController
    {
        [AllowAnonymous]
        [HttpPost]
        public ActionResult CISResult()
		{
			// Parsing request data
			try
			{
                Request.InputStream.Seek(0, SeekOrigin.Begin);
                string jsonData = new StreamReader(Request.InputStream).ReadToEnd();

                CisCallResult result = JsonConvert.DeserializeObject<CisCallResult>(jsonData);
                var receivedData = JsonConvert.SerializeObject(result);
                string errorMsg = string.Format("Unknown error for request \n{0}", receivedData);
                logger.Info(string.Format("CIS API call at {0}", receivedData));

                // Validate the secret key 
                var secretKeys = ConfigurationManager.AppSettings["SecretKeys"].Split(';');
                if (!secretKeys.Contains(result.secretkey))
                {
                    errorMsg = string.Format("Authentification failed, data received:\n{0}", receivedData);
                    logger.Error(errorMsg);
                    return Json(new
                    {
                        Result = 0,
                        Message = errorMsg
                    }, JsonRequestBehavior.AllowGet);
                }

                // Verify if the phone number exists in correct sources & peoject
                var customer = customerRepository.GetCustomer(new Guid(result.customer_id));
                if (null == customer || customer.IsDeleted)
                {
                    errorMsg = string.Format("Customer does NOT exists, data received:\n{0}", receivedData);
                    logger.Error(errorMsg);
                    return Json(new
                    {
                        Result = 0,
                        Message = errorMsg
                    }, JsonRequestBehavior.AllowGet);
                }

                var project = projectsRepository.GetProjectsById(result.project);
                if (null == project || project.IsDeleted)
                {
                    errorMsg = string.Format("Project does NOT exists, data received:\n{0}", receivedData);
                    logger.Error(errorMsg);
                    return Json(new
                    {
                        Result = 0,
                        Message = errorMsg
                    }, JsonRequestBehavior.AllowGet);
                }

                var source = sourcesRepository.GetSourcesById(customer.SourceID);
                if (null == source || !source.Visiable)
                {
                    errorMsg = string.Format("Sources does NOT exists, data received:\n{0}", receivedData);
                    logger.Error(errorMsg);
                    return Json(new
                    {
                        Result = 0,
                        Message = errorMsg
                    }, JsonRequestBehavior.AllowGet);
                }

                if (customer.SourceID != source.SourceID)
                {
                    errorMsg = string.Format("Phone does NOT belong to correct source and project which are running by us, data received:\n{0}", receivedData);
                    logger.Error(errorMsg);
                    return Json(new
                    {
                        Result = "0",
                        Message = errorMsg
                    }, JsonRequestBehavior.AllowGet);
                }


                // Save call log if not answered
                if (result.status != CisCallStatus.ANSWERED)
                {
                    int failedCallStatusId = 1005;
                    var callStatus = statusCallRepository.GetStatusCallByProject(result.project);

                    if (result.status == CisCallStatus.BUSY || result.status == CisCallStatus.NOANSWER)
                    {
                        var st = callStatus.First(i => i.Name.ToLower().Contains("không nhấc máy"));
                        if (null != st)
                            failedCallStatusId = st.StatusCallID;
                    }
                    if (result.status != CisCallStatus.FAILED)
                    {
                        var st = callStatus.First(i => i.Name.ToLower().Contains("không liên lạc"));
                        if (null != st)
                            failedCallStatusId = st.StatusCallID;
                    }

                    // Check total called
                    var getCall = callRepository.GetCallByProjectAndCustomer(projectId: CommonHelper.CurrentProject(), customerId: new Guid(result.customer_id));
                    var call = new Call();
                    call.ProjectID = project.ProjectID;
                    call.UserId = new Guid(User.Identity.GetUserId());
                    call.CustomerID = new Guid(result.customer_id);
                    call.StatusCallID = failedCallStatusId;
                    call.IsSuccess = false;
                    var callID = callRepository.AddNewCall(call);

                    if(callID > 0)
                    {
                        logger.Info("Customer call log created");

                        return Json(new
                        {
                            Result = 1,
                            Message = "Success: Customer " + result.phone + " call status received"
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new
                        {
                            Result = 0,
                            Message = "Failed: Customer " + result.phone + " would not saved"
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    errorMsg = string.Format("Phone answered, waiting for DTV");
                    logger.Info(errorMsg);
                    return Json(new
                    {
                        Result = 1,
                        Message = errorMsg
                    }, JsonRequestBehavior.AllowGet);
                }
                    
                // Return m
                return Json(new
                {
                    Result = 0,
                    Message = errorMsg
                }, JsonRequestBehavior.AllowGet);
                
			}
			catch (Exception ex)
			{
				logger.Fatal(string.Format("CIS API exception at {0}: \n{1}", ex.Message , ex.StackTrace));
                return Json(new 
                {
                    Result = 0,
                    Message =  ex.Message
                }, JsonRequestBehavior.AllowGet);
            }

		}
        [NonAction]
        public CisResponseResult CISSendCustomer(CisCallRequest request)
        {
            // Send customer to ITCS
            try
            {
                string res = string.Empty;
                using (var client = new WebClient())
                {

                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client.Headers[HttpRequestHeader.UserAgent] = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.109 Safari/537.36";
                    client.Encoding = Encoding.UTF8;
                    string json = JsonConvert.SerializeObject(request);
                    logger.Info(string.Format("Sending data, range \n{0}", request));
                    // Start downloading
                    res = client.UploadString(ConfigurationManager.AppSettings["CisApiUrl"], "POST", json);

                    try
                    {
                        var result = JsonConvert.DeserializeObject<CisResponseResult>(res);
                        logger.Info("Data sent result is\n" + res);
                        return result;
                    }
                    catch
                    {
                        logger.Fatal(string.Format("CIS Send API received trash \n{0}: \n{1}", res));
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Fatal(string.Format("CIS Send API exception at {0}: \n{1}", ex.Message, ex.StackTrace));
            }
            return new CisResponseResult();
        }
    }
}
