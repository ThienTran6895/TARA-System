using MB.Common;
using MB.OMS.Telesale.Domain.Interface;
using MB.OMS.Telesale.Domain.Model;
using MB.OMS.Telesale.Service;
using MB.Web.Core;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MB.Common.Helpers;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity;
using MB.Common.Kendoui;
using System.Collections.Specialized;
using System.Configuration;

namespace MB.OMS.Telesale.Controller
{
    public class CustomerController : MBController
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.CustomerController");
        #region Fields

        [Dependency]
        public IQuestionRepository questionRepository { get; set; }

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

        [Dependency]
        public IQuestionsSurveyRepository questionsSurveyRepository { get; set; }

        [Dependency]
        public ICallLogRepository callLogRepository { get; set; }

        [Dependency]
        public ICallRepository callRepository { get; set; }

        [Dependency]
        public ISurveyAnswerRepository surveyAnswerRepository { get; set; }

        [Dependency]
        public IProjectCustomerRepository projectCustomerRepository { get; set; }

        [Dependency]
        public IUserRepository userRepository { get; set; }

        [Dependency]
        public IUserRoleRepository userRoleRepository { get; set; }

        [Dependency]
        public IRoleRepository roleRepository { get; set; }

        [Dependency]
        public IEwayAPIService ewayAPIService { get; set; }

        #endregion

        #region Utilities

