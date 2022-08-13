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
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using MB.Common.Helpers;
using MB.Common.Kendoui;

namespace MB.OMS.Telesale.Controller
{
    public class StatusCallController : MBController
    {
        #region Feilds

        [Dependency]
        public IStatusCallRepository statusCallRepository { get; set; }
        [Dependency]
        public IProjectsRepository projectsRepository { get; set; }
        #endregion       

        #region List StatusCall

        //[PermissionAuthorize(Permissions.XemDanhSachTrangThaiCuocGoi)]
        public ActionResult ListStatusCall()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemDanhSachTrangThaiCuocGoi.ToString()))
                return AccessDeniedView();

            var status = new StatusCall();           
            return View(status);
        }

        [HttpPost]
        public JsonResult GetListStatusCall(DataSourceRequest dsRequest, MB.Common.Kendoui.Filter filter = null)
        {
            string name = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("Name")).Any())
                    name = ((string[])arrayFilter.Where(f => f.Field == "Name").FirstOrDefault().Value)[0];
            }

            var data = statusCallRepository.GetStatusCallDatasource(dsRequest: dsRequest, name: name);
            return data.ToJsonDataSource();
        }

        #endregion      

        #region Add StatusCall

        public ActionResult AddStatusCall()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.ThemTrangThaiCuocGoi.ToString()))
                return AccessDeniedView();

            var model = new StatusCallDTO();
            model.AvailableStatus.AddRange(statusCallRepository.GetAllStatus().Select(d => new SelectListItem()
            {
                Text = d.Name,
                Value = d.StatusID.ToString()
            }));
            
            return View(model);
        }

        [HttpPost]
        public ActionResult AddStatusCall(StatusCallDTO model)
        {
            var result = statusCallRepository.AddNewStatusCall(model);
            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Thêm mới trạng thái cuộc gọi không thành công!",
                    Title = "Thêm mới không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Thêm mới trạng thái cuộc gọi thành công!",
                Title = "Thêm mới thành công",
                URLList = "/OMS/StatusCall/ListStatusCall",
                Id = result.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion        

        #region Edit StatusCall

        public ActionResult EditStatusCall(int id)
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.SuaTrangThaiCuocGoi.ToString()))
                return AccessDeniedView();

            var status = statusCallRepository.GetStatusCall(id);
            if (status == null)
                return RedirectToAction("ListStatusCall");

            status.AvailableStatus.AddRange(statusCallRepository.GetAllStatus().Select(d => new SelectListItem()
            {
                Text = d.Name,
                Value = d.StatusID.ToString()
            }));
            
            return View(status);
        }

        [HttpPost]
        public ActionResult EditStatusCall(StatusCallDTO model)
        {
            var status = statusCallRepository.GetStatusCall(model.StatusCallID);
            if (status == null)
                return RedirectToAction("ListStatusCall");

            var result = statusCallRepository.UpdateStatusCall(model);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Cập nhật trạng thái cuộc gọi không thành công!",
                    Title = "Cập nhật không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Cập nhật trạng thái cuộc gọi thành công!",
                Title = "Cập nhật thành công",
                URLEdit = "/OMS/StatusCall/EditStatusCall/" + model.StatusID,
                Id = model.StatusID.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion      

        #region Delete StatusCall

        [HttpPost]
        public ActionResult DeleteStatusCall(int id)
        {
            var status = statusCallRepository.GetStatusCall(id);
            if (status == null)
                return RedirectToAction("ListStatusCall");

            var result = statusCallRepository.DeleteStatusCall(id);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Xóa trạng thái cuộc gọi không thành công!",
                    Title = "Xóa không thành công"
                }, JsonRequestBehavior.DenyGet);

            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa trạng thái cuộc gọi thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/StatusCall/ListStatusCall"
            }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    statusCallRepository.DeleteStatusCall(item);
                }
            }
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa trạng thái thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/StatusCall/ListStatusCall"
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion
       
    }
}
