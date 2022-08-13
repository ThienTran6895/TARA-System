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
    public class SurveyController : MBController
    {
        #region Feilds

        [Dependency]
        public IQuestionRepository questionRepository { get; set; }

        [Dependency]
        public IProjectsRepository projectsRepository { get; set; }

        [Dependency]
        public ISurveyRepository surveyRepository { get; set; }

        [Dependency]
        public IUserRepository userRepository { get; set; }

        [Dependency]
        public IUserRoleRepository userRoleRepository { get; set; }

        [Dependency]
        public IRoleRepository roleRepository { get; set; }

        #endregion

        #region List Survey
        //[PermissionAuthorize(Permissions.XemDanhSachDapAn)]
        public ActionResult ListSurvey()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemDanhSachDapAn.ToString()))
                return AccessDeniedView();

            var survey = new SurveyDTO();           
            return View(survey);
        }

        [HttpPost]
        public JsonResult GetListSurvey(MB.Common.Kendoui.DataSourceRequest dsRequest, MB.Common.Kendoui.Filter filter = null)
        {
            string surveyContent = null;
            string code = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("SurveyContent")).Any())
                    surveyContent = ((string[])arrayFilter.Where(f => f.Field == "SurveyContent").FirstOrDefault().Value)[0];

                if (arrayFilter.Where(f => f.Field.Contains("Code")).Any())
                    code = ((string[])arrayFilter.Where(f => f.Field == "Code").FirstOrDefault().Value)[0];
            }


            var data = surveyRepository.GetSurveyDatasource(dsRequest: dsRequest, surveyContent: surveyContent, code: code);
            return data.ToJsonDataSource();
        }

        #endregion       

        #region Add Survey

        public ActionResult AddSurvey()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.ThemDapAn.ToString()))
                return AccessDeniedView();

            var model = new SurveyDTO();          
            return View(model);
        }

        [HttpPost]
        public ActionResult AddSurvey(SurveyDTO model)
        {
            model.IsDeleted = false;
            var result = surveyRepository.AddNewSurvey(model);

            var survey = surveyRepository.GetSurvey(result);
            survey.Code = "TL_" + survey.SurveyID;
            surveyRepository.UpdateSurvey(survey);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Thêm mới câu trả lời không thành công!",
                    Title = "Thêm mới không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Thêm mới câu trả lời thành công!",
                Title = "Thêm mới thành công",
                URLList = "/OMS/Survey/ListSurvey",
                Id = result.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region Edit Survey

        public ActionResult EditSurvey(int id)
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.SuaDapAn.ToString()))
                return AccessDeniedView();

            var survey = surveyRepository.GetSurvey(id);
            if (survey == null)
                return RedirectToAction("ListSurvey");
            
            return View(survey);
        }

        [HttpPost]
        public ActionResult EditSurvey(SurveyDTO model)
        {
            var survey = surveyRepository.GetSurvey(model.SurveyID);
            if (survey == null)
                return RedirectToAction("ListSurvey");

            var result = surveyRepository.UpdateSurvey(model);
            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Cập nhật câu trả lời không thành công!",
                    Title = "Cập nhật không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Cập nhật câu trả lời thành công!",
                Title = "Cập nhật thành công",
                URLEdit = "/OMS/Survey/EditSurvey/" + model.SurveyID,
                Id = model.SurveyID.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion       

        #region Delete Survey

        [HttpPost]
        public ActionResult DeleteSurvey(int id)
        {
            var survey = surveyRepository.GetSurvey(id);
            if (survey == null)
                return RedirectToAction("ListSurvey");

            var result = surveyRepository.DeleteSurvey(id);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Xóa câu trả lời không thành công!",
                    Title = "Xóa không thành công"
                }, JsonRequestBehavior.DenyGet);

            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa câu trả lời thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/Survey/ListSurvey"
            }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    surveyRepository.DeleteSurvey(item);
                }
            }
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa câu trả lời thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/Survey/ListSurvey"
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion
        
    }
}