        public ActionResult ChooseProject()
        {
            var model = new CustomerDTO();
            model.AvailableProject.Add(new SelectListItem()
            {
                Text = "---Chọn dự án---",
                Value = "0",
                Selected = true
            });
            model.AvailableProject.AddRange(projectsRepository.GetAll(visiable: true).Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.ProjectID.ToString()
            }));
            return Json(new { success = true, html = RenderPartialViewToString("ChooseProject", model) }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region CallSurvey
        //[PermissionAuthorize(Permissions.XemKhachHangCuaToi)]
        public ActionResult ListCustomerInitial()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemKhachHangCuaToi.ToString()))
                return AccessDeniedView();

            var customer = new CustomerDTO();
            customer.AvailableSource.Add(new SelectListItem()
            {
                Text = "--Chọn nguồn--",
                Value = "0",
                Selected = true
            });
            customer.AvailableSource.AddRange(sourcesRepository.GetAllSourcesByProject(CommonHelper.CurrentProject()).Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.SourceID.ToString()
            }));

            var listCustomerFeild = customerFieldRepository.GetCustomerFieldByProject(CommonHelper.CurrentProject());
            var arr = new List<string>();
            arr.Add("[0].Value");
            for (int i = 0; i < listCustomerFeild.Count(); i++)
            {
                int kq = i + 7;
                arr.Add("[" + kq + "].Value");
            }
            arr.Add("[5].Value");
            ViewBag.ListCustomerField = arr;

            var curentProject = projectsRepository.GetProjectsById(CommonHelper.CurrentProject());

            customer.ProjectName = curentProject == null ? "" : curentProject.Name;

            return View(customer);
        }

        [HttpPost]
        public JsonResult GetListCustomerInitial(DataSourceRequest dsRequest, CustomerDTO customer)
        {
            int? sourceID = null;

            var userRole = userRoleRepository.GetAll(userIds: new Guid(User.Identity.GetUserId())).FirstOrDefault();
            var role = roleRepository.GetRoleById(userRole.RoleId);
            if (role.RoleName != Role.Administrator.ToString())
            {
                sourceID = -1; // Dont show all customers
            }

            if (customer.SourceID > 0)
            {
                sourceID = customer.SourceID;
            }
            var data = customerRepository.GetAllCustomerFieldValueByProjectDatasource(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), sourceID: sourceID, callBy: new Guid(User.Identity.GetUserId()));

            return data.ToJsonDataSource();
        }



        public ActionResult CallSurvey(Guid customerID, int? callLogID = null)
        {
            var model = new CallSurvey();

            //Gọi lại
            if (callLogID.HasValue)
            {
                #region recall
                var callLog = callLogRepository.GetCallLogByID(callLogID.Value);
                var statusCall = statusCallRepository.GetStatusCall(callLog.StatusCallID);

                model = customerRepository.GetCustomerCallSurvey(customerID);

                model.AvailableCustomerFieldValue = customerFieldRepository.GetCustomerFieldAndValueByCustomerID(customerID: customerID, projectID: CommonHelper.CurrentProject()).Select(d => new CustomerFieldDTO()
                {
                    FieldCode = d.FieldCode,
                    FieldName = d.FieldName,
                    FieldValue = d.FieldValue
                }).ToList();

                var project = projectsRepository.GetProjectsById(CommonHelper.CurrentProject());
                var question = questionRepository.GetQuestionByProjectId(projectId: CommonHelper.CurrentProject(), visiable: true);
                foreach (var ques in question)
                {
                    var survey = surveyRepository.GetSurveyByQuestionId(projectID: CommonHelper.CurrentProject(), questionID: ques.QuestionID);
                    foreach (var sur in survey)
                    {
                        sur.NextQuestionID = questionsSurveyRepository.GetAll(projectID: CommonHelper.CurrentProject(), questionID: ques.QuestionID, surveyID: sur.SurveyID).FirstOrDefault().NextQuestionID;
                    }
                    ques.AvailableSurveys = survey.ToList();
                }
                model.AvailableQuestions = question.ToList();

                model.StatusCallID = callLog.StatusCallID;
                if (statusCall.StatusID == (int)StatusCallEnum.Recall)
                    model.RecallDate = callLog.RecallDate.HasValue ? callLog.RecallDate.Value.ToString() : string.Empty;

                model.Note = string.IsNullOrEmpty(callLog.Note) == true ? null : callLog.Note;

                model.CallID = callLog.CallID;
                model.CampaignConclusion = project.Conclusion;
                model.CampaignGreeting = project.Greeting;
                #endregion
            }
            else
            {
                #region new call
                model = customerRepository.GetCustomerCallSurvey(customerID);

                model.AvailableCustomerFieldValue = customerFieldRepository.GetCustomerFieldAndValueByCustomerID(customerID: customerID, projectID: CommonHelper.CurrentProject()).Select(d => new CustomerFieldDTO()
                {
                    FieldCode = d.FieldCode,
                    FieldName = d.FieldName,
                    FieldValue = d.FieldValue
                }).ToList();

                var project = projectsRepository.GetProjectsById(CommonHelper.CurrentProject());
                var question = questionRepository.GetQuestionByProjectId(projectId: CommonHelper.CurrentProject(), visiable: true);
                foreach (var ques in question)
                {
                    var survey = surveyRepository.GetSurveyByQuestionId(projectID: CommonHelper.CurrentProject(), questionID: ques.QuestionID);
                    foreach (var sur in survey)
                    {
                        sur.NextQuestionID = questionsSurveyRepository.GetAll(projectID: CommonHelper.CurrentProject(), questionID: ques.QuestionID, surveyID: sur.SurveyID).FirstOrDefault().NextQuestionID;
                    }
                    ques.AvailableSurveys = survey.ToList();
                }
                model.AvailableQuestions = question.ToList();

                model.CampaignConclusion = project.Conclusion;
                model.CampaignGreeting = project.Greeting;
                #endregion
            }

            //sondt - 2019
            // cần điều chỉnh lại, code lung tung quá
            #region  get call status of Project
            var lstStatus = statusCallRepository.GetAllStatus();
            if (lstStatus != null)
            {
                model.AvailableStatus.Add(new SelectListItem() { Text = "-Chọn tình trạng-", Value = "0" });
                model.AvailableStatus.AddRange(statusCallRepository.GetAllStatus().Select(d => new SelectListItem()
                {
                    Text = d.Name,
                    Value = d.StatusID.ToString()
                }));


                int firstStatusId = lstStatus.First().StatusID;
                model.AvailableStatusCall.Add(new SelectListItem() { Text = "-Chọn tình trạng-", Value = "0" });
                model.AvailableStatusCall.AddRange(statusCallRepository.GetStatusCallByStatusId(firstStatusId).Select(d => new SelectListItem()
                {
                    Text = d.Name,
                    Value = d.StatusCallID.ToString()
                }));
            }
            #endregion

            return Json(new { success = true, html = RenderPartialViewToString("CallSurvey", model) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult CallSurvey(FormCollection form)
        {
            //Lưu thông tin khách hàng
            var customer = new Customer();
            customer.CustomerID = new Guid(form["CustomerID"]);
            //customer.MobilePhone = form["MobilePhone"];
            customer.Visiable = true;
            customer.SourceID = Convert.ToInt32(form["SourceID"]);
            customerRepository.UpdateCustomer(customer);

            // All fields of this call
            var fields = new NameValueCollection();
            foreach (var item in form)
            {
                var customerField = customerFieldRepository.GetAll(fieldcode: item.ToString()).FirstOrDefault();
                if (customerField != null)
                {
                    var customerFieldValue = new CustomerFieldValue();
                    customerFieldValue.CustomerID = new Guid(form["CustomerID"]);
                    customerFieldValue.CustomerFieldID = customerField.CustomerFieldID;
                    customerFieldValue.FieldValue = form[item.ToString()];
                    customerFieldValueRepository.UpdateCustomerFieldValue(customerFieldValue);

                    // Prepairing data for Eway
                    //fields.Add(item.ToString(), form[item.ToString()]);
                }
            }

            var statusCall = statusCallRepository.GetStatusCall(Convert.ToInt32(form["StatusCallID"]));
            int callIds = Convert.ToInt32(form["CallID"]);

            if (callIds == 0)
            {
                #region new call
                var getCall = callRepository.GetCallByProjectAndCustomer(projectId: CommonHelper.CurrentProject(), customerId: new Guid(form["CustomerID"]));
                if (getCall.Count() == 0)
                {
                    //Lưu thông tin cuộc gọi           
                    var call = new Call();
                    call.ProjectID = CommonHelper.CurrentProject();
                    call.UserId = new Guid(User.Identity.GetUserId());
                    call.CustomerID = new Guid(form["CustomerID"]);
                    call.StatusCallID = statusCall.StatusCallID;
                    if (statusCall.StatusID == (int)StatusCallEnum.Success || statusCall.StatusID == (int)(StatusCallEnum.Potential) || statusCall.StatusID == (int)(StatusCallEnum.Recall))
                        call.IsSuccess = true;
                    else
                        call.IsSuccess = false;
                    var callID = callRepository.AddNewCall(call);
                    if (callID > 0)
                    {
                        var callLog = new CallLog();
                        callLog.CallID = callID;
                        callLog.ProjectID = CommonHelper.CurrentProject();
                        callLog.UserId = new Guid(User.Identity.GetUserId());
                        callLog.CustomerID = new Guid(form["CustomerID"]);
                        callLog.StatusCallID = statusCall.StatusCallID;
                        if (string.IsNullOrEmpty(form["RecallDate"]))
                            callLog.RecallDate = null;
                        else
                            callLog.RecallDate = DateTime.Parse(form["RecallDate"]);

                        callLog.Note = form["Note"];

                        if (statusCall.StatusID == (int)StatusCallEnum.Success ||
                            statusCall.StatusID == (int)(StatusCallEnum.Potential) ||
                            statusCall.StatusID == (int)(StatusCallEnum.Recall))
                            callLog.IsSuccess = true;
                        else
                            callLog.IsSuccess = false;

                        var callLogId = callLogRepository.AddNewCallLog(callLog);

                        fields.Add("status_message", form["Note"]);

                        //Cập nhật IsCall 
                        var projectCustomer = projectCustomerRepository.GetProjectCustomer(projectID: CommonHelper.CurrentProject(), customerID: new Guid(form["CustomerID"])).FirstOrDefault();
                        projectCustomer.IsCall = true;
                        projectCustomer.UpdatedDate = null;
                        projectCustomerRepository.UpdateProjectCustomer(projectCustomer);

                        //Lưu câu hỏi và trả lời sau khi khảo sát
                        var question = questionRepository.GetQuestionByProjectId(projectId: CommonHelper.CurrentProject(), visiable: true);
                        foreach (var que in question)
                        {
                            var listSurvey = surveyRepository.GetSurveyByQuestionId(projectID: CommonHelper.CurrentProject(), questionID: que.QuestionID).Where(s => s.SurveyType == (int)TypeSurvey.TEXTBOX).ToList();
                            //var answer = form["Question_" + que.QuestionID + "[]"];
                            var answer = form.GetValues("Question_" + que.QuestionID + "[]");
                            if (answer != null)
                            {
                                foreach (var id in answer)
                                {
                                    var surveyAnswer = new SurveyAnswer();
                                    surveyAnswer.CallLogID = callLogId;
                                    surveyAnswer.QuestionID = que.QuestionID;

                                    int number;
                                    bool isNumeric = int.TryParse(id, out number);
                                    if (isNumeric)
                                    {
                                        surveyAnswer.SurveyID = number;
                                        surveyAnswer.SurveyContent = "1";
                                    }
                                    else
                                    {
                                        surveyAnswer.SurveyID = listSurvey.FirstOrDefault().SurveyID;
                                        surveyAnswer.SurveyContent = id;

                                        listSurvey.Remove(listSurvey.FirstOrDefault());
                                    }

                                    surveyAnswerRepository.AddNewSurveyAnswer(surveyAnswer);
                                }
                            }
                        }
                    }
                }
                else
                {
                    return Json(new { Result = false, Message = "Khách hàng này đã được khảo sát" });
                }
                #endregion

            }
            else
            {

                #region count number calls
                //var countReCall = callLogRepository.GetAllCallHistory(projectID: CommonHelper.CurrentProject(), callID: callIds).Where(c => c.StatusID == (int)StatusCallEnum.Recall).Count();
                //if (statusCall.StatusID == (int)StatusCallEnum.Recall) && countReCall >= 6)
                //{
                //    // Báo thất bại trả lại cho EWAY - 12/26/2017
                //    fields.Add("status_message", form["Note"].Trim() == string.Empty ? "Đã gọi đủ 6 lần" : form["Note"]);
                //    int status = -1; // faied
                //    if (statusCall.StatusID == (int)StatusCallEnum.Success)
                //        status = 1;
                //    else if (statusCall.StatusID == (int)(StatusCallEnum.Recall) || statusCall.StatusID == (int)(StatusCallEnum.Potential))
                //        status = 0;
                //    else
                //    {
                //        status = -1;
                //        if (statusCall.StatusID == (int)(StatusCallEnum.Fail))
                //            fields["status_message"] = "Khách hàng từ chối: " + fields["status_message"];
                //        else if (statusCall.StatusID == (int)(StatusCallEnum.Wrong))
                //            fields["status_message"] = "Sai số điện thoại: " + fields["status_message"];
                //    }
                //    ewayAPIService.EwayExportData(fields, status);
                //    return Json(new { Result = false, Message = "Không được gọi lại quá 6 lần" });
                //}
                //else
                //{
                #endregion

                #region re-call
                var listRecall = callLogRepository.GetAllCallHistory(projectID: CommonHelper.CurrentProject(), callID: callIds).Where(c => c.StatusID == (int)StatusCallEnum.Recall && c.IsSuccess == true).Count();

                //Lưu thông tin cuộc gọi           
                var call = callRepository.GetCallByID(callIds);
                call.StatusCallID = statusCall.StatusCallID;
                if (statusCall.StatusID == (int)StatusCallEnum.Success || statusCall.StatusID == (int)(StatusCallEnum.Potential) || (statusCall.StatusID == (int)(StatusCallEnum.Recall) && listRecall <= 1))
                    call.IsSuccess = true;
                else
                    call.IsSuccess = false;

                callRepository.UpdateCall(call);

                var callLog = new CallLog();
                callLog.CallID = callIds;
                callLog.ProjectID = CommonHelper.CurrentProject();
                callLog.UserId = new Guid(User.Identity.GetUserId());
                callLog.CustomerID = new Guid(form["CustomerID"]);
                callLog.StatusCallID = statusCall.StatusCallID;
                if (string.IsNullOrEmpty(form["RecallDate"]))
                    callLog.RecallDate = null;
                else
                    callLog.RecallDate = DateTime.Parse(form["RecallDate"]);
                callLog.Note = form["Note"];
                if (statusCall.StatusID == (int)StatusCallEnum.Success || statusCall.StatusID == (int)(StatusCallEnum.Potential) || (statusCall.StatusID == (int)(StatusCallEnum.Recall) && listRecall <= 1))
                {
                    callLog.IsSuccess = true;
                }
                else
                {
                    callLog.IsSuccess = false;
                }

                fields.Add("status_message", form["Note"]);
                #region Send information to Eway and only if this is EWAY PROJECT
                //if (CommonHelper.CurrentProject().ToString() == ConfigurationManager.AppSettings["EWayProjectID"])
                //{

                //int status = 1; // success
                //if (statusCall.StatusID == (int)StatusCallEnum.Success)
                //    status = 1;
                //else if (statusCall.StatusID == (int)(StatusCallEnum.Recall) || statusCall.StatusID == (int)(StatusCallEnum.Potential))
                //    status = 0;
                //else
                //{
                //    status = -1;
                //    if (statusCall.StatusID == (int)(StatusCallEnum.Fail))
                //        fields["status_message"] = "Khách hàng từ chối: " + fields["status_message"];
                //    else if (statusCall.StatusID == (int)(StatusCallEnum.Wrong))
                //        fields["status_message"] = "Sai số điện thoại: " + fields["status_message"];
                //}

                //// Send data - don't send pending calls
                //if (status != 0)
                //    ewayAPIService.EwayExportData(fields, status);
                //}
                #endregion End Eway

                var callLogId = callLogRepository.AddNewCallLog(callLog);
                //Lưu câu hỏi và trả lời sau khi khảo sát
                var question = questionRepository.GetQuestionByProjectId(projectId: CommonHelper.CurrentProject(), visiable: true);
                foreach (var que in question)
                {
                    var listSurvey = surveyRepository.GetSurveyByQuestionId(projectID: CommonHelper.CurrentProject(), questionID: que.QuestionID).Where(s => s.SurveyType == (int)TypeSurvey.TEXTBOX).ToList();
                    var answer = form.GetValues("Question_" + que.QuestionID + "[]");
                    if (answer != null)
                    {
                        foreach (var id in answer)
                        {
                            var surveyAnswer = new SurveyAnswer();
                            surveyAnswer.CallLogID = callLogId;
                            surveyAnswer.QuestionID = que.QuestionID;

                            int number;
                            bool isNumeric = int.TryParse(id, out number);
                            if (isNumeric)
                            {
                                surveyAnswer.SurveyID = number;
                                surveyAnswer.SurveyContent = "1";
                            }
                            else
                            {
                                surveyAnswer.SurveyID = listSurvey.FirstOrDefault().SurveyID;
                                surveyAnswer.SurveyContent = id;

                                listSurvey.Remove(listSurvey.FirstOrDefault());
                            }

                            surveyAnswerRepository.AddNewSurveyAnswer(surveyAnswer);
                        }
                    }
                }
                #endregion
                //}
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult ChangeStatusCall(int statusID)
        {
            var model = statusCallRepository.GetStatusCall(statusID);

            bool success = false;
            if (model.StatusID == (int)StatusCallEnum.Recall || model.StatusID == (int)StatusCallEnum.Recall_2)
                success = true;

            return Json(new { success }, JsonRequestBehavior.AllowGet);
        }

        // sondt - 2019
        [HttpPost]
        public ActionResult GetStatusCallByStatusId(int st)
        {
            var model = statusCallRepository.GetStatusCallByStatusId(st);
            return Json(new { success = true, data = model }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult GetListCallHistory(DataSourceRequest dsRequest, CallSurvey callSurvey)
        //{
        //    var data = callLogRepository.GetAllCallHistoryDatasource(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), callID: callSurvey.CallID);
        //    return data.ToJsonDataSource();
        //}


        [HttpPost]
        public ActionResult GetListCallHistory(int callID)
        {
            DataSourceRequest dsRequest = new DataSourceRequest();
            dsRequest.Page = 1;
            dsRequest.PageSize = 20;
            var data = callLogRepository.GetAllCallHistoryDatasource(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), callID: callID);
            return data.ToJsonDataSource();
        }

        //[PermissionAuthorize(Permissions.XemKhachHangCuaToi)]
        public ActionResult CallServeyPopup(int projectId, string customerId, string phoneNumber)
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemKhachHangCuaToi.ToString()))
                return AccessDeniedView();

            var customer = customerRepository.GetCustomer(new Guid(customerId));

            return View(customer);
        }
        #endregion

        #region List Customer
        public ActionResult ListCustomer()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemDanhSachKhachHangMoi.ToString()))
                return AccessDeniedView();

            var customer = new CustomerDTO();
            customer.AvailableSource.Add(new SelectListItem()
            {
                Text = "--Chọn nguồn--",
                Value = "0"
            });
            var userRole = userRoleRepository.GetAll(userIds: new Guid(User.Identity.GetUserId())).FirstOrDefault();
            var role = roleRepository.GetRoleById(userRole.RoleId);
            if (role.RoleName == Role.Administrator.ToString())
            {
                customer.AvailableSource.AddRange(sourcesRepository.GetAll(visiable: true).Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.SourceID.ToString()
                }));
            }
            else
            {
                customer.AvailableSource.AddRange(sourcesRepository.GetAllSourcesByProject(CommonHelper.CurrentProject()).Select(u => new SelectListItem()
                {
                    Text = u.Name,
                    Value = u.SourceID.ToString()
                }));
            }
            // sondt
            //if (customer.AvailableSource.Count > 0) customer.AvailableSource[1].Selected = true;


            var listCustomerFeild = customerFieldRepository.GetCustomerFieldByProject(CommonHelper.CurrentProject());
            var arr = new List<string>();
            arr.Add("[0].Value");
            for (int i = 0; i < listCustomerFeild.Count(); i++)
            {
                int kq = i + 7;
                arr.Add("[" + kq + "].Value");
            }
            //arr.Add("[5].Value");
            ViewBag.ListCustomerField = arr;

            return View(customer);
        }

        [HttpPost]
        public JsonResult GetListCustomer(DataSourceRequest dsRequest, CustomerDTO customer)
        {

            int? sourceID = null;

            if (customer.SourceID > 0)
                sourceID = customer.SourceID;

            var data = new DataSourceResult();
            var userRole = userRoleRepository.GetAll(userIds: new Guid(User.Identity.GetUserId())).FirstOrDefault();
            var role = roleRepository.GetRoleById(userRole.RoleId);
            if (role.RoleName == Role.Administrator.ToString() || null != sourceID)
            {

                string DateFrom = string.IsNullOrEmpty(customer.DateFrom) == true ? null : customer.DateFrom;
                string EndDate = string.IsNullOrEmpty(customer.DateEnd) == true ? null : customer.DateEnd;
                data = customerRepository.GetAllFieldValueDatasource(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), sourceID: sourceID, dateFrom: DateFrom, dateEnd: EndDate);
            }

            return data.ToJsonDataSource();

        }

        #endregion

        #region List CustomerError

        public ActionResult ListCustomerError()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemDanhSachKHLoi.ToString()))
                return AccessDeniedView();

            var customer = new CustomerErrorDTO();
            customer.AvailableSource.Add(new SelectListItem()
            {
                Text = "--Chọn nguồn--",
                Value = "0",
                Selected = true
            });
            customer.AvailableSource.AddRange(sourcesRepository.GetAll().Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.SourceID.ToString()
            }));

            var listCustomerFeild = customerFieldRepository.GetCustomerFieldByProject(CommonHelper.CurrentProject());
            var arr = new List<string>();
            arr.Add("[0].Value");
            arr.Add("[1].Value");
            for (int i = 0; i < listCustomerFeild.Count(); i++)
            {
                int kq = i + 7;
                arr.Add("[" + kq + "].Value");
            }
            ViewBag.ListCustomerField = arr;

            return View(customer);
        }

        [HttpPost]
        public JsonResult GetListCustomerError(DataSourceRequest dsRequest, CustomerErrorDTO customer)
        {
            int? sourceID = null;

            if (customer.SourceID > 0)
                sourceID = customer.SourceID;

            var data = customerErrorRepository.GetAllFieldValueDatasource(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), sourceID: sourceID);

            return data.ToJsonDataSource();
        }
        #endregion

        #region List CustomerExist

        public ActionResult ListCustomerExist()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemDanhSachKHTrung.ToString()))
                return AccessDeniedView();

            var customer = new CustomerExistDTO();
            customer.AvailableSource.Add(new SelectListItem()
            {
                Text = "--Chọn nguồn--",
                Value = "0",
                Selected = true
            });
            customer.AvailableSource.AddRange(sourcesRepository.GetAll().Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.SourceID.ToString()
            }));

            var listCustomerFeild = customerFieldRepository.GetCustomerFieldByProject(CommonHelper.CurrentProject());
            var arr = new List<string>();
            arr.Add("[0].Value");
            arr.Add("[2].Value");
            for (int i = 0; i < listCustomerFeild.Count(); i++)
            {
                int kq = i + 9;
                arr.Add("[" + kq + "].Value");
            }
            ViewBag.ListCustomerField = arr;

            return View(customer);
        }

        [HttpPost]
        public JsonResult GetListCustomerExist(DataSourceRequest dsRequest, CustomerExistDTO customer)
        {
            int? sourceID = null;

            if (customer.SourceID > 0)
                sourceID = customer.SourceID;

            var data = customerExistRepository.GetAllFieldValueDatasource(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), sourceID: sourceID);

            return data.ToJsonDataSource();
        }
        #endregion

        #region Add Customer

        public ActionResult AddCustomer()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.ThemKhachHangMoi.ToString()))
                return AccessDeniedView();

            var model = new CustomerDTO();

            model.AvailableSource.Add(new SelectListItem()
            {
                Text = "Chọn nguồn",
                Value = "0",
                Selected = true
            });
            model.AvailableSource.AddRange(sourcesRepository.GetAll().Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.SourceID.ToString()
            }));

            model.AvailableCustomerFieldValue = customerFieldRepository.GetCustomerFieldByProject(projectID: CommonHelper.CurrentProject()).Select(d => new CustomerFieldDTO()
            {
                FieldCode = d.FieldCode,
                FieldName = d.FieldName
            }).ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddCustomer(FormCollection form)
        {
            if (customerRepository.GetAll(mobilePhone: form["MobilePhone"]).FirstOrDefault() == null)
            {
                // Insert Customer
                var model = new Customer();
                model.CustomerID = Guid.NewGuid();
                model.SourceID = Convert.ToInt32(form["SourceID"]);
                model.MobilePhone = form["MobilePhone"];
                model.Visiable = Convert.ToBoolean(form["Visiable"] != "false");
                model.IsDeleted = false;
                var result = customerRepository.AddNewCustomer(model);

                //Insert CustomerFieldValue
                foreach (var item in form)
                {
                    var customerField = customerFieldRepository.GetAll(fieldcode: item.ToString()).FirstOrDefault();
                    if (customerField != null)
                    {
                        var customerFieldValue = new CustomerFieldValue();
                        customerFieldValue.CustomerID = result;
                        customerFieldValue.CustomerFieldID = customerField.CustomerFieldID;
                        customerFieldValue.FieldValue = form[item.ToString()];
                        customerFieldValueRepository.AddNewCustomerFieldValue(customerFieldValue);
                    }
                }

                if (result == Guid.Empty)
                    return Json(new JsonResponse
                    {
                        Result = JsonMessage.Failed,
                        Message = "Thêm mới khách hàng không thành công!",
                        Title = "Thêm mới không thành công"
                    }, JsonRequestBehavior.DenyGet);
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Success,
                    Message = "Thêm mới khách hàng thành công!",
                    Title = "Thêm mới thành công",
                    URLList = "/OMS/Customer/ListCustomer",
                    Id = result.ToString()
                }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Số điện thoại đã được sử dụng!",
                    Title = "Thêm mới không thành công"
                }, JsonRequestBehavior.DenyGet);
            }
        }

        #endregion

        #region Edit Customer

        public ActionResult EditCustomer(Guid customerID)
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.SuaKhachHangMoi.ToString()))
                return AccessDeniedView();

            var model = customerRepository.GetCustomer(customerID);
            if (model == null)
                return RedirectToAction("ListCustomer");

            model.AvailableSource.Add(new SelectListItem()
            {
                Text = "Chọn nguồn",
                Value = "0",
                Selected = true
            });
            model.AvailableSource.AddRange(sourcesRepository.GetAll().Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.SourceID.ToString()
            }));

            model.AvailableCustomerFieldValue = customerFieldRepository.GetCustomerFieldAndValueByCustomerID(customerID: customerID, projectID: CommonHelper.CurrentProject()).Select(d => new CustomerFieldDTO()
            {
                FieldCode = d.FieldCode,
                FieldName = d.FieldName,
                FieldValue = d.FieldValue
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult EditCustomer(FormCollection form)
        {
            var customer = customerRepository.GetCustomer(new Guid(form["CustomerID"]));
            if (customer == null)
                return RedirectToAction("ListCustomer");

            if (customerRepository.GetAll(mobilePhone: form["MobilePhone"]).FirstOrDefault() == null && customer.MobilePhone != form["MobilePhone"])
            {
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Số điện thoại đã được sử dụng!",
                    Title = "Cập nhật không thành công"
                }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                // Update Customer
                var model = new Customer();
                model.CustomerID = new Guid(form["CustomerID"]);
                model.SourceID = Convert.ToInt32(form["SourceID"]);
                model.MobilePhone = form["MobilePhone"];
                model.Visiable = Convert.ToBoolean(form["Visiable"] != "false");
                var result = customerRepository.UpdateCustomer(model);

                //Update CustomerFieldValue
                foreach (var item in form)
                {
                    var customerField = customerFieldRepository.GetAll(fieldcode: item.ToString()).FirstOrDefault();
                    if (customerField != null)
                    {
                        var customerFieldValue = new CustomerFieldValue();
                        customerFieldValue.CustomerID = new Guid(form["CustomerID"]);
                        customerFieldValue.CustomerFieldID = customerField.CustomerFieldID;
                        customerFieldValue.FieldValue = form[item.ToString()];
                        customerFieldValueRepository.UpdateCustomerFieldValue(customerFieldValue);
                    }
                }

                if (result == 0)
                    return Json(new JsonResponse
                    {
                        Result = JsonMessage.Failed,
                        Message = "Cập nhật khách hàng không thành công!",
                        Title = "Cập nhật không thành công"
                    }, JsonRequestBehavior.DenyGet);
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Success,
                    Message = "Cập nhật khách hàng thành công!",
                    Title = "Cập nhật thành công",
                    URLEdit = "/OMS/Customer/EditCustomer?customerID=" + form["CustomerID"],
                    Id = form["CustomerID"]
                }, JsonRequestBehavior.DenyGet);
            }
        }

        #endregion

        #region Delete Customer

        [HttpPost]
        public ActionResult DeleteCustomer(Guid id)
        {
            var customer = customerRepository.GetCustomer(id);
            if (customer == null)
                return RedirectToAction("ListCustomer");

            var result = customerRepository.DeleteCustomer(id);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Xóa khách hàng không thành công!",
                    Title = "Xóa không thành công"
                }, JsonRequestBehavior.DenyGet);

            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa khách hàng thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/Customer/ListCustomer"
            }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<Guid> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    customerRepository.DeleteCustomer(item);
                }
            }
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa khách hàng thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/Customer/ListCustomer"
            }, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region Delete Customer Exist
        [HttpPost]
        public ActionResult DeleteSelectedCustomerExist(ICollection<Guid> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    customerExistRepository.DeleteCustomerExist(item);
                }
            }
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa khách hàng thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/Customer/ListCustomerExist"
            }, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region Delete Customer Error
        [HttpPost]
        public ActionResult DeleteSelectedCustomerError(ICollection<Guid> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    customerErrorRepository.DeleteCustomer(item);
                }
            }
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa khách hàng thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/Customer/ListCustomerError"
            }, JsonRequestBehavior.DenyGet);
        }
        #endregion

        #region Import, Export Excel

        public ActionResult ImportExcel()
        {
            var data = new CustomerDTO();
            return View(data);
        }

        [HttpPost]
        public ActionResult HandlingImportExcel(CustomerDTO model)
        {
            try
            {
                HttpPostedFileBase hpf = null;

                foreach (string file in Request.Files)
                {
                    hpf = Request.Files[file] as HttpPostedFileBase;
                    if (hpf.ContentLength == 0)
                        continue;
                    break;
                }

                if (hpf != null && hpf.ContentLength > 0)
                {

                    using (var xlPackage = new ExcelPackage(hpf.InputStream))
                    {
                        //Kiểm tra có sheet nào trong file excel không
                        var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                        if (worksheet == null)
                            return Json(new { success = false, message = "Không có worksheet nào trong file" }, JsonRequestBehavior.AllowGet);

                        var properties = new List<String>();
                        properties.Add("Số di động");
                        foreach (var item in customerFieldRepository.GetCustomerFieldByProject(projectID: CommonHelper.CurrentProject()))
                        {
                            properties.Add(item.FieldCode);
                        }

                        //Kiểm tra tên nguồn có tồn tại hay không
                        if (sourcesRepository.GetSourcesByName(model.SourceName) != null)
                            return Json(new { success = false, message = "Tên nguồn đã được sử dụng, vui lòng nhập lại tên nguồn khác!" }, JsonRequestBehavior.AllowGet);

                        //Kiểm tra cột trong file excel có hợp lệ hay không
                        int col = 1;
                        int countCol = 0;

                        while (true)
                        {
                            if (worksheet.Cells[1, col].Value == null)
                            {
                                countCol = col;
                                break;
                            }
                            col++;
                        }

                        var attributeExcel = new List<string>();
                        for (int i = 0; i < countCol - 1; i++)
                        {
                            attributeExcel.Add(CommonHelper.ConvertColumnToString(worksheet.Cells[1, i + 1].Value));
                        }

                        int count = 0;

                        for (int i = 0; i < properties.Count; i++)
                        {
                            for (int j = 0; j < attributeExcel.Count; j++)
                            {
                                if (attributeExcel[j].Contains(properties[i]) == true)
                                {
                                    count++;
                                }
                            }
                        }

                        if (count != properties.Count || properties.Count != attributeExcel.Count)
                            return Json(new { success = false, message = "Các cột trong file excel không giống file mẫu, vui lòng kiểm tra lại" }, JsonRequestBehavior.AllowGet);

                        //UpLoad file excel
                        var projectName = projectsRepository.GetProjectsById(CommonHelper.CurrentProject()).Name;
                        var path = CommonHelper.MapPath("~/Uploads/Excels/" + projectName + "_" + Path.GetFileName(hpf.FileName));
                        hpf.SaveAs(path);

                        //Insert Source
                        var source = new Sources();
                        source.Link = path;
                        source.Visiable = true;
                        source.Name = model.SourceName;
                        var sourceID = sourcesRepository.AddNewSources(source);

                        int listSuccess = 0;
                        var listExist = 0;
                        var listFail = 0;

                        int iRow = 2;

                        while (true)
                        {
                            logger.Info("iRow:" + iRow + "-SourceId:" + sourceID);
                            bool allColumnsAreEmpty = true;
                            for (var i = 1; i <= properties.Count; i++)
                                if (worksheet.Cells[iRow, i].Value != null && !String.IsNullOrEmpty(worksheet.Cells[iRow, i].Value.ToString()))
                                {
                                    allColumnsAreEmpty = false;
                                    break;
                                }
                            if (allColumnsAreEmpty)
                                break;
                            //if (iRow == 348)
                            //{
                            //    string emp = "asdasd";
                            //    emp = "";
                            //}
                            //Get value in file excel
                            string mobilePhone = CommonHelper.ConvertColumnToString(worksheet.Cells[iRow, CommonHelper.GetColumnIndex(properties, "Số di động")].Value);
                            logger.Info("NumberPhone:" + mobilePhone);
                            #region check phone number 
                            //if (!Regex.IsMatch(mobilePhone, StringHelper.PhoneValidate()))
                            //{
                            //    #region Insert CustomerError
                            //    logger.Info("Error: ImportError, NumberPhone:" + mobilePhone.ToString());
                            //    //Insert CustomerError
                            //    var customerError = new CustomerError();
                            //    customerError.CustomerErrorID = Guid.NewGuid();
                            //    customerError.SourceID = sourceID;
                            //    customerError.ProjectID = CommonHelper.CurrentProject();
                            //    customerError.Phone = mobilePhone;
                            //    customerError.RowError = iRow;
                            //    customerError.Visiable = true;
                            //    customerError.IsDeleted = false;
                            //    var customerErrorID = customerErrorRepository.AddNewCustomer(customerError);

                            //    //Insert CustomerErrorFieldValue
                            //    foreach (var item in properties.Skip(1))
                            //    {
                            //        string fieldValue = CommonHelper.ConvertColumnToString(worksheet.Cells[iRow, CommonHelper.GetColumnIndex(properties, item)].Value);
                            //        var customerField = customerFieldRepository.GetAll(fieldcode: item.ToString()).FirstOrDefault();
                            //        if (customerField != null)
                            //        {
                            //            var customerErrorFieldValue = new CustomerErrorFieldValue();
                            //            customerErrorFieldValue.CustomerErrorID = customerErrorID;
                            //            customerErrorFieldValue.CustomerFieldID = customerField.CustomerFieldID;
                            //            customerErrorFieldValue.FieldValue = fieldValue;
                            //            customerErrorFieldValueRepository.AddNewCustomerErrorFieldValue(customerErrorFieldValue);
                            //        }
                            //    }
                            //    #endregion 
                            //    listFail++;
                            //}
                            //else
                            //{
                            #endregion

                            var checkCustomer = customerRepository.GetAll(sourceID: sourceID, mobilePhone: mobilePhone).FirstOrDefault();
                            if (checkCustomer == null)
                            {
                                #region  Insert Customer
                                // Insert Customer
                                var customer = new Customer();
                                customer.CustomerID = Guid.NewGuid();
                                customer.SourceID = sourceID;
                                customer.MobilePhone = mobilePhone;
                                customer.Visiable = true;
                                customer.IsDeleted = false;
                                var customerID = customerRepository.AddNewCustomer(customer);

                                //Insert CustomerFieldValue
                                foreach (var item in properties.Skip(1))
                                {
                                    string fieldValue = CommonHelper.ConvertColumnToString(worksheet.Cells[iRow, CommonHelper.GetColumnIndex(properties, item)].Value);
                                    var customerField = customerFieldRepository.GetAll(fieldcode: item.ToString()).FirstOrDefault();
                                    if (customerField != null)
                                    {
                                        var customerFieldValue = new CustomerFieldValue();
                                        customerFieldValue.CustomerID = customerID;
                                        customerFieldValue.CustomerFieldID = customerField.CustomerFieldID;
                                        customerFieldValue.FieldValue = fieldValue;
                                        customerFieldValueRepository.AddNewCustomerFieldValue(customerFieldValue);
                                    }
                                }
                                #endregion
                                listSuccess++;
                            }
                            else
                            {
                                #region Insert Customer CustomerExist
                                // Insert Customer CustomerExist
                                var customerExist = new CustomerExist();
                                customerExist.CustomerExistID = Guid.NewGuid();
                                customerExist.SourceID = sourceID;
                                customerExist.CustomerID = checkCustomer.CustomerID;
                                customerExist.ProjectID = CommonHelper.CurrentProject();
                                customerExist.Phone = mobilePhone;
                                customerExist.RowExist = iRow;
                                customerExist.Visiable = true;
                                customerExist.IsDeleted = false;
                                var customerExistID = customerExistRepository.AddNewCustomerExist(customerExist);

                                //Insert CustomerExistFieldValue
                                foreach (var item in properties.Skip(1))
                                {
                                    string fieldValue = CommonHelper.ConvertColumnToString(worksheet.Cells[iRow, CommonHelper.GetColumnIndex(properties, item)].Value);
                                    var customerField = customerFieldRepository.GetAll(fieldcode: item.ToString()).FirstOrDefault();
                                    if (customerField != null)
                                    {
                                        var customerExistFieldValue = new CustomerExistFieldValue();
                                        customerExistFieldValue.CustomerExistID = customerExistID;
                                        customerExistFieldValue.CustomerFieldID = customerField.CustomerFieldID;
                                        customerExistFieldValue.FieldValue = fieldValue;
                                        customerExistFieldValueRepository.AddNewCustomerExistFieldValue(customerExistFieldValue);
                                    }
                                }
                                #endregion
                                listExist++;
                            }

                            //}

                            //next customer
                            iRow++;
                        }

                        //Gửi mail
                        var currentUser = userRepository.GetUserById(new Guid(User.Identity.GetUserId()));
                        string body = "<p><b>Tên nguồn: </b> " + model.SourceName + "</p>";
                        body = "<p><b>Tên file import: </b> " + hpf.FileName + "</p>";
                        body += "<p><b>Import thành công: </b> " + listSuccess + " khách hàng</p>";
                        body += "<p><b>Import trùng: </b> " + listExist + " khách hàng</p>";
                        body += "<p><b>Import lỗi: </b> " + listFail + " khách hàng</p>";
                        body += "<p><b>Import hoàn thành lúc: </b> " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt") + "</p>";
                        body += "<p><b>Người thực hiện: </b> " + currentUser.UserName + "</p>";
                        try
                        {
                            logger.Info("Gửi mail import");
                            CommonHelper.SendEmail(subject: "Import data", body: body, toAddress: currentUser.Email, toName: currentUser.FullName, cc: new List<string>() { "khangvn@matbao.com" });

                        }
                        catch (Exception exm)
                        {
                            logger.Fatal("Error: Gửi mail, Detail:" + exm.ToString());
                        }
                    }
                }
                else
                {
                    logger.Info("File empty");
                    return Json(new { success = false, message = "File không tồn tại!!!" }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, message = "Import thành công" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                logger.Fatal("Error: ImportExcel, Detail:" + ex.ToString());
                //return null;
                return Json(new { success = false, message = "Import không thành công" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownloadExcel()
        {
            byte[] bytes;
            try
            {
                using (var stream = new MemoryStream())
                {
                    if (stream == null)
                        throw new ArgumentNullException("stream");

                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Danh sách khách hàng");
                        worksheet.DefaultColWidth = 18;

                        worksheet.Cells[1, 1].Value = "Số di động";
                        worksheet.Cells[1, 1].Style.Font.Bold = true;

                        int j = 0;
                        foreach (var item in customerFieldRepository.GetCustomerFieldByProject(projectID: CommonHelper.CurrentProject()))
                        {
                            worksheet.Cells[1, j + 2].Value = string.Format("{0}|{1}", item.FieldCode, item.FieldName);
                            worksheet.Cells[1, j + 2].Style.Font.Bold = true;
                            j++;
                        }
                        var border = worksheet.Cells[1, 1, 1, j + 1].Style.Border;
                        border.Bottom.Style = border.Right.Style = border.Left.Style = border.Top.Style = ExcelBorderStyle.Thin;
                        worksheet.Cells[1, 1, 1, j + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, 1, 1, j + 1].Style.Fill.BackgroundColor.SetColor(Color.Aquamarine);

                        xlPackage.Save();
                    }
                    bytes = stream.ToArray();
                }
                return File(bytes, "text/xls", "template.xlsx");
            }
            catch (Exception ex)
            {
                logger.Fatal("DownloadExcel: " + ex.ToString());
            }
            return null;
        }

        public ActionResult ExportExcel(int SourceID, string DateFrom, string DateEnd)
        {
            // 2019 - sondt
            // kiem tra lai truong hop ko chọn source thì thông báo, ko cho export
            string sourceName = "";
            CustomerDTO customer = new CustomerDTO();
            if (SourceID > 0)
                customer.SourceID = SourceID;
            DateFrom = string.IsNullOrEmpty(DateFrom) == true ? null : DateFrom;
            DateEnd = string.IsNullOrEmpty(DateEnd) == true ? null : DateEnd;

            //var data = customerRepository.GetAllFieldValue(projectID: CommonHelper.CurrentProject(), sourceID: customer.SourceID, dateFrom: DateFrom, dateEnd: DateEnd);
            var data = customerRepository.GetCustomerFieldValues(projectID: CommonHelper.CurrentProject(), sourceID: customer.SourceID, dateFrom: DateFrom, dateEnd: DateEnd);

            var CustomerField = customerFieldRepository.GetCustomerFieldByProject(CommonHelper.CurrentProject());
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Danh sách khách hàng");
                    int row = 2;
                    var propertype = new List<string>();
                    propertype.Add("phone_number");
                    propertype.Add("source_name");
                    propertype.Add("created_date");

                    foreach (var item in CustomerField)
                    {
                        propertype.Add(item.FieldName);
                    }
                    propertype.Add("StatusName");
                    propertype.Add("StatusCallName");


                    for (int i = 0; i < propertype.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = propertype[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Column(1 + 1).Width = 20;
                        var border1 = worksheet.Cells[1, i + 1].Style.Border;
                        border1.Bottom.Style = border1.Top.Style = border1.Left.Style = border1.Right.Style = ExcelBorderStyle.Thin;
                    }


                    foreach (IDictionary<string, object> rp in data)
                    {
                        int col = 1;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "MobilePhone").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "SourceName").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "CreatedDate").Value.ToDateTime().ToString("dd/MM/yyyy HH:mm");
                        col++;
                        foreach (var item in CustomerField)
                        {
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == item.FieldName).Value;
                            col++;
                        }

                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "StatusName").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "StatusCallName").Value;
                        col++;

                        row++;
                    }
                    xlPackage.Save();

                }
                bytes = stream.ToArray();
            }
            return File(bytes, "text/xls", DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
        }

        /// <summary>
        /// ExportToCSV
        /// </summary>
        /// <param name="SourceID"></param>
        /// <returns></returns>
        public ActionResult ExportCSV(int SourceID, string DateFrom, string DateEnd)
        {
            string sourceName = "";
            CustomerDTO customer = new CustomerDTO();
            if (SourceID > 0)
                customer.SourceID = SourceID;
            DateFrom = string.IsNullOrEmpty(DateFrom) == true ? null : DateFrom;
            DateEnd = string.IsNullOrEmpty(DateEnd) == true ? null : DateEnd;

            // lay ds thuoc tinh cua KH
            var listCustomerFields = new string[] {
                "target_bank_code",
                "product_code",
                "phone_number",
                "national_id",
                "monthly_income",
                "loan_amount",
                "income_type",
                "email",
                "full_name",
                "dob",
                "desired_tenor"
            };


            // lay toan bo thong tin KH
            StringBuilder customerCSV = new StringBuilder();
            var data = customerRepository.GetAllFieldValue(projectID: CommonHelper.CurrentProject(), sourceID: customer.SourceID, dateFrom: DateFrom, dateEnd: DateEnd);
            if (data != null)
            {
                foreach (IDictionary<string, object> rp in data)
                {
                    customerCSV.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                        rp.FirstOrDefault(x => x.Key == "target_bank_code").Value,
                        rp.FirstOrDefault(x => x.Key == "product_code").Value,
                        rp.FirstOrDefault(x => x.Key == "MobilePhone").Value,
                        rp.FirstOrDefault(x => x.Key == "national_id").Value,
                        rp.FirstOrDefault(x => x.Key == "monthly_income").Value,
                        rp.FirstOrDefault(x => x.Key == "loan_amount").Value,
                        rp.FirstOrDefault(x => x.Key == "income_type").Value,
                        rp.FirstOrDefault(x => x.Key == "email").Value,
                        rp.FirstOrDefault(x => x.Key == "full_name").Value,
                        rp.FirstOrDefault(x => x.Key == "dob").Value,
                        rp.FirstOrDefault(x => x.Key == "desired_tenor").Value
                        ));
                }
            }
            byte[] buffer = Encoding.UTF8.GetBytes($"{string.Join(",", listCustomerFields)}\r\n{customerCSV.ToString()}");
            return File(buffer, "text/csv", DateTime.Now.ToString("yyyyMMdd") + "_result_" + sourceName + ".csv");
        }
        public ActionResult ExportExcelExist(int SourceID)
        {
            CustomerDTO customer = new CustomerDTO();
            if (SourceID > 0)
                customer.SourceID = SourceID;

            var data = customerExistRepository.GetAllFieldValue(projectID: CommonHelper.CurrentProject(), sourceID: customer.SourceID);
            var CustomerField = customerFieldRepository.GetCustomerFieldByProject(CommonHelper.CurrentProject());
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                using (var xlPackage = new ExcelPackage(stream))
                {
                    //var QuestionInProject = projectQuestionsRepository.GetAll(projectID: curProjectID);
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Danh sách khách hàng trùng");
                    int row = 2;
                    var propertype = new List<string>();
                    propertype.Add("STT");
                    propertype.Add("SĐT cá nhân");
                    propertype.Add("Tên nguồn");
                    foreach (var item in CustomerField)
                    {
                        propertype.Add(item.FieldName);
                    }
                    for (int i = 0; i < propertype.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = propertype[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Column(1 + 1).Width = 20;
                        var border1 = worksheet.Cells[1, i + 1].Style.Border;
                        border1.Bottom.Style = border1.Top.Style = border1.Left.Style = border1.Right.Style = ExcelBorderStyle.Thin;
                    }
                    foreach (IDictionary<string, object> rp in data)
                    {
                        int col = 1;
                        worksheet.Cells[row, col].Value = row - 1;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "Phone").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "SourceName").Value;
                        col++;
                        foreach (var item in CustomerField)
                        {
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == item.FieldName).Value;
                            col++;
                        }
                        row++;
                    }
                    xlPackage.Save();

                }
                bytes = stream.ToArray();
            }
            return File(bytes, "text/xls", "Danh sách khách hàng trùng.xlsx");
        }
        public ActionResult ExportExcelError(int SourceID)
        {
            CustomerDTO customer = new CustomerDTO();
            if (SourceID > 0)
                customer.SourceID = SourceID;

            var data = customerErrorRepository.GetAllFieldValue(projectID: CommonHelper.CurrentProject(), sourceID: customer.SourceID);
            var CustomerField = customerFieldRepository.GetCustomerFieldByProject(CommonHelper.CurrentProject());
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                using (var xlPackage = new ExcelPackage(stream))
                {
                    //var QuestionInProject = projectQuestionsRepository.GetAll(projectID: curProjectID);
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Danh sách khách hàng lỗi");
                    int row = 2;
                    var propertype = new List<string>();
                    propertype.Add("STT");
                    propertype.Add("SĐT cá nhân");
                    propertype.Add("Tên nguồn");
                    foreach (var item in CustomerField)
                    {
                        propertype.Add(item.FieldName);
                    }
                    for (int i = 0; i < propertype.Count; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = propertype[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Column(1 + 1).Width = 20;
                        var border1 = worksheet.Cells[1, i + 1].Style.Border;
                        border1.Bottom.Style = border1.Top.Style = border1.Left.Style = border1.Right.Style = ExcelBorderStyle.Thin;
                    }
                    foreach (IDictionary<string, object> rp in data)
                    {
                        int col = 1;
                        worksheet.Cells[row, col].Value = row - 1;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "Phone").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "SourceName").Value;
                        col++;
                        foreach (var item in CustomerField)
                        {
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == item.FieldName).Value;
                            col++;
                        }
                        row++;
                    }
                    xlPackage.Save();

                }
                bytes = stream.ToArray();
            }
            return File(bytes, "text/xls", "Danh sách khách hàng lỗi.xlsx");
        }

        #endregion

        #region Compare Customer

        public ActionResult CompareCustomer(Guid customerID)
        {
            var model = customerExistRepository.GetCustomerExist(customerID);
            if (model == null)
                return RedirectToAction("ListCustomerExist");

            model.AvailableCustomerFieldValue = customerFieldRepository.GetCustomerFieldAndValueByCustomerExistID(customerExistID: customerID, projectID: CommonHelper.CurrentProject()).Select(d => new CustomerFieldDTO()
            {
                FieldCode = d.FieldCode,
                FieldName = d.FieldName,
                FieldValue = d.FieldValue
            }).ToList();

            model.Customer = customerRepository.GetCustomer(model.CustomerID);
            model.Customer.AvailableCustomerFieldValue = customerFieldRepository.GetCustomerFieldAndValueByCustomerID(customerID: model.CustomerID, projectID: CommonHelper.CurrentProject()).Select(d => new CustomerFieldDTO()
            {
                FieldCode = d.FieldCode,
                FieldName = d.FieldName,
                FieldValue = d.FieldValue
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult CompareCustomer(ICollection<string> selectedIds, Guid customerExistID)
        {
            var customerExist = customerExistRepository.GetCustomerExist(customerExistID);
            if (selectedIds != null)
            {
                foreach (var itemt in selectedIds)
                {
                    var customerField = customerFieldRepository.GetAll(fieldcode: itemt).FirstOrDefault();
                    if (customerField != null)
                    {
                        var customerExistFieldValue = customerExistFieldValueRepository.GetAll(customerFieldID: customerField.CustomerFieldID, customerExistID: customerExistID).FirstOrDefault().FieldValue;

                        var customerFieldValue = new CustomerFieldValue();
                        customerFieldValue.CustomerID = customerExist.CustomerID;
                        customerFieldValue.CustomerFieldID = customerField.CustomerFieldID;
                        customerFieldValue.FieldValue = customerExistFieldValue;
                        customerFieldValueRepository.UpdateCustomerFieldValue(customerFieldValue);
                    }
                }
            }

            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Ghi đè thông tin khách hàng thành công!",
                Title = "Ghi đè thành công",
                URLEdit = "/OMS/Customer/CompareCustomer?customerID=" + customerExistID,
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region Show Detail Customer

        public ActionResult ShowDetailCustomer(string mobilePhone, string cId)
        {
            var model = new CustomerDTO();
            //model = customerRepository.GetCustomerByMobilePhone(mobilePhone);
            model = customerRepository.GetCustomer(new Guid(cId));

            model.AvailableCustomerFieldValue = customerFieldRepository.GetCustomerFieldAndValueByMobilePhone(phone: mobilePhone, customerId: new Guid(cId), projectID: CommonHelper.CurrentProject()).Select(d => new CustomerFieldDTO()
            {
                FieldCode = d.FieldCode,
                FieldName = d.FieldName,
                FieldValue = d.FieldValue
            }).ToList();
            model.AvailableSource.AddRange(sourcesRepository.GetAll().Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.SourceID.ToString()
            }));
            model.MobilePhone = mobilePhone;

            var lstStatus = statusCallRepository.GetAllStatus();
            if (lstStatus != null)
            {
                ViewBag.ListStatus = lstStatus;
                int firstStatusId = lstStatus.First().StatusID;
                ViewBag.ListStatusCall = statusCallRepository.GetStatusCallByStatusId(firstStatusId);
            }


            return Json(new { success = true, html = RenderPartialViewToString("ShowDetailCustomer", model) }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult GetListShowCallHistory(DataSourceRequest dsRequest, CustomerDTO customer)
        public ActionResult GetListShowCallHistory(string customerId)
        {
            DataSourceRequest dsRequest = new DataSourceRequest();
            dsRequest.Page = 1;
            dsRequest.PageSize = 20;
            var data = callLogRepository.GetAllCallHistoryDatasource(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), customerID: new Guid(customerId));
            return data.ToJsonDataSource();
        }


        public ActionResult ShowCallLogDetail(int callLogId)
        {
            var model = callLogRepository.GetCallLogByID(callLogId);
            return Json(new { success = true, data= model }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveCallLog(UpdateCallLogFormModel formModel)
        {
            try
            {
                var model = formModel.CallLog;
                if (model.StatusID == (int)StatusCallEnum.Success ||
                    model.StatusID == (int)(StatusCallEnum.Potential) ||
                    model.StatusID == (int)(StatusCallEnum.Recall))
                    model.IsSuccess = true;
                else
                    model.IsSuccess = false;

                var lstCusField = (List<CustomerValue>)formModel.CustomerInfo;
                foreach (var cf in lstCusField)
                {
                    var customerField = customerFieldRepository.GetAll(fieldname: cf.FieldName).FirstOrDefault();
                    if (customerField != null)
                    {
                        var customerFieldValue = new CustomerFieldValue();
                        customerFieldValue.CustomerID = new Guid(cf.CustomerID);
                        customerFieldValue.CustomerFieldID = customerField.CustomerFieldID;
                        customerFieldValue.FieldValue = cf.FieldValue;
                        customerFieldValueRepository.UpdateCustomerFieldValue(customerFieldValue);
                    }
                }

                callLogRepository.UpdateCallLog(model);
                return Json(new { success = true, message = "Đã cập nhật cuộc gọi." }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi cập nhật cuộc gọi, vui lòng liên hệ Administrator" }, JsonRequestBehavior.AllowGet);
            }
        }
        
        #endregion


    }
}
