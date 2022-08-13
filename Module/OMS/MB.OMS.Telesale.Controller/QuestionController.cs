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
    public class QuestionController : MBController
    {
        #region Feilds

        [Dependency]
        public IQuestionRepository questionRepository { get; set; }

        [Dependency]
        public IProjectsRepository projectsRepository { get; set; }

        [Dependency]
        public IProjectQuestionsRepository projectQuestionsRepository { get; set; }

        [Dependency]
        public ISurveyRepository surveyRepository { get; set; }

        [Dependency]
        public IQuestionsSurveyRepository questionsSurveyRepository { get; set; }

        [Dependency]
        public IUserRepository userRepository { get; set; }

        [Dependency]
        public IUserRoleRepository userRoleRepository { get; set; }

        [Dependency]
        public IRoleRepository roleRepository { get; set; }

        #endregion        

        #region List Questions
        //[PermissionAuthorize(Permissions.XemDanhSachCauHoi)]
        public ActionResult ListQuestions()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemDanhSachCauHoi.ToString()))
                return AccessDeniedView();

            var question = new Question();          
            return View(question);
        }

        [HttpPost]
        public JsonResult GetListQuestions(DataSourceRequest dsRequest, MB.Common.Kendoui.Filter filter = null)
        {
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
                data = questionRepository.GetQuestionDatasource(dsRequest: dsRequest, name: name, code: code);
            }
            else
            {
                data = questionRepository.GetQuestionForProjectDatasource(dsRequest: dsRequest,projectID: CommonHelper.CurrentProject(), name: name, visiable: true);
            }
           return data.ToJsonDataSource();
        }

        #endregion        

        #region Add Question

        public ActionResult AddQuestion()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.ThemCauHoi.ToString()))
                return AccessDeniedView();

            var model = new Question();
            
            return View(model);
        }

        [HttpPost]
        public ActionResult AddQuestion(Question model)
        {
            model.IsDeleted = false;
            var result = questionRepository.AddNewQuestion(model);

            var question = questionRepository.GetQuestionById(result);
            question.Code = "CH_" + question.QuestionID;
            questionRepository.UpdateQuestion(question);
          
            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Thêm mới câu hỏi không thành công!",
                    Title = "Thêm mới không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Thêm mới câu hỏi thành công!",
                Title = "Thêm mới thành công",
                URLList = "/OMS/Question/ListQuestions",
                Id = result.ToString()
            }, JsonRequestBehavior.DenyGet);

        }

        #endregion       

        #region Edit Question

        public ActionResult EditQuestion(int id)
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.SuaCauHoi.ToString()))
                return AccessDeniedView();

            var question = questionRepository.GetQuestionById(id);
            if (question == null)
                return RedirectToAction("ListQuestions");
            
            return View(question);
        }

        [HttpPost]
        public ActionResult EditQuestion(QuestionDTO model)
        {
            var question = questionRepository.GetQuestionById(model.QuestionID);
            if (question == null)
                return RedirectToAction("ListQuestions");

            var result = questionRepository.UpdateQuestion(model);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Cập nhật câu hỏi không thành công!",
                    Title = "Cập nhật không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Cập nhật câu hỏi thành công!",
                Title = "Cập nhật thành công",
                URLEdit = "/OMS/Question/EditQuestion/" + model.QuestionID,
                Id = model.QuestionID.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion        

        #region Delete Question

        [HttpPost]
        public ActionResult DeleteQuestion(int id)
        {
            var question = questionRepository.GetQuestionById(id);
            if (question == null)
                return RedirectToAction("ListQuestions");

            var result = questionRepository.DeleteQuestion(id);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Xóa câu hỏi không thành công!",
                    Title = "Xóa không thành công"
                }, JsonRequestBehavior.DenyGet);

            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa câu hỏi thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/Question/ListQuestions"
            }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    questionRepository.DeleteQuestion(item);
                }
            }
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa câu hỏi thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/Question/ListQuestions"
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion        

        #region Survey

        [HttpPost]
        public JsonResult GetListSurvey(DataSourceRequest dsRequest, SurveyDTO survey, MB.Common.Kendoui.Filter filter = null)
        {
            string noiDungNgan = null;
            string code = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("NoiDungNgan")).Any())
                    noiDungNgan = ((string[])arrayFilter.Where(f => f.Field == "NoiDungNgan").FirstOrDefault().Value)[0];

                if (arrayFilter.Where(f => f.Field.Contains("Code")).Any())
                    code = ((string[])arrayFilter.Where(f => f.Field == "Code").FirstOrDefault().Value)[0];
            }
            var data = surveyRepository.GetAllNotInQuestionsSurveyDatasource(dsRequest: dsRequest, projectID: survey.ProjectID, questionID: survey.QuestionID, codeSurveys: code, survey: noiDungNgan);

            return data.ToJsonDataSource();
        }

        [HttpPost]
        public JsonResult GetListSurveyInQuestion(DataSourceRequest dsRequest, SurveyDTO survey, MB.Common.Kendoui.Filter filter = null)
        {
            string noiDungNgan = null;
            string code = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("NoiDungNgan")).Any())
                    noiDungNgan = ((string[])arrayFilter.Where(f => f.Field == "NoiDungNgan").FirstOrDefault().Value)[0];

                if (arrayFilter.Where(f => f.Field.Contains("Code")).Any())
                    code = ((string[])arrayFilter.Where(f => f.Field == "Code").FirstOrDefault().Value)[0];
            }
            var data = surveyRepository.GetAllForQuestionsDatasource(dsRequest: dsRequest, projectID: survey.ProjectID, questionID: survey.QuestionID, codeSurveys: code, survey: noiDungNgan);

            return data.ToJsonDataSource();
        }

        public ActionResult SurveyShareQuestion(int id)
        {
            var question = questionRepository.GetQuestionById(id);
            var survey = new SurveyDTO();
            survey.QuestionID = question.QuestionID;

            return Json(new { success = true, html = RenderPartialViewToString("SurveyShareQuestion", survey) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ShareSurveySelected(ICollection<int> selectedIds, int questionId, int projectID)
        {
            if (selectedIds != null)
            {
                var count = questionsSurveyRepository.GetAll(projectID: projectID, questionID: questionId).Count();
                for (int i = 0; i < selectedIds.Count; i++ )
                {
                    var questionsSurvey = new QuestionsSurvey();
                    questionsSurvey.ProjectID = projectID;
                    questionsSurvey.SurveyID = selectedIds.ElementAt(i);
                    questionsSurvey.QuestionID = questionId;
                    questionsSurvey.DisplayOrder = count + 1 + i;
                    questionsSurveyRepository.AddNewQuestionsSurvey(questionsSurvey);
                }
            }

            return Json(new { Result = true });
        }

        [HttpPost]
        public ActionResult DestroySurveySelected(ICollection<int> selectedIds, int questionId, int projectID)
        {
            if (selectedIds != null)
            {
                foreach (var itemt in selectedIds)
                {
                    questionsSurveyRepository.DeleteQuestionsSurvey(projectID: projectID, questionID: questionId, surveyID: itemt);
                }
            }

            return Json(new { Result = true });
        }       

        #endregion

        #region NextQuestion

        public ActionResult NextQuestion(int projectId, int questionId, int surveyId)
        {
            var model = new SurveyDTO();
            model.AvailableNextQuestion.Add(new SelectListItem()
            {
                Text = "Kết thúc",
                Value = "0",
                Selected = true
            });
            var data = questionsSurveyRepository.GetAll(projectID: projectId, questionID: questionId, surveyID: surveyId).FirstOrDefault();

            model.NextQuestionID = data.NextQuestionID;
            model.SurveyID = surveyId;
            model.DisplayOrder = data.DisplayOrder;
            model.AvailableNextQuestion.AddRange(questionRepository.GetQuestionByProjectId(projectId: projectId, visiable: true).Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.QuestionID.ToString()
            }));
            return Json(new { success = true, html = RenderPartialViewToString("NextQuestion", model) }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddNextQuestion(QuestionsSurvey questionsSurvey)
        { 
            var result = questionsSurveyRepository.UpdateQuestionsSurvey(questionsSurvey);     
            if(result == 0)
                return Json(new { Result = false });
            else
                return Json(new { Result = true });
        }

        #endregion
       
    }
}
