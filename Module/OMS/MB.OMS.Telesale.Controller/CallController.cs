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
using System.Collections.Specialized;

namespace MB.OMS.Telesale.Controller
{
    public class CallController : MBController
    {
        #region Feild

        [Dependency]
        public ICallLogRepository callLogRepository { get; set; }

        [Dependency]
        public ICallRepository callRepository { get; set; }

        [Dependency]
        public IProjectsRepository projectsRepository { get; set; }

        [Dependency]
        public IUserRepository userRepository { get; set; }

        [Dependency]
        public IUserRoleRepository userRoleRepository { get; set; }

        [Dependency]
        public IRoleRepository roleRepository { get; set; }

        [Dependency]
        public IProjectCustomerFieldRepository projectCustomerFieldRepository { get; set; }

        // for the new all
        [Dependency]
        public IQuestionRepository questionRepository { get; set; }
        [Dependency]
        public ICustomerRepository customerRepository { get; set; }

        [Dependency]
        public ISurveyRepository surveyRepository { get; set; }
        [Dependency]
        public ISurveyAnswerRepository surveyAnswerRepository { get; set; }
        [Dependency]
        public IQuestionsSurveyRepository questionsSurveyRepository { get; set; }
        [Dependency]
        public ICustomerFieldRepository customerFieldRepository { get; set; }

        [Dependency]
        public IStatusCallRepository statusCallRepository { get; set; }

        [Dependency]
        public ICustomerFieldValueRepository customerFieldValueRepository { get; set; }

        [Dependency]
        public IProjectCustomerRepository projectCustomerRepository { get; set; }
        #endregion

