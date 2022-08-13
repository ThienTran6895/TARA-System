using MB.Common;
using MB.Common.Helpers;
using MB.Common.Kendoui;
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

namespace MB.OMS.Telesale.Controller
{
    public class CampaignController : MBController
    {
        #region Feild

        [Dependency]
        public ICampaignsRepository campaignsRepository { get; set; }

        #endregion        

        #region List Campaigns

        public ActionResult ListCampaigns()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemDanhSachChienDich.ToString()))
                return AccessDeniedView();

            var campaign = new Campaigns();
            return View(campaign);
        }

        [HttpPost]
        public JsonResult GetListCampaigns(DataSourceRequest dsRequest, MB.Common.Kendoui.Filter filter = null)
        {
            string name = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("Name")).Any())
                    name = arrayFilter.Where(f => f.Field == "Name").FirstOrDefault().Value.ToString();
            }

            var data = campaignsRepository.GetCampaignsDatasource(dsRequest: dsRequest, name: name, isDeleted: false);
            return data.ToJsonDataSource();
        }

        #endregion

        #region Add Campaigns

        public ActionResult AddCampaigns()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.ThemChienDich.ToString()))
                return AccessDeniedView();

            var model = new Campaigns();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddCampaigns(Campaigns model)
        {
            model.IsDeleted = false;
            var result = campaignsRepository.AddNewCampaigns(model);
            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Thêm mới chiến dịch không thành công!",
                    Title = "Thêm mới không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Thêm mới chiến dịch thành công!",
                Title = "Thêm mới thành công",
                URLList = "/OMS/Campaign/ListCampaigns",
                Id = result.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion       

        #region Edit Campaigns

        public ActionResult EditCampaigns(int id)
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.SuaChienDich.ToString()))
                return AccessDeniedView();

            var campaign = campaignsRepository.GetCampaigns(id);
            if (campaign == null)
                return RedirectToAction("ListCampaigns");

            return View(campaign);
        }

        [HttpPost]
        public ActionResult EditCampaigns(Campaigns model)
        {
            var campaign = campaignsRepository.GetCampaigns(model.CampaignID);
            if (campaign == null)
                return RedirectToAction("ListCampaigns");

            var result = campaignsRepository.UpdateCampaigns(model);
            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Cập nhật chiến dịch không thành công!",
                    Title = "Cập nhật không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Cập nhật chiến dịch thành công!",
                Title = "Cập nhật thành công",
                URLEdit = "/OMS/Campaign/EditCampaigns/" + model.CampaignID,
                Id = model.CampaignID.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion       

        #region Delete Campaigns

        [HttpPost]
        public ActionResult DeleteCampaigns(int id)
        {
            var campaign = campaignsRepository.GetCampaigns(id);
            if (campaign == null)
                return RedirectToAction("ListCampaigns");

            var result = campaignsRepository.DeleteCampaigns(id);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Xóa chiến dịch không thành công!",
                    Title = "Xóa không thành công"
                }, JsonRequestBehavior.DenyGet);

            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa chiến dịch thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/Campaign/ListCampaigns"
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion
        
    }
}
