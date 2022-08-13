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
    public class CustomerFieldController : MBController
    {
        #region Feilds

        [Dependency]
        public ICustomerFieldRepository customerFieldRepository { get; set; }

        [Dependency]
        public IProjectCustomerFieldRepository projectCustomerFieldRepository { get; set; }

        [Dependency]
        public IProjectsRepository projectsRepository { get; set; }
        #endregion

        #region List CustomerField

        //[PermissionAuthorize(Permissions.XemDanhSachThuocTinhKH)]
        public ActionResult ListCustomerField()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemDanhSachThuocTinhKH.ToString()))
                return AccessDeniedView();

            var customerField = new CustomerField();
            return View(customerField);
        }

        [HttpPost]
        public JsonResult GetListCustomerField(DataSourceRequest dsRequest, MB.Common.Kendoui.Filter filter = null)
        {
            string fieldName = null;
            string fieldCode = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("FieldName")).Any())
                    fieldName = ((string[])arrayFilter.Where(f => f.Field == "FieldName").FirstOrDefault().Value)[0];

                if (arrayFilter.Where(f => f.Field.Contains("FieldCode")).Any())
                    fieldCode = ((string[])arrayFilter.Where(f => f.Field == "FieldCode").FirstOrDefault().Value)[0];
            }
            var data = customerFieldRepository.GetCustomerFieldDatasource(dsRequest: dsRequest, fieldcode: fieldCode, fieldname: fieldName);
            return data.ToJsonDataSource();
        }

        #endregion       

        #region Add CustomerField

        public ActionResult AddCustomerField()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.ThemThuocTinhKH.ToString()))
                return AccessDeniedView();

            var model = new CustomerFieldDTO();
            return View(model);
        }

        [HttpPost]
        public ActionResult AddCustomerField(CustomerFieldDTO model)
        {
            var result = customerFieldRepository.AddNewCustomerField(model);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Thêm mới thuộc tính khách hàng không thành công!",
                    Title = "Thêm mới không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Thêm mới thuộc tính khách hàng thành công!",
                Title = "Thêm mới thành công",
                URLList = "/OMS/CustomerField/ListCustomerField",
                Id = result.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion       

        #region Edit CustomerField

        public ActionResult EditCustomerField(int id)
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.SuaThuocTinhKH.ToString()))
                return AccessDeniedView();

            var customerField = customerFieldRepository.GetCustomerField(id);
            if (customerField == null)
                return RedirectToAction("ListCustomerField");
            return View(customerField);
        }

        [HttpPost]
        public ActionResult EditCustomerField(CustomerFieldDTO model)
        {
            var customerField = customerFieldRepository.GetCustomerField(model.CustomerFieldID);
            if (customerField == null)
                return RedirectToAction("ListCustomerField");

            var result = customerFieldRepository.UpdateCustomerField(model);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Cập nhật thuộc tính khách hàng không thành công!",
                    Title = "Cập nhật không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Cập nhật thộc tính khách hàng thành công!",
                Title = "Cập nhật thành công",
                URLEdit = "/OMS/CustomerField/EditCustomerField/" + model.CustomerFieldID,
                Id = model.CustomerFieldID.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region Delete CustomerField

        [HttpPost]
        public ActionResult DeleteCustomerField(int id)
        {
            var customerField = customerFieldRepository.GetCustomerField(id);
            if (customerField == null)
                return RedirectToAction("ListCustomerField");

            var result = customerFieldRepository.DeleteCustomerField(id);

            projectCustomerFieldRepository.DeleteProjectCustomerField(customerFieldID: id);

            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Xóa thuộc tính khách hàng không thành công!",
                    Title = "Xóa không thành công"
                }, JsonRequestBehavior.DenyGet);

            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa thuộc tính khách hàng thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/CustomerField/ListCustomerField"
            }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public ActionResult DeleteSelected(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    customerFieldRepository.DeleteCustomerField(item);
                }
            }
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Xóa thuộc tính thành công!",
                Title = "Xóa thành công",
                URLList = "/OMS/CustomerField/ListCustomerField"
            }, JsonRequestBehavior.DenyGet);
        }
        #endregion       
    }
}