        #region List cuoc goi thành công
        public ActionResult ListCall()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemCuocGoiThanhCong.ToString()))
                return AccessDeniedView();

            var call = new Call();

            call.AvailableUser.Add(new SelectListItem()
            {
                Text = "--Chọn điện thoai viên--",
                Value = "",
                Selected = true
            });
            call.AvailableUser.AddRange(userRepository.GetAll(visible: true).Select(d => new SelectListItem()
            {
                Text = d.LastName + ' ' + d.FirstName,
                Value = d.Id.ToString()
            }));
            call.UserId = new Guid(User.Identity.GetUserId());

            var userRole = userRoleRepository.GetAll(userIds: new Guid(User.Identity.GetUserId())).FirstOrDefault();
            var role = roleRepository.GetRoleById(userRole.RoleId);
            ViewBag.RoleName = role.RoleName;

            return View(call);
        }

        [HttpPost]
        public JsonResult GetListCallSuccess(DataSourceRequest dsRequest, Call call)
        {
            if (call.UserId != Guid.Empty)
            {
                var data = callRepository.GetAllCallSuccess(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), DTV: call.UserId);
                return data.ToJsonDataSource();
            }
            else
            {
                var data = callRepository.GetAllCallSuccess(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject());
                return data.ToJsonDataSource();
            }
        }
        #endregion

        #region List cuoc goi không thành công

        public ActionResult ListCallNotSuccess()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemCuocGoiKhongThanhCong.ToString()))
                return AccessDeniedView();

            var call = new Call();

            call.AvailableUser.Add(new SelectListItem()
            {
                Text = "--Chọn điện thoai viên--",
                Value = "",
                Selected = true
            });
            call.AvailableUser.AddRange(userRepository.GetAll(visible: true).Select(d => new SelectListItem()
            {
                Text = d.LastName + ' ' + d.FirstName,
                Value = d.Id.ToString()
            }));
            call.UserId = new Guid(User.Identity.GetUserId());

            var userRole = userRoleRepository.GetAll(userIds: new Guid(User.Identity.GetUserId())).FirstOrDefault();
            var role = roleRepository.GetRoleById(userRole.RoleId);
            ViewBag.RoleName = role.RoleName;


            call.AvailableFeild.Add(new SelectListItem()
            {
                Text = "--Chọn thuộc tính--",
                Value = "",             
            });
            call.AvailableFeild.AddRange(customerFieldRepository.GetCustomerFieldByProject(projectID: CommonHelper.CurrentProject()).Select(t => new SelectListItem()
            {
                Text = t.FieldName,
                Value = t.CustomerFieldID.ToString()
            }));

            call.AvailableStatus.Add(new SelectListItem()
            {
                Text = "--Chọn tình trạng--",
                Value = "",
            });
            call.AvailableStatus.AddRange(statusCallRepository.GetStatusCallByProject(projectID: CommonHelper.CurrentProject()).Select(t => new SelectListItem()
            {
                Text = t.Name,
                Value = t.StatusCallID.ToString()
            }));
           
            return View(call);
        }
              


        [HttpPost]
        public JsonResult GetListCallNotSuccess(DataSourceRequest dsRequest, Call call)
        {            
            if (call.UserId != Guid.Empty)
            {
                var data = callRepository.GetAllCallNotSuccess(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), StatusCallID: call.StatusCallID, MobilePhone: call.MobilePhone, DTV: call.UserId);                
                return data.ToJsonDataSource();
            }
            else
            {
                var data = callRepository.GetAllCallNotSuccess(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), StatusCallID: call.StatusCallID, MobilePhone: call.MobilePhone);               
                return data.ToJsonDataSource();
            }          
        }
       

        [HttpPost]
        public JsonResult GetListCallNotSuccessByDTVandMobilePhone(DataSourceRequest dsRequest, Call call)
        {
            if (call.UserId != Guid.Empty)
            {
                var data = callRepository.GetAllCallNotSuccessByDTVandMobilePhone(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(),MobilePhone: call.MobilePhone , DTV: call.UserId);
                return data.ToJsonDataSource();
            }
            else
            {
                var data = callRepository.GetAllCallNotSuccessByDTVandMobilePhone(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), MobilePhone: call.MobilePhone);
                return data.ToJsonDataSource();
            }
        }

        #endregion

        #region List cuoc goi sai so

        public ActionResult ListCallNumberFail()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemCuocGoiSaiSo.ToString()))
                return AccessDeniedView();

            var call = new Call();

            call.AvailableUser.Add(new SelectListItem()
            {
                Text = "--Chọn điện thoai viên--",
                Value = "",
                Selected = true
            });
            call.AvailableUser.AddRange(userRepository.GetAll(visible: true).Select(d => new SelectListItem()
            {
                Text = d.LastName + ' ' + d.FirstName,
                Value = d.Id.ToString()
            }));
            call.UserId = new Guid(User.Identity.GetUserId());

            var userRole = userRoleRepository.GetAll(userIds: new Guid(User.Identity.GetUserId())).FirstOrDefault();
            var role = roleRepository.GetRoleById(userRole.RoleId);
            ViewBag.RoleName = role.RoleName;

            return View(call);
        }

        [HttpPost]
        public JsonResult GetListCallListCallNumberFail(DataSourceRequest dsRequest, Call call)
        {
            if (call.UserId != Guid.Empty)
            {
                var data = callRepository.GetAllCallNumberFail(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), DTV: call.UserId);
                return data.ToJsonDataSource();
            }
            else
            {
                var data = callRepository.GetAllCallNumberFail(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject());
                return data.ToJsonDataSource();
            }
        }

        #endregion

        #region List cuoc goi hen goi lại
        public ActionResult ListCallReCall()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemCuocHenGoiLai.ToString()))
                return AccessDeniedView();

            var call = new Call();

            call.AvailableUser.Add(new SelectListItem()
            {
                Text = "--Chọn điện thoai viên--",
                Value = "",
                Selected = true
            });
            call.AvailableUser.AddRange(userRepository.GetAll(visible: true).Select(d => new SelectListItem()
            {
                Text = d.LastName + ' ' + d.FirstName,
                Value = d.Id.ToString()
            }));
            call.UserId = new Guid(User.Identity.GetUserId());

            var userRole = userRoleRepository.GetAll(userIds: new Guid(User.Identity.GetUserId())).FirstOrDefault();
            var role = roleRepository.GetRoleById(userRole.RoleId);
            ViewBag.RoleName = role.RoleName;

            return View(call);
        }
        [HttpPost]
        public JsonResult GetListCallListCallReCall(DataSourceRequest dsRequest, Call call)
        {
            if (call.UserId != Guid.Empty)
            {
                var data = callRepository.GetAllCallReCall(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), DTV: call.UserId);
                return data.ToJsonDataSource();
            }
            else
            {
                var data = callRepository.GetAllCallReCall(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject());
                return data.ToJsonDataSource();
            }
        }
        #endregion

        #region List cuoc goi tiem nang
        public ActionResult ListCallPotential()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemCuocGoiTiemNang.ToString()))
                return AccessDeniedView();

            var call = new Call();

            call.AvailableUser.Add(new SelectListItem()
            {
                Text = "--Chọn điện thoại viên--",
                Value = "",
                Selected = true
            });
            call.AvailableUser.AddRange(userRepository.GetAll(visible: true).Select(d => new SelectListItem()
            {
                Text = d.LastName + ' ' + d.FirstName,
                Value = d.Id.ToString()
            }));
            call.UserId = new Guid(User.Identity.GetUserId());

            var userRole = userRoleRepository.GetAll(userIds: new Guid(User.Identity.GetUserId())).FirstOrDefault();
            var role = roleRepository.GetRoleById(userRole.RoleId);
            ViewBag.RoleName = role.RoleName;

            return View(call);
        }
        [HttpPost]
        public JsonResult GetListCallListCallPotential(DataSourceRequest dsRequest, Call call)
        {
            if (call.UserId != Guid.Empty)
            {
                var data = callRepository.GetAllCallPotential(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), DTV: call.UserId);
                return data.ToJsonDataSource();
            }
            else
            {
                var data = callRepository.GetAllCallPotential(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject());
                return data.ToJsonDataSource();
            }
        }
        #endregion

        ////public ActionResult NewCall(string phone, int sourceId, string agent, string cisId)
        ////{
        ////    var model = new CallSurvey();
        ////    model = customerRepository.GetCustomerByMobilePhone(phone, sourceId);
        ////    if (model == null)
        ////        return AccessDeniedView();

        ////    model.AvailableCustomerFieldValue = customerFieldRepository.GetCustomerFieldAndValueByCustomerID(customerID: model.CustomerID, projectID: CommonHelper.CurrentProject()).Select(d => new CustomerFieldDTO()
        ////    {
        ////        FieldCode = d.FieldCode,
        ////        FieldName = d.FieldName,
        ////        FieldValue = d.FieldValue
        ////    }).ToList();

        ////    var project = projectsRepository.GetProjectsById(CommonHelper.CurrentProject());
        ////    var question = questionRepository.GetQuestionByProjectId(projectId: CommonHelper.CurrentProject(), visiable: true);
        ////    foreach (var ques in question)
        ////    {
        ////        var survey = surveyRepository.GetSurveyByQuestionId(projectID: CommonHelper.CurrentProject(), questionID: ques.QuestionID);
        ////        foreach (var sur in survey)
        ////        {
        ////            sur.NextQuestionID = questionsSurveyRepository.GetAll(projectID: CommonHelper.CurrentProject(), questionID: ques.QuestionID, surveyID: sur.SurveyID).FirstOrDefault().NextQuestionID;
        ////        }
        ////        ques.AvailableSurveys = survey.ToList();
        ////    }
        ////    model.AvailableQuestions = question.ToList();

        ////    model.CampaignConclusion = project.Conclusion;
        ////    model.CampaignGreeting = project.Greeting;
        ////    #region  get call status of Project
        ////    var lstStatus = statusCallRepository.GetAllStatus();
        ////    if (lstStatus != null)
        ////    {
        ////        model.AvailableStatus.AddRange(statusCallRepository.GetAllStatus().Select(d => new SelectListItem()
        ////        {
        ////            Text = d.Name,
        ////            Value = d.StatusID.ToString()
        ////        }));


        ////        int firstStatusId = lstStatus.First().StatusID;
        ////        //model.AvailableStatusCall.Add(new SelectListItem() { Text = "-Chọn tình trạng-", Value = "" });
        ////        model.AvailableStatusCall.AddRange(statusCallRepository.GetStatusCallByStatusId(firstStatusId).Select(d => new SelectListItem()
        ////        {
        ////            Text = d.Name,
        ////            Value = d.StatusCallID.ToString()
        ////        }));
        ////    }
        ////    #endregion

        ////    model.AgentCode = agent;
        ////    model.CisId = string.IsNullOrEmpty(cisId) ? 0 : Convert.ToInt64(cisId); 
        ////    return View(model);

        ////}

        ////[HttpPost]
        ////[ValidateInput(false)]
        ////public ActionResult NewCall(FormCollection form)
        ////{
        ////    //Lưu thông tin khách hàng
        ////    var customer = new Customer();
        ////    customer.CustomerID = new Guid(form["CustomerID"]);
        ////    //customer.MobilePhone = form["MobilePhone"];
        ////    customer.Visiable = true;
        ////    customer.SourceID = Convert.ToInt32(form["SourceID"]);
        ////    //customerRepository.UpdateCustomer(customer);
        ////    customerRepository.UpdateProjectCustomer(customer: customer, projectId: CommonHelper.CurrentProject(), callBy: new Guid(User.Identity.GetUserId()), isCall: true);

        ////    #region All fields of this call
        ////    var fields = new NameValueCollection();
        ////    foreach (var item in form)
        ////    {
        ////        var customerField = customerFieldRepository.GetAll(fieldcode: item.ToString()).FirstOrDefault();
        ////        if (customerField != null)
        ////        {
        ////            var customerFieldValue = new CustomerFieldValue();
        ////            customerFieldValue.CustomerID = new Guid(form["CustomerID"]);
        ////            customerFieldValue.CustomerFieldID = customerField.CustomerFieldID;
        ////            customerFieldValue.FieldValue = form[item.ToString()];
        ////            customerFieldValueRepository.UpdateCustomerFieldValue(customerFieldValue);

        ////        }
        ////    }
        ////    #endregion

        ////    var statusCall = statusCallRepository.GetStatusCall(Convert.ToInt32(form["StatusCallID"]));
        ////    #region new call
        ////    var getCall = callRepository.GetCallByProjectAndCustomer(projectId: CommonHelper.CurrentProject(), customerId: new Guid(form["CustomerID"]));
        ////    if (getCall.Count() == 0)
        ////    {
        ////        //Lưu thông tin cuộc gọi           
        ////        var call = new Call();
        ////        call.ProjectID = CommonHelper.CurrentProject();
        ////        call.UserId = new Guid(User.Identity.GetUserId());
        ////        call.CustomerID = new Guid(form["CustomerID"]);
        ////        call.StatusCallID = statusCall.StatusCallID;
        ////        if (statusCall.StatusID == (int)StatusCallEnum.Success || statusCall.StatusID == (int)(StatusCallEnum.Potential) || statusCall.StatusID == (int)(StatusCallEnum.Recall))
        ////            call.IsSuccess = true;
        ////        else
        ////            call.IsSuccess = false;
        ////        var callID = callRepository.AddNewCall(call);
        ////        if (callID > 0)
        ////        {
        ////            #region update calllog - lich sử cuộc gọi
        ////            var callLog = new CallLog();
        ////            callLog.CallID = callID;
        ////            callLog.ProjectID = CommonHelper.CurrentProject();
        ////            callLog.UserId = new Guid(User.Identity.GetUserId());
        ////            callLog.CustomerID = new Guid(form["CustomerID"]);
        ////            callLog.StatusCallID = statusCall.StatusCallID;
        ////            if (string.IsNullOrEmpty(form["RecallDate"]))
        ////                callLog.RecallDate = null;
        ////            else
        ////                callLog.RecallDate = DateTime.Parse(form["RecallDate"]);

        ////            callLog.Note = form["Note"];

        ////            if (statusCall.StatusID == (int)StatusCallEnum.Success ||
        ////                statusCall.StatusID == (int)(StatusCallEnum.Potential) ||
        ////                statusCall.StatusID == (int)(StatusCallEnum.Recall))
        ////                callLog.IsSuccess = true;
        ////            else
        ////                callLog.IsSuccess = false;

        ////            callLog.AgentCode = form["AgentCode"];
        ////            callLog.CisId = string.IsNullOrEmpty(form["CisId"].ToString()) ? 0 : Convert.ToInt64(form["CisId"]);

        ////            var callLogId = callLogRepository.AddNewCallLog(callLog);

        ////            //fields.Add("status_message", form["Note"]);
        ////            #endregion

        ////            #region Update vào Project Customer
        ////            //DTV ko dc chia KH tử đầu => update lại vào ProjectCustomer để biết KH của DTV nào

        ////            #endregion

        ////            #region Lưu câu hỏi và trả lời sau khi khảo sát
        ////            var question = questionRepository.GetQuestionByProjectId(projectId: CommonHelper.CurrentProject(), visiable: true);
        ////            foreach (var que in question)
        ////            {
        ////                var listSurvey = surveyRepository.GetSurveyByQuestionId(projectID: CommonHelper.CurrentProject(), questionID: que.QuestionID).Where(s => s.SurveyType == (int)TypeSurvey.TEXTBOX).ToList();
        ////                var answer = form.GetValues("Question_" + que.QuestionID + "[]");
        ////                if (answer != null)
        ////                {
        ////                    foreach (var id in answer)
        ////                    {
        ////                        var surveyAnswer = new SurveyAnswer();
        ////                        surveyAnswer.CallLogID = callLogId;
        ////                        surveyAnswer.QuestionID = que.QuestionID;

        ////                        int number;
        ////                        bool isNumeric = int.TryParse(id, out number);
        ////                        if (isNumeric)
        ////                        {
        ////                            surveyAnswer.SurveyID = number;
        ////                            surveyAnswer.SurveyContent = "1";
        ////                        }
        ////                        else
        ////                        {
        ////                            surveyAnswer.SurveyID = listSurvey.FirstOrDefault().SurveyID;
        ////                            surveyAnswer.SurveyContent = id;

        ////                            listSurvey.Remove(listSurvey.FirstOrDefault());
        ////                        }

        ////                        surveyAnswerRepository.AddNewSurveyAnswer(surveyAnswer);
        ////                    }
        ////                }
        ////            }
        ////            #endregion
        ////        }

        ////        return Json(new { Result = true, Message = "Cập nhật thành công" });
        ////    }
        ////    else
        ////    {
        ////        return Json(new { Result = false, Message = "Khách hàng này đã được khảo sát" });
        ////    }
        ////    #endregion
        ////}

    }
}
