using MB.Common;
using MB.Common.Kendoui;
using MB.Common.Helpers;
using MB.OMS.Telesale.Domain.Interface;
using MB.OMS.Telesale.Domain.Model;
using MB.Web.Core;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Controller
{
    public class SourcesController : MBController
    {
        #region Feild

        [Dependency]
        public ISourcesRepository sourcesRepository { get; set; }

        [Dependency]
        public ICallRepository callRepository { get; set; }

        [Dependency]
        public ICallLogRepository callLogRepository { get; set; }

        [Dependency]
        public IProjectCustomerRepository projectCustomerRepository { get; set; }
        [Dependency]
        public ICustomerRepository customerRepository { get; set; }
        [Dependency]
        public ICustomerErrorFieldValueRepository customerErrorFieldValueRepository { get; set; }
        [Dependency]
        public ICustomerErrorRepository customerErrorRepository { get; set; }
        [Dependency]
        public ICustomerExistRepository customerExistRepository { get; set; }
        [Dependency]
        public ICustomerExistFieldValueRepository customerExistFieldValueRepository { get; set; }
        [Dependency]
        public ICustomerFieldValueRepository customerFieldValueRepository { get; set; }
        [Dependency]
        public ISurveyAnswerRepository surveyAnswerRepository { get; set; }
        #endregion

        #region List Sources

        public ActionResult ListSources()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemDanhSachNguon.ToString()))
                return AccessDeniedView();

            var sources = new Sources();
            return View(sources);
        }

        [HttpPost]
        public JsonResult GetListSources(DataSourceRequest dsRequest, MB.Common.Kendoui.Filter filter = null)
        {
            string name = null;
            if (filter != null && filter.Filters != null)
            {
                var arrayFilter = filter.Filters.ToList();

                if (arrayFilter.Where(f => f.Field.Contains("Name")).Any())
                    name = ((string[])arrayFilter.Where(f => f.Field == "Name").FirstOrDefault().Value)[0];
            }
            var data = sourcesRepository.GetSourcesDatasource(dsRequest: dsRequest, name: name);
            return data.ToJsonDataSource();
        }

        #endregion       

        #region Add Sources

        public ActionResult AddSources()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.ThemNguon.ToString()))
                return AccessDeniedView();

            var model = new Sources();            
            return View(model);
        }

        [HttpPost]
        public ActionResult AddSources(Sources model)
        {
            var result = sourcesRepository.AddNewSources(model);
            if (result == 0)
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Thêm mới nguồn không thành công!",
                    Title = "Thêm mới không thành công"
                }, JsonRequestBehavior.DenyGet);
            return Json(new JsonResponse
            {
                Result = JsonMessage.Success,
                Message = "Thêm mới nguồn thành công!",
                Title = "Thêm mới thành công",
                URLList = "/OMS/Sources/ListSources",
                Id = result.ToString()
            }, JsonRequestBehavior.DenyGet);
        }

        #endregion

        #region Edit Sources

        public ActionResult EditSources(int id)
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.SuaNguon.ToString()))
                return AccessDeniedView();

            var source = sourcesRepository.GetSourcesById(id);
            if (source == null)
                return RedirectToAction("ListSources");

            return View(source);
        }

        [HttpPost]
        public ActionResult EditSources(Sources model)
        {
            var source = sourcesRepository.GetSourcesById(model.SourceID);
            if (source == null)
                return RedirectToAction("ListSources");

            var sourceName = sourcesRepository.GetSourcesByName(model.Name);
            if (sourceName != null && source.Name != model.Name)
            {
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Failed,
                    Message = "Tên nguồn đã được sử dụng",
                    Title = "Cập nhật không thành công"
                }, JsonRequestBehavior.DenyGet);
            }
            else
            {
                var result = sourcesRepository.UpdateSources(model);
                if (result == 0)
                    return Json(new JsonResponse
                    {
                        Result = JsonMessage.Failed,
                        Message = "Cập nhật nguồn không thành công!",
                        Title = "Cập nhật không thành công"
                    }, JsonRequestBehavior.DenyGet);
                return Json(new JsonResponse
                {
                    Result = JsonMessage.Success,
                    Message = "Cập nhật nguồn thành công!",
                    Title = "Cập nhật thành công",
                    URLEdit = "/OMS/Sources/EditSources/" + model.SourceID,
                    Id = model.SourceID.ToString()
                }, JsonRequestBehavior.DenyGet);
            }
        }

        #endregion       

        #region Excel

        public ActionResult DownloadExcel(int id)
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XuatNguon.ToString()))
                return AccessDeniedView();

            var source = sourcesRepository.GetSourcesById(id);
            return File(source.Link, "text/xls", Path.GetFileName(source.Link));
        }

        #endregion

        #region Delete Sources
        [HttpPost]
        public ActionResult DeleteSources(ICollection<int> selectedIds)
        {
            if (selectedIds != null)
            {
                foreach (var item in selectedIds)
                {
                    var Customer = customerRepository.GetAllCustomerBySourceID(SourceID: item).ToList();
                    foreach (var itemc in Customer)
                    {
                        //Xóa CallLogID trong bảng SurveyAnswer
                        var callogList = callLogRepository.GetCallLogByCustomerID(itemc.CustomerID).ToList();
                        if (callogList != null)
                        {
                            foreach (var tempCallLog in callogList)
                            {
                                var CallLogID = tempCallLog.CallLogID;
                                surveyAnswerRepository.DeleteSurveyAnswer(CallLogID);
                            }
                        }
                        //Xóa CustomerID trong bảng CallLog
                        callLogRepository.DeleteCallLogByCustomerNew(customerID:itemc.CustomerID);
                        //Xóa CustomerID trong bảng Call
                        callRepository.DeleteCallByCustomerNew(customerID: itemc.CustomerID);
                        //Xóa CustomerID trong bảng phân chia khách hàng cho dự án(ProjectCustomer)
                        projectCustomerRepository.DeleteProjectCustomerNew(customerID: itemc.CustomerID);
                        //Xóa CustomerErrorFieldValue
                        customerErrorFieldValueRepository.DeleteCustomerErrorFieldAllValue(CustomerErrorID: itemc.CustomerID);
                        //Xóa CustomerExistFieldValue
                        var customerExist = customerExistRepository.GetAll(customerID: itemc.CustomerID).ToList() ;
                        if (customerExist != null)
                        {
                            foreach(var it in customerExist)
                            {
                                var CustomerExistID = it.CustomerExistID;
                                customerExistFieldValueRepository.DeleteCustomerExistFieldAllValue(CustomerExistID);
                            }    
                        }
                        //Xóa trong bảng CustomerFieldValue
                        customerFieldValueRepository.DeleteCustomerFieldAllValue(itemc.CustomerID);

                        //Xoa trong bang Customer
                        customerRepository.DeleteCustomerNew(id: itemc.CustomerID);
                    }
                    //Xóa Customer trong bảng lỗi.
                    customerErrorRepository.DeleteCustomerErrorBySourceID(item);
                    //Xóa Customer trong bảng trùng.
                    customerExistRepository.DeleteCustomerExistBySourceID(item);
                    //Xóa nguồn
                    sourcesRepository.DeleteSourcesNew(item);
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
    }
}
