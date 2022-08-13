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
    public class ProjectController : MBController
    {
        #region Fields

        [Dependency]
        public IProjectsRepository projectsRepository { get; set; }

        [Dependency]
        public ICampaignsRepository campaignsRepository { get; set; }

        [Dependency]
        public ICustomerRepository customerRepository { get; set; }

        [Dependency]
        public ISourcesRepository sourcesRepository { get; set; }

        [Dependency]
        public IProjectCustomerRepository projectCustomerRepository { get; set; }

        [Dependency]
        public IProjectUsersRepository projectUsersRepository { get; set; }

        [Dependency]
        public IUserRepository userRepository { get; set; }

        [Dependency]
        public IUserRoleRepository userRoleRepository { get; set; }

        [Dependency]
        public IRoleRepository roleRepository { get; set; }

        [Dependency]
        public IStatusCallRepository statusCallRepository { get; set; }

        [Dependency]
        public IProjectStatusCallRepository projectStatusCallRepository { get; set; }

        [Dependency]
        public ICustomerFieldRepository customerFieldRepository { get; set; }

        [Dependency]
        public IProjectCustomerFieldRepository projectCustomerFieldRepository { get; set; }

        [Dependency]
        public IQuestionRepository questionRepository { get; set; }

        [Dependency]
        public IProjectQuestionsRepository projectQuestionsRepository { get; set; }

        [Dependency]
        public IQuestionsSurveyRepository questionsSurveyRepository { get; set; }

        [Dependency]
        public ICallLogRepository callLogRepository { get; set; }

        [Dependency]
        public ICallRepository callRepository { get; set; }

        [Dependency]
        public ISurveyRepository surveyRepository { get; set; }
        #endregion        

        #region List Projects
        //[PermissionAuthorize(Permissions.XemDanhSachDuAn)]
        public ActionResult ListProjects()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemDanhSachDuAn.ToString()))
                return AccessDeniedView();

            var project = new ProjectsDTO();
            project.AvailableCampaign.Add(new SelectListItem()
            {
                Text = "Tất cả",
                Value = "0",
                Selected = true
            });
            project.AvailableCampaign.AddRange(campaignsRepository.GetAll(isDeleted: false).Select(d => new SelectListItem()
            {
                Text = d.Name,
                Value = d.CampaignID.ToString()
            }));
            
            return View(project);
        }

        [HttpPost]
        public JsonResult GetListProjects(DataSourceRequest dsRequest, ProjectsDTO projects, MB.Common.Kendoui.Filter filter = null)
        {
            int? campaignID = null;
            if (projects.CampaignID != 0)
                campaignID = projects.CampaignID;

            string name = null;
            string code = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("Name")).Any())
                    name = ((string[])arrayFilter.Where(f => f.Field == "Name").FirstOrDefault().Value)[0];

                if (arrayFilter.Where(f => f.Field.Contains("Code")).Any())
                    code = ((string[])arrayFilter.Where(f => f.Field == "Code").FirstOrDefault().Value)[0];
            }


            var data = new DataSourceResult();
            var userRole = userRoleRepository.GetAll(userIds: new Guid(User.Identity.GetUserId())).FirstOrDefault();
            var role = roleRepository.GetRoleById(userRole.RoleId);
            if (role.RoleName == Role.Administrator.ToString())
            {
                data = projectsRepository.GetProjectsDatasource(dsRequest: dsRequest, campaignID: campaignID, name: name, code: code);
            }
            else
            {
                data = projectsRepository.GetProjectsByUserDatasource(new Guid(User.Identity.GetUserId()));
            }
            return data.ToJsonDataSource();
        }

        #endregion        

        #region Add Projects

        public ActionResult AddProjects()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.ThemDuAn.ToString()))
                return AccessDeniedView();

            var model = new ProjectsDTO();
            model.AvailableCampaign.AddRange(campaignsRepository.GetAll(isDeleted: false).Select(d => new SelectListItem()
            {
                Text = d.Name,
                Value = d.CampaignID.ToString()
            }));
            
            return View(model);
        }

        [HttpPost]
        public ActionResult AddProjects(ProjectsDTO model)
        {
            model.IsDeleted = false;
            var result = projectsRepository.AddNewProjects(model);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Thêm mới dự án không thành công!",
                    Title = "Thêm mới không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Thêm mới dự án thành công!",
                Title = "Thêm mới thành công",
                URLList = "/OMS/Project/ListProjects",
                Id = result.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion       

        #region Edit Projects

        public ActionResult EditProjects(int id)
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.SuaDuAn.ToString()))
                return AccessDeniedView();

            /*
            var projects = userRepository.GetProjectByUser(new Guid(User.Identity.GetUserId()));
            if (!projects.Any(p => p.ProjectID == CommonHelper.CurrentProject()))
                return AccessDeniedView();
            */        

            var project = projectsRepository.GetProjectsById(id);
            if (project == null)
                return RedirectToAction("ListProjects");

            project.AvailableCampaign.AddRange(campaignsRepository.GetAll(isDeleted: false).Select(d => new SelectListItem()
            {
                Text = d.Name,
                Value = d.CampaignID.ToString()
            }));

            project.AvailableSource.Add(new SelectListItem()
            {
                Text = "--Chọn nguồn--",
                Value = "0",
                Selected = true
            });
            project.AvailableSource.AddRange(sourcesRepository.GetAll().Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.SourceID.ToString()
            }));

            var question = questionRepository.GetQuestionByProjectId(projectId: id, visiable: true);
            foreach (var ques in question)
            {
                var survey = surveyRepository.GetSurveyByQuestionId(projectID: id, questionID: ques.QuestionID);
                foreach (var sur in survey)
                {
                    sur.NextQuestionID = questionsSurveyRepository.GetAll(projectID: id, questionID: ques.QuestionID, surveyID: sur.SurveyID).FirstOrDefault().NextQuestionID;
                }
                ques.AvailableSurveys = survey.ToList();
            }
            project.AvailableQuestions = question.ToList();

            var listCustomerFeild = customerFieldRepository.GetCustomerFieldByProject(id);

            var arr1 = new List<string>();
            arr1.Add("[0].Value");
            for (int i = 0; i < listCustomerFeild.Count(); i++)
            {
                int kq = i + 7;
                arr1.Add("[" + kq + "].Value");
            }
            ViewBag.ListCustomerField1 = arr1;

            var arr2= new List<string>();
            arr2.Add("[0].Value");
            for (int i = 0; i < listCustomerFeild.Count(); i++)
            {
                int kq = i + 9;
                arr2.Add("[" + kq + "].Value");
            }
            ViewBag.ListCustomerField2 = arr2;
             
            return View(project);
        }

        [HttpPost]
        public ActionResult EditProjects(ProjectsDTO model)
        {
            var project = projectsRepository.GetProjectsById(model.ProjectID);
            if (project == null)
                return RedirectToAction("ListProjects");

            var result = projectsRepository.UpdateProjects(model);
            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Cập nhật dự án không thành công!",
                    Title = "Cập nhật không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Cập nhật dự án thành công!",
                Title = "Cập nhật thành công",
                URLEdit = "/OMS/Project/EditProjects/" + model.ProjectID,
                Id = model.ProjectID.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion        

        #region Delete Projects

        [HttpPost]
        public ActionResult DeleteProjects(int id)
        {
            var project = projectsRepository.GetProjectsById(id);
            if (project == null)
                return RedirectToAction("ListProjects");

            var result = projectsRepository.DeleteProjects(id);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Xóa dự án không thành công!",
                    Title = "Xóa không thành công"
                }, JsonRequestBehavior.DenyGet);

            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa dự án thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/Project/ListProjects"
            }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    projectsRepository.DeleteProjects(item);
                }
            }
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa dự án thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/Project/ListProjects"
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion        

        #region Customer Sharing Project

        [HttpPost]
        public JsonResult GetListCustomer(DataSourceRequest dsRequest, ProjectsDTO project)
        {
            int? sourceID = null;

            if (project.SearchSourceID1 > 0)
            {
                sourceID = project.SearchSourceID1;
            }

            var data = customerRepository.CustomerGetAllFieldValueNotInProjectDatasource(dsRequest: dsRequest, projectID: project.ProjectID, sourceID: sourceID);

            return data.ToJsonDataSource();
        }

        [HttpPost]
        public JsonResult GetListCustomerInProject(DataSourceRequest dsRequest, ProjectsDTO project)
        {
            int? sourceID = null;

            if (project.SearchSourceID2 > 0)
            {
                sourceID = project.SearchSourceID2;
            }

            var data = customerRepository.ProjectCustomerGetAllFieldValueDatasource(dsRequest: dsRequest, projectID: project.ProjectID, sourceID: sourceID);

            return data.ToJsonDataSource();
        }

        [HttpPost]
        public ActionResult ShareCustomerSelected(ICollection<Guid> selectedIds, int projectID)
        {
            if (selectedIds != null)
            {
                foreach(var itemt in selectedIds)
                {
                    var projectCustomer = new ProjectCustomer();
                    projectCustomer.CustomerID = itemt;
                    projectCustomer.ProjectID = projectID;
                    projectCustomer.CallBy = Guid.Empty;
                    projectCustomer.IsCall = false;
                    projectCustomerRepository.AddNewProjectCustomer(projectCustomer);
                }
            }

            return Json(new { Result = true });
        }
        [HttpPost]
        public ActionResult ShareCustomerAll(int projectID, int sourceID)
        {
            if (projectID != null)
            {
                if (sourceID > 0)
                {
                    var data = customerRepository.CustomerGetAllFieldValueNotInProject(projectID, sourceID).ToList();
                    foreach (var item in data)
                    {
                        var projectCustomer = new ProjectCustomer();
                        projectCustomer.CustomerID = item.CustomerID;
                        projectCustomer.ProjectID = projectID;
                        projectCustomer.CallBy = Guid.Empty;
                        projectCustomer.IsCall = false;
                        projectCustomerRepository.AddNewProjectCustomer(projectCustomer);
                    }
                    return Json(new { Result = true });

                }
                else
                {
                    var data = customerRepository.CustomerGetAllFieldValueNotInProject(projectID).ToList();
                    foreach (var item in data)
                    {
                        var projectCustomer = new ProjectCustomer();
                        projectCustomer.CustomerID = item.CustomerID;
                        projectCustomer.ProjectID = projectID;
                        projectCustomer.CallBy = Guid.Empty;
                        projectCustomer.IsCall = false;
                        projectCustomerRepository.AddNewProjectCustomer(projectCustomer);
                    }
                    return Json(new { Result = true });
                }
            }

            return Json(new { Result = true });
        }
        [HttpPost]
        public ActionResult DestroyCustomerSelected(ICollection<Guid> selectedIds, int projectID)
        {
            if (selectedIds != null)
            {
                foreach (var itemt in selectedIds)
                {
                    //Xóa CustomerID trong bảng CallLog
                    callLogRepository.DeleteCallLogByCustomer(projectID: projectID, customerID: itemt);
                    //Xóa CustomerID trong bảng Call
                    callRepository.DeleteCallByCustomer(projectID: projectID, customerID: itemt);
                    //Xóa CustomerID trong bảng phân chia khách hàng cho dự án(ProjectCustomer)
                    projectCustomerRepository.DeleteProjectCustomer(projectID: projectID, customerID: itemt);
                }
            }

            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Chuyển khách hàng về nguồn thành công!",
                Title = "Chuyển thành công"
            }, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public ActionResult DestroyCustomerAll(int projectID)
        {
            if (projectID != null && projectID > 0)
            {
                var data = customerRepository.ProjectCustomerGetAllFieldValue(projectID);
                foreach (var itemt in data)
                {
                    //Xóa CustomerID trong bảng CallLog
                    callLogRepository.DeleteCallLogByCustomer(projectID: projectID, customerID: itemt.CustomerID);
                    //Xóa CustomerID trong bảng Call
                    callRepository.DeleteCallByCustomer(projectID: projectID, customerID: itemt.CustomerID);
                    //Xóa CustomerID trong bảng phân chia khách hàng cho dự án(ProjectCustomer)
                    projectCustomerRepository.DeleteProjectCustomer(projectID: projectID, customerID: itemt.CustomerID);
                }
            }

            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Chuyển khách hàng về nguồn thành công!",
                Title = "Chuyển thành công"
            }, JsonRequestBehavior.DenyGet);
        }
        
        #endregion

        #region User Sharing Project

        [HttpPost]
        public JsonResult GetListUser(DataSourceRequest dsRequest, ProjectsDTO project, MB.Common.Kendoui.Filter filter = null)
        {
            string userName = null;
            string fullName = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("UserName")).Any())
                    userName = ((string[])arrayFilter.Where(f => f.Field == "UserName").FirstOrDefault().Value)[0];

                if (arrayFilter.Where(f => f.Field.Contains("FullName")).Any())
                    fullName = ((string[])arrayFilter.Where(f => f.Field == "FullName").FirstOrDefault().Value)[0];
            }
            var data = userRepository.GetUserNotInProjectDatasource(dsRequest: dsRequest, projectID: project.ProjectID, userName: userName, fullName: fullName);
            return data.ToJsonDataSource();
        }

        [HttpPost]
        public JsonResult GetListUserInProject(DataSourceRequest dsRequest, ProjectsDTO project, MB.Common.Kendoui.Filter filter = null)
        {
            string userName = null;
            string fullName = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("UserName")).Any())
                    userName = ((string[])arrayFilter.Where(f => f.Field == "UserName").FirstOrDefault().Value)[0];

                if (arrayFilter.Where(f => f.Field.Contains("FullName")).Any())
                    fullName = ((string[])arrayFilter.Where(f => f.Field == "FullName").FirstOrDefault().Value)[0];
            }
            var data = userRepository.GetUserByProjectDatasource(dsRequest: dsRequest, projectID: project.ProjectID, userName: userName, fullName: fullName);

            return data.ToJsonDataSource();
        }

        [HttpPost]
        public ActionResult ShareUserSelected(ICollection<Guid> selectedIds, int projectID)
        {
            if (selectedIds != null)
            {
                foreach (var itemt in selectedIds)
                {
                    var projectUsers = new ProjectUsers();
                    projectUsers.UserId = itemt;
                    projectUsers.ProjectID = projectID;
                    projectUsersRepository.AddNewProjectUsers(projectUsers);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult DestroyUserSelected(ICollection<Guid> selectedIds, int projectID)
        {
            if (selectedIds != null)
            {
                foreach (var itemt in selectedIds)
                {
                    projectUsersRepository.DeleteProjectUsers(projectID: projectID, usersId: itemt);
                }
            }

            return Json(new { Result = true });
        }

        #endregion

        #region StatusCall Sharing Project

        [HttpPost]
        public JsonResult GetListStatusCall(DataSourceRequest dsRequest, ProjectsDTO project, MB.Common.Kendoui.Filter filter = null)
        {
            string name = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("Name")).Any())
                    name = ((string[])arrayFilter.Where(f => f.Field == "Name").FirstOrDefault().Value)[0];
            }
            var data = statusCallRepository.GetStatusCallNotInProjectDatasource(dsRequest: dsRequest, projectID: project.ProjectID, name: name);

            return data.ToJsonDataSource();
        }

        [HttpPost]
        public JsonResult GetListStatusCallInProject(DataSourceRequest dsRequest, ProjectsDTO project, MB.Common.Kendoui.Filter filter = null)
        {
            string name = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("Name")).Any())
                    name = ((string[])arrayFilter.Where(f => f.Field == "Name").FirstOrDefault().Value)[0];
            }
            var data = statusCallRepository.GetStatusCallForProjectDatasource(dsRequest: dsRequest, projectID: project.ProjectID, name: name);

            return data.ToJsonDataSource();
        }

        [HttpPost]
        public ActionResult ShareStatusCallSelected(ICollection<int> selectedIds, int projectID)
        {
            if (selectedIds != null)
            {
                foreach (var itemt in selectedIds)
                {
                    var projectStatusCall = new ProjectStatusCall();
                    projectStatusCall.StatusCallID = itemt;
                    projectStatusCall.ProjectID = projectID;
                    projectStatusCallRepository.AddNewProjectStatusCall(projectStatusCall);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult DestroyStatusCallSelected(ICollection<int> selectedIds, int projectID)
        {
            if (selectedIds != null)
            {
                foreach (var itemt in selectedIds)
                {
                    projectStatusCallRepository.DeleteProjectStatusCall(projectID: projectID, statusID: itemt);
                }
            }

            return Json(new { Result = true });
        }

        #endregion

        #region CustomerField Sharing Project

        [HttpPost]
        public JsonResult GetListCustomerField(DataSourceRequest dsRequest, ProjectsDTO project, MB.Common.Kendoui.Filter filter = null)
        {
            string fieldName = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("FieldName")).Any())
                    fieldName = ((string[])arrayFilter.Where(f => f.Field == "FieldName").FirstOrDefault().Value)[0];
            }
            var data = customerFieldRepository.GetCustomerFieldNotInForProjectDatasource(dsRequest: dsRequest, projectID: project.ProjectID, fieldname: fieldName);

            return data.ToJsonDataSource();
        }

        [HttpPost]
        public JsonResult GetListCustomerFieldInProject(DataSourceRequest dsRequest, ProjectsDTO project, MB.Common.Kendoui.Filter filter = null)
        {
            string fieldName = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("FieldName")).Any())
                    fieldName = ((string[])arrayFilter.Where(f => f.Field == "FieldName").FirstOrDefault().Value)[0];
            }
            var data = customerFieldRepository.GetCustomerFieldForProjectDatasource(dsRequest: dsRequest, projectID: project.ProjectID, fieldname: fieldName);

            return data.ToJsonDataSource();
        }

        [HttpPost]
        public ActionResult ShareCustomerFieldSelected(ICollection<int> selectedIds, int projectID)
        {
            if (selectedIds != null)
            {
                foreach (var itemt in selectedIds)
                {
                    var projectCustomerField = new ProjectCustomerField();
                    projectCustomerField.CustomerFieldID = itemt;
                    projectCustomerField.ProjectID = projectID;
                    projectCustomerFieldRepository.AddNewProjectCustomerField(projectCustomerField);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult DestroyCustomerFieldSelected(ICollection<int> selectedIds, int projectID)
        {
            if (selectedIds != null)
            {
                foreach (var itemt in selectedIds)
                {
                    projectCustomerFieldRepository.DeleteProjectCustomerField(projectID: projectID, customerFieldID: itemt);
                }
            }

            return Json(new { Result = true });
        }
        #endregion

        #region Question Sharing Project

        [HttpPost]
        public JsonResult GetListQuestion(DataSourceRequest dsRequest, ProjectsDTO project, MB.Common.Kendoui.Filter filter = null)
        {
            string tieuDeNgan = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("TieuDeNgan")).Any())
                    tieuDeNgan = ((string[])arrayFilter.Where(f => f.Field == "TieuDeNgan").FirstOrDefault().Value)[0];
            }
            var data = questionRepository.GetQuestionNotInForProjectDatasource(dsRequest: dsRequest, projectID: project.ProjectID, name: tieuDeNgan, visiable: true);

            return data.ToJsonDataSource();
        }

        [HttpPost]
        public JsonResult GetListQuestionInProject(DataSourceRequest dsRequest, ProjectsDTO project, MB.Common.Kendoui.Filter filter = null)
        {
            string tieuDeNgan = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("TieuDeNgan")).Any())
                    tieuDeNgan = ((string[])arrayFilter.Where(f => f.Field == "TieuDeNgan").FirstOrDefault().Value)[0];
            }

            var data = questionRepository.GetQuestionForProjectDatasource(dsRequest: dsRequest, projectID: project.ProjectID, name: tieuDeNgan, visiable: true);

            return data.ToJsonDataSource();
        }

        [HttpPost]
        public ActionResult ShareQuestionSelected(ICollection<int> selectedIds, int projectID)
        {
            if (selectedIds != null)
            {
                var count = projectQuestionsRepository.GetAll(projectID: projectID).Count();
                for(int i = 0 ; i < selectedIds.Count; i++)
                {
                    var projectQuestions = new ProjectQuestions();
                    projectQuestions.QuestionID = selectedIds.ElementAt(i);
                    projectQuestions.ProjectID = projectID;
                    projectQuestions.DisplayOrder = count + 1 + i;
                    projectQuestionsRepository.AddNewProjectQuestions(projectQuestions);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult DestroyQuestionSelected(ICollection<int> selectedIds, int projectID)
        {
            if (selectedIds != null)
            {
                foreach (var itemt in selectedIds)
                {
                    projectQuestionsRepository.DeleteProjectQuestions(projectID: projectID, questionID: itemt);
                    questionsSurveyRepository.DeleteQuestionsSurvey(projectID: projectID, questionID: itemt);
                }
            }

            return Json(new { Result = true });
        }
        #endregion

        #region Customer Sharing User

        public ActionResult CustomerSharingUser()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.ChiaKHChoDTV.ToString()))
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

            customer.AvailableUser.AddRange(userRepository.GetUserByProject(projectID: CommonHelper.CurrentProject()).Select(ct => new SelectListItem()
            {
                Text = ct.FirstName + " " + ct.LastName,
                Value = ct.Id.ToString()
            }));

            return View(customer);
        }

        [HttpPost]
        public ActionResult CustomerSharingUser(CustomerDTO customer)
        {
            var listCustomer = customerRepository.GetAllCustomerByIsCallNot(projectID: CommonHelper.CurrentProject(), sourceID: customer.SourceID);

            List<Guid> listUserId = new List<Guid>();
            foreach (var item in customer.UserId)
            {
                listUserId.Add(item);
            }

            foreach (var i in listCustomer.Take(customer.CountCustomer))
            {
                var projectCustomer = projectCustomerRepository.GetProjectCustomer(projectID: CommonHelper.CurrentProject(), customerID: i.CustomerID).FirstOrDefault();
                if (projectCustomer != null)
                {
                    projectCustomer.UpdatedDate = DateTime.Now;
                    projectCustomer.CallBy = listUserId.FirstOrDefault();
                    projectCustomerRepository.UpdateProjectCustomer(projectCustomer);
                }

                listUserId.Remove(listUserId.FirstOrDefault());
                if (listUserId.Count() == 0)
                {
                    foreach (var item in customer.UserId)
                    {
                        listUserId.Add(item);
                    }
                }
            }

            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Phân chia KH cho ĐTV thành công!",
                Title = "Phân chia thành công",
                URLEdit = "/OMS/Project/CustomerSharingUser"
            }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult SearchSource(int sourceId)
        {
            var TotalCustomer = customerRepository.GetAllCustomerByIsCallNot(projectID: CommonHelper.CurrentProject(), sourceID: sourceId).Count();

            return Json(new { TotalCustomer, success = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Reorder
        [HttpPost]
        public ActionResult ReOrder(int projectID,int cur_Item, int pre_Item)
        {
            var projectQuestionCur = projectQuestionsRepository.GetAll(projectID, cur_Item).FirstOrDefault();
            if (pre_Item > 0)
            {
                var projectQuestionPre = projectQuestionsRepository.GetAll(projectID, pre_Item).FirstOrDefault();
                if (projectQuestionPre != null)
                {
                    int Temp = projectQuestionCur.DisplayOrder;
                    //Cập nhật displayorder câu hỏi hiện tại
                    projectQuestionCur.DisplayOrder = projectQuestionPre.DisplayOrder;
                    projectQuestionsRepository.UpdateProjectQuestions(projectQuestionCur);
                    //Cập nhật displayorder câu hỏi kế tiếp hoặc kế sau
                    projectQuestionPre.DisplayOrder = Temp;
                    projectQuestionsRepository.UpdateProjectQuestions(projectQuestionPre);

                }
            }

            return Json(new {success = true }, JsonRequestBehavior.AllowGet);      
        }
        [HttpPost]
        
        public ActionResult ReOrderSurvey(int projectID, int cur_Survey, int pre_Survey,int questionID)
        {
            var projectSurveyCur = questionsSurveyRepository.GetAll(projectID:projectID, questionID:questionID,surveyID:cur_Survey).FirstOrDefault();
            if (pre_Survey > 0)
            {
                var projectSurveyPre = questionsSurveyRepository.GetAll(projectID: projectID, questionID: questionID, surveyID: pre_Survey).FirstOrDefault();
                if (projectSurveyPre != null)
                {
                    int Temp = projectSurveyCur.DisplayOrder;
                    //Cập nhật displayorder câu hỏi hiện tại
                    projectSurveyCur.DisplayOrder = projectSurveyPre.DisplayOrder;
                    questionsSurveyRepository.UpdateQuestionsSurvey(projectSurveyCur);
                    //Cập nhật displayorder câu hỏi kế tiếp hoặc kế sau
                    projectSurveyPre.DisplayOrder = Temp;
                    questionsSurveyRepository.UpdateQuestionsSurvey(projectSurveyPre);

                }
            }

            return Json(new {success = true }, JsonRequestBehavior.AllowGet);      
        }
        
        #endregion

        #region Forward Customer For User

        public ActionResult ForwardCustomerForUser()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.ChuyenKHChoDTV.ToString()))
                return AccessDeniedView();

            var customer = new CustomerDTO();

            customer.AvailableUser.Add(new SelectListItem()
            {
                Text = "--Chọn điện thoại viên--",
                Value = "0",
                Selected = true
            });
            customer.AvailableUser.AddRange(userRepository.GetUserByProject(projectID: CommonHelper.CurrentProject()).Select(ct => new SelectListItem()
            {
                Text = ct.FirstName + " " + ct.LastName,
                Value = ct.Id.ToString()
            }));

            return View(customer);
        }

        [HttpPost]
        public ActionResult ForwardCustomerForUser(CustomerDTO customer)
        {
            foreach(var cus in customer.ListCustomerID)
            {
                var projectCustomer = projectCustomerRepository.GetProjectCustomer(projectID: CommonHelper.CurrentProject(), customerID: cus).Where(p => p.CallBy == customer.UserId1).FirstOrDefault();
                projectCustomer.CallBy = customer.UserId2;
                projectCustomer.UpdatedDate = DateTime.Now;
                projectCustomerRepository.UpdateProjectCustomer(projectCustomer);
                //update lại userid trong table call
                var callCustomer = callRepository.GetCallByProjectAndCustomer(projectId: CommonHelper.CurrentProject(), customerId: cus).FirstOrDefault();
                if (callCustomer != null)
                {
                    callCustomer.UserId = customer.UserId2;
                    callRepository.UpdateCall(callCustomer);
                }
            }
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Chuyển KH cho ĐTV thành công!",
                Title = "Chuyển KH thành công",
                URLEdit = "/OMS/Project/ForwardCustomerForUser"
            }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult SearchUser(DataSourceRequest dsRequest, CustomerDTO customer)
        {
            var data = callLogRepository.GetCustomerByUserDatasource(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), callBy: customer.UserId1);
            return data.ToJsonDataSource();
        }
        #endregion

    }
}
