using MB.Common.Helpers;
using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Interface;
using MB.OMS.Telesale.Domain.Model;
using MB.Web.Core;
using Microsoft.Practices.Unity;
using OfficeOpenXml;
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
    public class EmployeeController : MBController
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("Tools.CheckEmployeeController");
        #region Fields
        [Dependency]
        public ISourcesRepository sourcesRepository { get; set; }
        #endregion


        #region List PG
        public ActionResult ListPGEmployee()
        {
            var employee = new PG_Employee();

            var arr = new List<string>();
            arr.Add("Mã cửa hàng");
            arr.Add("Tên cửa hàng");
            arr.Add("Tên nhân viên");
            arr.Add("Tên QLTT");
            arr.Add("Thứ 2");
            arr.Add("Thứ 3");
            arr.Add("Thứ 4");
            arr.Add("Thứ 5");
            arr.Add("Thứ 6");
            arr.Add("Thứ 7");
            arr.Add("CN");

            //Dành cho cột động
            //var listCustomerFeild = customerFieldRepository.GetCustomerFieldByProject(CommonHelper.CurrentProject());
            //arr.Add("[0].Value");
            //for (int i = 0; i < listCustomerFeild.Count(); i++)
            //{
            //    int kq = i + 7;
            //    arr.Add("[" + kq + "].Value");
            //}
            //arr.Add("[5].Value");

            ViewBag.ListCustomerField = arr;

            return View(employee);
        }

        [HttpPost]
        public JsonResult GetListPGEmployee(PG_Employee pg_employee)
        {

            var data = new DataSourceResult();
            //data = customerRepository.GetAllFieldValueDatasource(dsRequest: dsRequest, projectID: CommonHelper.CurrentProject(), sourceID: sourceID, dateFrom: DateFrom, dateEnd: EndDate);


            return data.ToJsonDataSource();

        }

        #endregion

        #region Import, Export Excel

        public ActionResult ImportExcel()
        {
            var data = new PG_Employee();
            return View(data);
        }

        [HttpPost]
        public ActionResult HandlingImportExcel(PG_Employee model)
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
                        properties.Add("Mã cửa hàng");
                        properties.Add("Tên cửa hàng");
                        properties.Add("Tên nhân viên");
                        properties.Add("Tên QLTT");
                        properties.Add("Thứ 2");
                        properties.Add("Thứ 3");
                        properties.Add("Thứ 4");
                        properties.Add("Thứ 5");
                        properties.Add("Thứ 6");
                        properties.Add("Thứ 7");
                        properties.Add("CN");


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


                        //UpLoad file excel
                        var path = CommonHelper.MapPath("~/Uploads/Excels/" + Path.GetFileName(hpf.FileName));
                        hpf.SaveAs(path);

                        //Insert Source
                        var source = new Sources();
                        source.Link = path;
                        source.Visiable = true;
                        source.Name = model.SourceName;
                        var sourceID = sourcesRepository.AddNewSources(source);


                        int iRow = 2;

                        while (true)
                        {
                            logger.Info("iRow:" + iRow);
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

                            #region  Insert Customer
                            // Insert Customer
                            //var customer = new Customer();
                            //customer.CustomerID = Guid.NewGuid();
                            //customer.SourceID = sourceID;
                            //customer.MobilePhone = mobilePhone;
                            //customer.Visiable = true;
                            //customer.IsDeleted = false;
                            //var customerID = customerRepository.AddNewCustomer(customer);

                            //Insert CustomerFieldValue
                            //foreach (var item in properties.Skip(1))
                            //{
                            //    string fieldValue = CommonHelper.ConvertColumnToString(worksheet.Cells[iRow, CommonHelper.GetColumnIndex(properties, item)].Value);
                            //    var customerField = customerFieldRepository.GetAll(fieldcode: item.ToString()).FirstOrDefault();
                            //    if (customerField != null)
                            //    {
                            //        var customerFieldValue = new CustomerFieldValue();
                            //        customerFieldValue.CustomerID = customerID;
                            //        customerFieldValue.CustomerFieldID = customerField.CustomerFieldID;
                            //        customerFieldValue.FieldValue = fieldValue;
                            //        customerFieldValueRepository.AddNewCustomerFieldValue(customerFieldValue);
                            //    }
                            //}
                            #endregion


                            //}

                            //next customer
                            iRow++;
                        }

                        //Gửi mail
                        //var currentUser = userRepository.GetUserById(new Guid(User.Identity.GetUserId()));
                        //string body = "<p><b>Tên nguồn: </b> " + model.SourceName + "</p>";
                        //body = "<p><b>Tên file import: </b> " + hpf.FileName + "</p>";
                        //body += "<p><b>Import thành công: </b> " + listSuccess + " khách hàng</p>";
                        //body += "<p><b>Import trùng: </b> " + listExist + " khách hàng</p>";
                        //body += "<p><b>Import lỗi: </b> " + listFail + " khách hàng</p>";
                        //body += "<p><b>Import hoàn thành lúc: </b> " + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt") + "</p>";
                        //body += "<p><b>Người thực hiện: </b> " + currentUser.UserName + "</p>";
                        //try
                        //{
                        //    logger.Info("Gửi mail import");
                        //    CommonHelper.SendEmail(subject: "Import data", body: body, toAddress: currentUser.Email, toName: currentUser.FullName, cc: new List<string>() { "khangvn@matbao.com" });

                        //}
                        //catch (Exception exm)
                        //{
                        //    logger.Fatal("Error: Gửi mail, Detail:" + exm.ToString());
                        //}
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

        #endregion

        #region Dashboard
        public ActionResult Dashboard()
        {
            return View();
        }
        #endregion
    }
}
