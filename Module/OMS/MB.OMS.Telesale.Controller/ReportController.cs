using MB.Common;
using MB.Common.Helpers;
using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Interface;
using MB.OMS.Telesale.Domain.Model;
using MB.Web.Core;
using Microsoft.Practices.Unity;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Controller
{
    public class ReportController : MBController
    {
        log4net.ILog logger = log4net.LogManager.GetLogger("ReportController");

        #region Field
        [Dependency]
        public IReportRepository reportRepository { get; set; }

        [Dependency]
        public ICustomerFieldRepository customerFieldRepository { get; set; }
        [Dependency]
        public IQuestionRepository questionRepository { get; set; }
        [Dependency]
        public IQuestionsSurveyRepository questionsSurveyRepository { get; set; }
        [Dependency]
        public ISurveyAnswerRepository surveyAnswerRepository { get; set; }
        [Dependency]
        public ISurveyRepository surveyRepository { get; set; }
        [Dependency]
        public IProjectQuestionsRepository projectQuestionsRepository { get; set; }

        [Dependency]
        public ISourcesRepository sourcesRepository { get; set; }

        [Dependency]
        public IStatusCallRepository statuscallRepository { get; set; }
        #endregion

        #region LoadThongKe
        public ActionResult Overview()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemThongKeTheoDTV.ToString()))
                return AccessDeniedView();

            var model = new Report();
            return View(model);
        }
        [HttpPost]
        public JsonResult Overview(DataSourceRequest dsRequest, Report model)
        {
            model.ProjectID = CommonHelper.CurrentProject();
            var data = reportRepository.ThongKeTheoOverviewDatasource(dsRequest: dsRequest, model: model);
            return data.ToJsonDataSource();
        }
        public ActionResult GetListThongKeTheoNgay(string DateFrom, string EndDate)
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemThongKeTheoNgay.ToString()))
                return AccessDeniedView();

            var model = new Report();
            model.DateFrom = string.IsNullOrEmpty(DateFrom) == true ? null : DateFrom;
            model.DateEnd = string.IsNullOrEmpty(EndDate) == true ? null : EndDate;

            if (model.DateEnd == null)
                model.DateEnd = DateTime.Now.Date.AddDays(1).AddTicks(-1).ToShortDateString();
            else
                model.DateEnd = DateTime.Parse(model.DateEnd).Date.AddDays(1).AddTicks(-1).ToShortDateString();

            if (model.DateFrom == null)
                model.DateFrom = DateTime.Now.Date.AddDays(-60).ToShortDateString();
            else
                model.DateFrom = DateTime.Parse(model.DateFrom).Date.ToShortDateString();

            return View(model);
        }
        [HttpPost]
        public ActionResult GetListThongKeTheoNgay(DataSourceRequest dsRequest, Report model)
        {
            model.ProjectID = CommonHelper.CurrentProject();

            if (model.DateEnd == null)
                model.DateEnd = DateTime.Now.Date.AddDays(1).AddTicks(-1).ToString();
            else
                model.DateEnd = DateTime.Parse(model.DateEnd).Date.AddDays(1).AddTicks(-1).ToString();

            if (model.DateFrom == null)
                model.DateFrom = DateTime.Now.Date.AddDays(-60).ToString();
            else
                model.DateFrom = DateTime.Parse(model.DateFrom).Date.ToString();
            return reportRepository.ThongKeTheoNgayDatasource(dsRequest: dsRequest, model: model);

        }
        public ActionResult GetListThongKeTheoNguon()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemThongKeTheoNguon.ToString()))
                return AccessDeniedView();

            var model = new Report();
            return View(model);
        }
        [HttpPost]
        public JsonResult GetListThongKeTheoNguon(DataSourceRequest dsRequest, Report model)
        {
            model.ProjectID = CommonHelper.CurrentProject();
            var data = reportRepository.ThongKeTheoNguonDatasource(dsRequest: dsRequest, model: model);
            return data.ToJsonDataSource();
        }

        public ActionResult GetListThongKeTheoDTV()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemThongKeTheoDTV.ToString()))
                return AccessDeniedView();

            var model = new Report();
            return View(model);
        }
        [HttpPost]
        public JsonResult GetListThongKeTheoDTV(DataSourceRequest dsRequest, Report model)
        {
            model.ProjectID = CommonHelper.CurrentProject();
            var data = reportRepository.ThongKeTheoDTVDatasource(dsRequest: dsRequest, model: model);
            return data.ToJsonDataSource();
        }

        public ActionResult GetListThongKeNangSuat()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemThongKeNangSuat.ToString()))
                return AccessDeniedView();

            var model = new Report();
            return View(model);
        }
        [HttpPost]
        public JsonResult GetListThongKeNangSuat(DataSourceRequest dsRequest, Report model)
        {
            model.ProjectID = CommonHelper.CurrentProject();
            var data = reportRepository.ThongKeNangSuatDatasource(dsRequest: dsRequest, model: model);
            return data.ToJsonDataSource();
        }
        public ActionResult GetListThongKeBaoCaoChiTiet()
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemThongKeBaoCaoChiTiet.ToString()))
                return AccessDeniedView();

            var model = new Report();
            var curProjectID = MB.Common.Helpers.CommonHelper.CurrentProject();
            var listCustomerFeild = customerFieldRepository.GetCustomerFieldByProject(curProjectID);

            var arr1 = new List<string>();
            arr1.Add("[15].Value");
            arr1.Add("[16].Value");
            for (int i = 0; i < listCustomerFeild.Count(); i++)
            {
                int kq = i + 24;
                arr1.Add("[" + kq + "].Value");
            }
            arr1.Add("[17].Value");
            arr1.Add("[18].Value");
            arr1.Add("[19].Value");
            arr1.Add("[20].Value");
            arr1.Add("[21].Value");
            ViewBag.ListCustomerField1 = arr1;
            return View(model);
        }
        [HttpPost]
        public JsonResult GetListThongKeBaoCaoChiTiet(DataSourceRequest dsRequest, Report model)
        {
            var curProjectID = MB.Common.Helpers.CommonHelper.CurrentProject();
            model.ProjectID = curProjectID;
            model.StatusCallID = model.StatusCallID == 0 ? null : model.StatusCallID;
            var data = reportRepository.ThongKeBaoCaoChiTietDatasource(dsRequest: dsRequest, model: model);
            return data.ToJsonDataSource();
        }


        #endregion

        #region Export Excel

        public ActionResult ExportThongKeTheoNguon(string DateFrom, string EndDate)
        {
            var model = new Report();
            model.ProjectID = CommonHelper.CurrentProject();
            model.DateFrom = string.IsNullOrEmpty(DateFrom) == true ? null : DateFrom;
            model.DateEnd = string.IsNullOrEmpty(EndDate) == true ? null : EndDate;
            var report = reportRepository.ThongKeTheoNguon(model: model);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Thống kê theo nguồn");
                    var propertype = new[]
                    {
                        "STT",
                        "Tên nguồn",
                        "Tổng số",
                        "Đã phân công",
                        "Đã xử lý",
                        "Chưa xử lý",
                        "Đang hẹn gọi lại"
                    };

                    for (int i = 0; i < propertype.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = propertype[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Column(i + 1).Width = 20;
                        var border = worksheet.Cells[1, i + 1].Style.Border;
                        border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    int row = 2;
                    foreach (var rp in report)
                    {
                        int col = 1;

                        worksheet.Cells[row, col].Value = rp.STT;
                        col++;

                        worksheet.Cells[row, col].Value = rp.Name;
                        col++;

                        worksheet.Cells[row, col].Value = rp.TongSo;
                        col++;

                        worksheet.Cells[row, col].Value = rp.DaPhanCong;
                        col++;

                        worksheet.Cells[row, col].Value = rp.DaXuLy;
                        col++;

                        worksheet.Cells[row, col].Value = rp.ChuaXuLy;
                        col++;

                        worksheet.Cells[row, col].Value = rp.DangHenGoiLai;
                        col++;

                        row++;
                    }

                    xlPackage.Save();
                }
                bytes = stream.ToArray();
            }
            return File(bytes, "text/xls", "Thống kê theo nguồn.xlsx");
        }
        public ActionResult ExportThongKeTheoDTV(string DateFrom, string EndDate)
        {
            var model = new Report();
            model.ProjectID = CommonHelper.CurrentProject();
            model.DateFrom = string.IsNullOrEmpty(DateFrom) == true ? null : DateFrom;
            model.DateEnd = string.IsNullOrEmpty(EndDate) == true ? null : EndDate;
            var report = reportRepository.ThongKeTheoDTV(model);
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Thống kê theo điện thoại viên");
                    var propertype = new[]
                    {
                        "STT",
                        "Tên ĐTV",
                        "Tên đăng nhập",
                        "Đã nhận",
                        "Đã xử lý",
                        "Chưa gọi",
                        "Đang hẹn gọi lại"
                    };

                    for (int i = 0; i < propertype.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = propertype[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Column(i + 1).Width = 20;
                        var border = worksheet.Cells[1, i + 1].Style.Border;
                        border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    int row = 2;
                    foreach (var rp in report)
                    {
                        int col = 1;

                        worksheet.Cells[row, col].Value = rp.STT;
                        col++;

                        worksheet.Cells[row, col].Value = rp.HoTen;
                        col++;

                        worksheet.Cells[row, col].Value = rp.UserName;
                        col++;

                        worksheet.Cells[row, col].Value = rp.DaNhan;
                        col++;

                        worksheet.Cells[row, col].Value = rp.DaXuLy;
                        col++;

                        worksheet.Cells[row, col].Value = rp.ChuaGoi;
                        col++;

                        worksheet.Cells[row, col].Value = rp.DangHenGoiLai;
                        col++;

                        row++;
                    }

                    xlPackage.Save();
                }
                bytes = stream.ToArray();
            }
            return File(bytes, "text/xls", "Thống kê theo điện thoại viên.xlsx");
        }

        public ActionResult ExportThongKeNangSuat(string DateFrom, string EndDate)
        {
            var model = new Report();
            model.ProjectID = CommonHelper.CurrentProject();
            model.DateFrom = string.IsNullOrEmpty(DateFrom) == true ? null : DateFrom;
            model.DateEnd = string.IsNullOrEmpty(EndDate) == true ? null : EndDate;
            var report = reportRepository.ThongKeNangSuat(model);

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Thống kê năng suất");
                    var propertype = new[]
                    {
                        "STT",
                        "Tên ĐTV",
                        "Tổng số khách hàng đã gọi",
                        "Khách hàng thành công",
                        "Khách hàng tiềm năng",
                        "Khách hàng từ chối",
                        "Khách hàng sai số",
                        "Khách hàng hẹn gọi lại"
                    };

                    for (int i = 0; i < propertype.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = propertype[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Column(i + 1).Width = 20;
                        var border = worksheet.Cells[1, i + 1].Style.Border;
                        border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    int row = 2;
                    foreach (var rp in report)
                    {
                        int col = 1;

                        worksheet.Cells[row, col].Value = rp.STT;
                        col++;

                        worksheet.Cells[row, col].Value = rp.FullName;
                        col++;

                        worksheet.Cells[row, col].Value = rp.TongSoKHDaGoi;
                        col++;

                        worksheet.Cells[row, col].Value = rp.KHThanhCong;
                        col++;

                        worksheet.Cells[row, col].Value = rp.KHTiemNang;
                        col++;

                        worksheet.Cells[row, col].Value = rp.KHTuChoi;
                        col++;

                        worksheet.Cells[row, col].Value = rp.KHSaiSo;
                        col++;

                        worksheet.Cells[row, col].Value = rp.KHHenGoiLai;
                        col++;

                        row++;
                    }

                    xlPackage.Save();
                }
                bytes = stream.ToArray();
            }
            return File(bytes, "text/xls", "Thống kê năng suất.xlsx");
        }

        public ActionResult ExportThongKeChiTiet(string DateFrom, string EndDate)
        {

            logger.InfoFormat("BEGIN at {0}", DateTime.Now.ToString("HH:mm:ss"));
            var curProjectID = MB.Common.Helpers.CommonHelper.CurrentProject();
            var model = new Report();
            model.DateFrom = string.IsNullOrEmpty(DateFrom) == true ? null : DateFrom;
            model.DateEnd = string.IsNullOrEmpty(EndDate) == true ? null : EndDate;
            model.StatusCallID = null;
            model.ProjectID = curProjectID;

            var report = reportRepository.ThongKeBaoCaoChiTiet(model: model);
            var CustomerField = customerFieldRepository.GetCustomerFieldByProject(curProjectID);
            //logger.InfoFormat("Excute store GetCustomerFieldByProject (TotalRows={1}, Time={0})", sw.Elapsed.TotalSeconds, CustomerField.Count());
            logger.InfoFormat("Excute store GetCustomerFieldByProject(rows={0}), ThongKeBaoCaoChiTiet(rows={1}))", CustomerField.Count(), report.Count());


            string tmpLog = "";
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    if (stream == null)
                        throw new ArgumentNullException("stream");

                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        #region [1] get Thông tin câu hỏi và câu trả lời trong project(worksheet1)
                        var QuestionInProject = projectQuestionsRepository.GetAllByProjectID(projectID: curProjectID);
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Thống kê chi tiết");
                        var worksheet1 = xlPackage.Workbook.Worksheets.Add("Thông tin câu hỏi và câu trả lời");
                        worksheet1.Cells[1, 1].Value = "Danh sách mã câu hỏi và câu hỏi";
                        worksheet1.Cells[1, 1, 1, 2].Merge = true;
                        worksheet1.Cells[1, 1, 1, 2].Style.Font.Bold = true;
                        worksheet1.Cells[1, 1, 1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var borders1 = worksheet.Cells[1, 1, 1, 2].Style.Border;
                        borders1.Bottom.Style = borders1.Right.Style = borders1.Left.Style = borders1.Top.Style = ExcelBorderStyle.Thin;
                        int row1 = 4;
                        foreach (var itemq in QuestionInProject)
                        {
                            int col1 = 1;
                            var question = questionRepository.GetQuestionById(itemq.QuestionID);
                            worksheet1.Cells[row1, col1].Value = question.Code;
                            col1++;
                            worksheet1.Cells[row1, col1].Value = MB.Common.Helpers.CommonHelper.Decode_RemoveHTML(question.Name);
                            row1++;
                            int questionID = itemq.QuestionID;
                            var SurverForQuestion = surveyRepository.GetSurveyByQuestionId(questionID: questionID, projectID: curProjectID);
                            foreach (var items in SurverForQuestion)
                            {
                                int col2 = 2;
                                worksheet1.Cells[row1, col2].Value = items.Code + " - " + MB.Common.Helpers.CommonHelper.Decode_RemoveHTML(items.SurveyContent);
                                row1++;
                            }
                        }
                        #endregion

                        #region Tiêu đề file excel(worksheet)
                        var propertype = new List<string>();
                        propertype.Add("STT");
                        propertype.Add("SĐT cá nhân");
                        foreach (var item in CustomerField)
                        {
                            propertype.Add(item.FieldName);
                        }

                        //Thong tin cuoc goi
                        propertype.Add("Ngày gọi");
                        propertype.Add("Tình Trạng cuộc gọi");
                        propertype.Add("Agent");
                        propertype.Add("Ghi chú cuộc gọi");
                        propertype.Add("Ngày gọi");
                        propertype.Add("Tình Trạng cuộc gọi");
                        propertype.Add("Agent");
                        propertype.Add("Ngày gọi");
                        propertype.Add("Tình Trạng cuộc gọi");
                        propertype.Add("Agent");
                        propertype.Add("Ngày gọi");
                        propertype.Add("Tình Trạng cuộc gọi");
                        propertype.Add("Agent");

                        propertype.Add("Số lần gọi");

                        worksheet.Cells[1, 1].Value = "Danh sách nhân viên";
                        worksheet.Cells[1, 1, 2, CustomerField.Count() + 2].Merge = true;
                        worksheet.Cells[1, 1, 2, CustomerField.Count() + 2].Style.Font.Bold = true;
                        worksheet.Cells[1, 1, 2, CustomerField.Count() + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var border = worksheet.Cells[1, 1, 2, CustomerField.Count() + 2].Style.Border;
                        border.Bottom.Style = border.Right.Style = ExcelBorderStyle.Thin;

                        for (int i = 0; i < propertype.Count; i++)
                        {
                            worksheet.Cells[3, i + 1].Value = propertype[i];
                            worksheet.Cells[3, i + 1].Style.Font.Bold = true;
                            worksheet.Column(3 + 1).Width = 20;
                            var border1 = worksheet.Cells[3, i + 1].Style.Border;
                            border1.Bottom.Style = border1.Top.Style = border1.Left.Style = border1.Right.Style = ExcelBorderStyle.Thin;
                        }

                        worksheet.Cells[1, CustomerField.Count() + 3].Value = "Thông tin cuộc gọi";
                        worksheet.Cells[1, CustomerField.Count() + 3, 2, CustomerField.Count() + 6].Merge = true;
                        worksheet.Cells[1, CustomerField.Count() + 3, 2, CustomerField.Count() + 6].Style.Font.Bold = true;
                        worksheet.Cells[1, CustomerField.Count() + 3, 2, CustomerField.Count() + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var border2 = worksheet.Cells[1, CustomerField.Count() + 3, 2, CustomerField.Count() + 6].Style.Border;
                        border2.Bottom.Style = border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells[1, CustomerField.Count() + 7].Value = "Gọi lần 1";
                        worksheet.Cells[1, CustomerField.Count() + 7, 2, CustomerField.Count() + 9].Merge = true;
                        worksheet.Cells[1, CustomerField.Count() + 7, 2, CustomerField.Count() + 9].Style.Font.Bold = true;
                        worksheet.Cells[1, CustomerField.Count() + 7, 2, CustomerField.Count() + 9].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var border3 = worksheet.Cells[1, CustomerField.Count() + 7, 2, CustomerField.Count() + 9].Style.Border;
                        border3.Bottom.Style = border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells[1, CustomerField.Count() + 10].Value = "Gọi lần 2";
                        worksheet.Cells[1, CustomerField.Count() + 10, 2, CustomerField.Count() + 12].Merge = true;
                        worksheet.Cells[1, CustomerField.Count() + 10, 2, CustomerField.Count() + 12].Style.Font.Bold = true;
                        worksheet.Cells[1, CustomerField.Count() + 10, 2, CustomerField.Count() + 12].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var border4 = worksheet.Cells[1, CustomerField.Count() + 10, 2, CustomerField.Count() + 12].Style.Border;
                        border4.Bottom.Style = border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells[1, CustomerField.Count() + 13].Value = "Gọi lần 3";
                        worksheet.Cells[1, CustomerField.Count() + 13, 2, CustomerField.Count() + 15].Merge = true;
                        worksheet.Cells[1, CustomerField.Count() + 13, 2, CustomerField.Count() + 15].Style.Font.Bold = true;
                        worksheet.Cells[1, CustomerField.Count() + 13, 2, CustomerField.Count() + 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var border5 = worksheet.Cells[1, CustomerField.Count() + 13, 2, CustomerField.Count() + 15].Style.Border;
                        border5.Bottom.Style = border.Right.Style = ExcelBorderStyle.Thin;

                        worksheet.Cells[1, CustomerField.Count() + 16].Value = "Số lần gọi";
                        worksheet.Cells[1, CustomerField.Count() + 16, 3, CustomerField.Count() + 16].Merge = true;
                        worksheet.Cells[1, CustomerField.Count() + 16, 3, CustomerField.Count() + 16].Style.Font.Bold = true;
                        worksheet.Cells[1, CustomerField.Count() + 16, 3, CustomerField.Count() + 16].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var border6 = worksheet.Cells[1, CustomerField.Count() + 16, 3, CustomerField.Count() + 16].Style.Border;
                        border6.Bottom.Style = border.Right.Style = ExcelBorderStyle.Thin;



                        int Numstart = 17;
                        int NumEnd = 0;
                        int colSurvey = 17;
                        foreach (var itemq in QuestionInProject)
                        {
                            int questionID = itemq.QuestionID;
                            var SurverForQuestion = surveyRepository.GetSurveyByQuestionId(questionID: questionID, projectID: curProjectID);
                            int NumColSur = SurverForQuestion.Count() > 0 ? SurverForQuestion.Count() : 1;
                            // Load title cau hoi
                            NumEnd += Numstart + NumColSur - 1;
                            worksheet.Cells[2, CustomerField.Count() + Numstart].Value = questionRepository.GetQuestionById(questionID).Code;
                            worksheet.Cells[2, CustomerField.Count() + Numstart, 2, CustomerField.Count() + NumEnd].Merge = true;
                            worksheet.Cells[2, CustomerField.Count() + Numstart, 2, CustomerField.Count() + NumEnd].Style.Font.Bold = true;
                            worksheet.Cells[2, CustomerField.Count() + Numstart, 2, CustomerField.Count() + NumEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            var border7 = worksheet.Cells[2, CustomerField.Count() + Numstart, 2, CustomerField.Count() + NumEnd].Style.Border;
                            border7.Bottom.Style = border7.Right.Style = border7.Left.Style = border7.Top.Style = ExcelBorderStyle.Thin;
                            Numstart = NumEnd + 1;
                            NumEnd = 0;
                            if (SurverForQuestion.Count() > 0)
                            {
                                // Load title cau tra loi
                                foreach (var items in SurverForQuestion)
                                {
                                    worksheet.Cells[3, CustomerField.Count() + colSurvey].Value = items.Code;
                                    worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Merge = true;
                                    worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Style.Font.Bold = true;
                                    worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    var border9 = worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Style.Border;
                                    border9.Bottom.Style = border9.Right.Style = border9.Left.Style = border9.Top.Style = ExcelBorderStyle.Thin;
                                    colSurvey++;
                                }
                            }
                            else
                            {

                                worksheet.Cells[3, CustomerField.Count() + colSurvey].Value = "Chưa có cấu trả lời";
                                worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Merge = true;
                                worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Style.Font.Bold = true;
                                worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                var border9 = worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Style.Border;
                                border9.Bottom.Style = border9.Right.Style = border9.Left.Style = border9.Top.Style = ExcelBorderStyle.Thin;
                                colSurvey++;

                            }

                        }

                        worksheet.Cells[1, CustomerField.Count() + 17].Value = "Câu hỏi";
                        worksheet.Cells[1, CustomerField.Count() + 17, 1, CustomerField.Count() + (Numstart - 1)].Merge = true;
                        worksheet.Cells[1, CustomerField.Count() + 17, 1, CustomerField.Count() + (Numstart - 1)].Style.Font.Bold = true;
                        worksheet.Cells[1, CustomerField.Count() + 17, 1, CustomerField.Count() + (Numstart - 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var border8 = worksheet.Cells[1, CustomerField.Count() + 16, 1, CustomerField.Count() + (Numstart - 1)].Style.Border;
                        border8.Bottom.Style = border8.Right.Style = border8.Left.Style = border8.Top.Style = ExcelBorderStyle.Thin;
                        #endregion
                        logger.InfoFormat("End prepare template -TotalSecond={0}", DateTime.Now.ToString("HH:mm:ss"));
                        int row = 4;
                        logger.Info("Excute GetSurveyByQuestionIdTK");
                        var allSurveyForQuestion = surveyRepository.GetSurveyByQuestionIdTK(projectID: curProjectID);
                        logger.Info("Excute LoadAllSurveyAnswerTK");
                        var allSurveyAnswers = reportRepository.LoadAllSurveyAnswerTK(curProjectID, DateFrom, EndDate);
                        logger.Info("Load nội dung thống kê có trong store");
                        foreach (IDictionary<string, object> rp in report)
                        {
                            tmpLog += "r:" + row;
                            #region Load nội dung thống kê có trong store
                            int col = 1;
                            int CallID = Convert.ToInt32(rp.FirstOrDefault(x => x.Key == "CallID").Value);
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "STT").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "MobilePhone").Value;
                            col++;
                            tmpLog += "col:" + col;
                            foreach (var item in CustomerField)
                            {
                                worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == item.FieldCode).Value;
                                col++;
                                tmpLog += "col:" + col;
                            }
                            worksheet.Cells[row, col].Style.Numberformat.Format = "dd/mm/yyyy hh:mm AM/PM";
                            worksheet.Cells[row, col].AutoFitColumns();
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "NgayGoiSauCung").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TinhTrangCuocGoiSauCung").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "NguoiGoiSauCung").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "GhiChuCuoiCung").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Style.Numberformat.Format = "dd/mm/yyyy hh:mm AM/PM";
                            worksheet.Cells[row, col].AutoFitColumns();
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "NgayGoiLan1").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TinhTrangCuocGoiLan1").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "NguoiGoiLan1").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Style.Numberformat.Format = "dd/mm/yyyy hh:mm AM/PM";
                            worksheet.Cells[row, col].AutoFitColumns();
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "NgayGoiLan2").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TinhTrangCuocGoiLan2").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "NguoiGoiLan2").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Style.Numberformat.Format = "dd/mm/yyyy hh:mm: AM/PM";
                            worksheet.Cells[row, col].AutoFitColumns();
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "NgayGoiLan3").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TinhTrangCuocGoiLan3").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "NguoiGoiLan3").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TongSoLanGoi").Value;
                            col++;
                            tmpLog += "col:" + col;
                            #endregion

                            #region load nội dung câu hỏi theo callID cuối cùng của khách hàng

                            foreach (var itemq in QuestionInProject)
                            {
                                int questionID = itemq.QuestionID;
                                #region CODE CŨ
                                //Lấy tất cả các câu trả lời theo mã câu hỏi
                                //var SurverForQuestion = surveyRepository.GetSurveyByQuestionIdTest(questionID: questionID, projectID: curProjectID);
                                //int NumColSur = SurverForQuestion.Count();
                                //foreach (var items in SurverForQuestion)
                                //{
                                //    int SurveyID = items.SurveyID;
                                //    //Load nội dung câu trả lời theo mã câu hỏi, mã câu trả lời
                                //    var result = reportRepository.LoadAllSurveyAnswerTest(curProjectID, CallID, questionID, SurveyID, model.DateFrom, model.DateEnd).FirstOrDefault();
                                //    if (result != null)
                                //    {
                                //        worksheet.Cells[row, col].Value = result.SurveyContent;
                                //    }
                                //    col++;

                                //}
                                #endregion

                                #region CODE MỚI
                                var surveyQuestions = allSurveyForQuestion.Where<SurveyDTO>(s => s.QuestionID == questionID);
                                if (surveyQuestions != null && allSurveyAnswers != null)
                                {
                                    foreach (var it in surveyQuestions)
                                    {

                                        var result = allSurveyAnswers.Where<Report>(sa => sa.QuestionID == questionID && sa.SurveyID == it.SurveyID && sa.CallID == CallID).FirstOrDefault();
                                        if (result != null)
                                        {
                                            worksheet.Cells[row, col].Value = result.SurveyContent;
                                        }
                                        col++;
                                    }
                                }
                                #endregion
                            }

                            #endregion
                            row++;
                        }
                        xlPackage.Save();
                    }
                    bytes = stream.ToArray();
                }
                logger.InfoFormat("END at {0}", DateTime.Now.ToString("HH:mm:ss"));
                return File(bytes, "text/xls", "Thống kê chi tiết.xlsx");
            }
            catch (Exception ex)
            {
                logger.Info(tmpLog);
                logger.Fatal(ex.ToString());
                return File("", "text/xls", "Thống kê chi tiết.xlsx");
            }

        }
        public ActionResult ExportThongKeChiTietSimple(string DateFrom, string EndDate, string IsSuccess)
        {

            logger.InfoFormat("BEGIN at {0}", DateTime.Now.ToString("HH:mm:ss"));
            var curProjectID = MB.Common.Helpers.CommonHelper.CurrentProject();
            bool isExportSuccess = (IsSuccess == "true");
            var model = new Report();
            model.DateFrom = string.IsNullOrEmpty(DateFrom) == true ? null : DateFrom;
            model.DateEnd = string.IsNullOrEmpty(EndDate) == true ? null : EndDate;
            model.StatusCallID = null;
            model.ProjectID = curProjectID;

            var report = reportRepository.ThongKeBaoCaoChiTietSimple(model: model);
            var CustomerField = customerFieldRepository.GetCustomerFieldByProject(curProjectID);
            //logger.InfoFormat("Excute store GetCustomerFieldByProject (TotalRows={1}, Time={0})", sw.Elapsed.TotalSeconds, CustomerField.Count());
            logger.InfoFormat("Excute store GetCustomerFieldByProject(rows={0}), ThongKeBaoCaoChiTietSimple(rows={1}))", CustomerField.Count(), report.Count());



            string tmpLog = "";
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    if (stream == null)
                        throw new ArgumentNullException("stream");

                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        #region [1] get Thông tin câu hỏi và câu trả lời trong project(worksheet1)
                        var QuestionInProject = projectQuestionsRepository.GetAllByProjectID(projectID: curProjectID);
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Thống kê chi tiết");
                        var worksheet1 = xlPackage.Workbook.Worksheets.Add("Thông tin câu hỏi và câu trả lời");
                        worksheet1.Cells[1, 1].Value = "Danh sách mã câu hỏi và câu hỏi";
                        worksheet1.Cells[1, 1, 1, 2].Merge = true;
                        worksheet1.Cells[1, 1, 1, 2].Style.Font.Bold = true;
                        worksheet1.Cells[1, 1, 1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var borders1 = worksheet.Cells[1, 1, 1, 2].Style.Border;
                        borders1.Bottom.Style = borders1.Right.Style = borders1.Left.Style = borders1.Top.Style = ExcelBorderStyle.Thin;
                        int row1 = 4;
                        foreach (var itemq in QuestionInProject)
                        {
                            int col1 = 1;
                            var question = questionRepository.GetQuestionById(itemq.QuestionID);
                            worksheet1.Cells[row1, col1].Value = question.Code;
                            col1++;
                            worksheet1.Cells[row1, col1].Value = MB.Common.Helpers.CommonHelper.Decode_RemoveHTML(question.Name);
                            row1++;
                            int questionID = itemq.QuestionID;
                            var SurverForQuestion = surveyRepository.GetSurveyByQuestionId(questionID: questionID, projectID: curProjectID);
                            foreach (var items in SurverForQuestion)
                            {
                                int col2 = 2;
                                worksheet1.Cells[row1, col2].Value = items.Code + " - " + MB.Common.Helpers.CommonHelper.Decode_RemoveHTML(items.SurveyContent);
                                row1++;
                            }
                        }
                        #endregion

                        #region Tiêu đề file excel(worksheet)
                        var propertype = new List<string>();
                        propertype.Add("STT");
                        propertype.Add("SĐT cá nhân");
                        foreach (var item in CustomerField)
                        {
                            propertype.Add(item.FieldName);
                        }

                        //Thong tin cuoc goi
                        propertype.Add("Ngày gọi");
                        propertype.Add("Tình Trạng cuộc gọi");
                        propertype.Add("Agent");
                        propertype.Add("Ghi chú cuộc gọi");


                        propertype.Add("Số lần gọi");

                        worksheet.Cells[1, 1].Value = "Danh sách nhân viên";
                        worksheet.Cells[1, 1, 2, CustomerField.Count() + 2].Merge = true;
                        worksheet.Cells[1, 1, 2, CustomerField.Count() + 2].Style.Font.Bold = true;
                        worksheet.Cells[1, 1, 2, CustomerField.Count() + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var border = worksheet.Cells[1, 1, 2, CustomerField.Count() + 2].Style.Border;
                        border.Bottom.Style = border.Right.Style = ExcelBorderStyle.Thin;

                        for (int i = 0; i < propertype.Count; i++)
                        {
                            worksheet.Cells[3, i + 1].Value = propertype[i];
                            worksheet.Cells[3, i + 1].Style.Font.Bold = true;
                            worksheet.Column(3 + 1).Width = 20;
                            var border1 = worksheet.Cells[3, i + 1].Style.Border;
                            border1.Bottom.Style = border1.Top.Style = border1.Left.Style = border1.Right.Style = ExcelBorderStyle.Thin;
                        }

                        worksheet.Cells[1, CustomerField.Count() + 3].Value = "Thông tin cuộc gọi";
                        worksheet.Cells[1, CustomerField.Count() + 3, 2, CustomerField.Count() + 6].Merge = true;
                        worksheet.Cells[1, CustomerField.Count() + 3, 2, CustomerField.Count() + 6].Style.Font.Bold = true;
                        worksheet.Cells[1, CustomerField.Count() + 3, 2, CustomerField.Count() + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var border2 = worksheet.Cells[1, CustomerField.Count() + 3, 2, CustomerField.Count() + 6].Style.Border;
                        border2.Bottom.Style = border.Right.Style = ExcelBorderStyle.Thin;


                        worksheet.Cells[1, CustomerField.Count() + 7].Value = "Số lần gọi";
                        worksheet.Cells[1, CustomerField.Count() + 7, 3, CustomerField.Count() + 7].Merge = true;
                        worksheet.Cells[1, CustomerField.Count() + 7, 3, CustomerField.Count() + 7].Style.Font.Bold = true;
                        worksheet.Cells[1, CustomerField.Count() + 7, 3, CustomerField.Count() + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var border6 = worksheet.Cells[1, CustomerField.Count() + 7, 3, CustomerField.Count() + 7].Style.Border;
                        border6.Bottom.Style = border.Right.Style = ExcelBorderStyle.Thin;



                        int Numstart = 8;
                        int NumEnd = 0;
                        int colSurvey = 8;
                        foreach (var itemq in QuestionInProject)
                        {
                            int questionID = itemq.QuestionID;
                            var SurverForQuestion = surveyRepository.GetSurveyByQuestionId(questionID: questionID, projectID: curProjectID);
                            int NumColSur = SurverForQuestion.Count() > 0 ? SurverForQuestion.Count() : 1;
                            // Load title cau hoi
                            NumEnd += Numstart + NumColSur - 1;
                            worksheet.Cells[2, CustomerField.Count() + Numstart].Value = questionRepository.GetQuestionById(questionID).Code;
                            worksheet.Cells[2, CustomerField.Count() + Numstart, 2, CustomerField.Count() + NumEnd].Merge = true;
                            worksheet.Cells[2, CustomerField.Count() + Numstart, 2, CustomerField.Count() + NumEnd].Style.Font.Bold = true;
                            worksheet.Cells[2, CustomerField.Count() + Numstart, 2, CustomerField.Count() + NumEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            var border7 = worksheet.Cells[2, CustomerField.Count() + Numstart, 2, CustomerField.Count() + NumEnd].Style.Border;
                            border7.Bottom.Style = border7.Right.Style = border7.Left.Style = border7.Top.Style = ExcelBorderStyle.Thin;
                            Numstart = NumEnd + 1;
                            NumEnd = 0;
                            if (SurverForQuestion.Count() > 0)
                            {
                                // Load title cau tra loi
                                foreach (var items in SurverForQuestion)
                                {
                                    worksheet.Cells[3, CustomerField.Count() + colSurvey].Value = items.Code;
                                    worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Merge = true;
                                    worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Style.Font.Bold = true;
                                    worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    var border9 = worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Style.Border;
                                    border9.Bottom.Style = border9.Right.Style = border9.Left.Style = border9.Top.Style = ExcelBorderStyle.Thin;
                                    colSurvey++;
                                }
                            }
                            else
                            {

                                worksheet.Cells[3, CustomerField.Count() + colSurvey].Value = "Chưa có cấu trả lời";
                                worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Merge = true;
                                worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Style.Font.Bold = true;
                                worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                var border9 = worksheet.Cells[3, CustomerField.Count() + colSurvey, 3, CustomerField.Count() + colSurvey].Style.Border;
                                border9.Bottom.Style = border9.Right.Style = border9.Left.Style = border9.Top.Style = ExcelBorderStyle.Thin;
                                colSurvey++;

                            }

                        }

                        worksheet.Cells[1, CustomerField.Count() + 8].Value = "Câu hỏi";
                        worksheet.Cells[1, CustomerField.Count() + 8, 1, CustomerField.Count() + (Numstart - 1)].Merge = true;
                        worksheet.Cells[1, CustomerField.Count() + 8, 1, CustomerField.Count() + (Numstart - 1)].Style.Font.Bold = true;
                        worksheet.Cells[1, CustomerField.Count() + 8, 1, CustomerField.Count() + (Numstart - 1)].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var border8 = worksheet.Cells[1, CustomerField.Count() + 7, 1, CustomerField.Count() + (Numstart - 1)].Style.Border;
                        border8.Bottom.Style = border8.Right.Style = border8.Left.Style = border8.Top.Style = ExcelBorderStyle.Thin;
                        #endregion
                        logger.InfoFormat("End prepare template -TotalSecond={0}", DateTime.Now.ToString("HH:mm:ss"));
                        int row = 4;
                        logger.Info("Excute GetSurveyByQuestionIdTK");
                        var allSurveyForQuestion = surveyRepository.GetSurveyByQuestionIdTK(projectID: curProjectID);
                        logger.Info("Excute LoadAllSurveyAnswerTK");
                        var allSurveyAnswers = reportRepository.LoadAllSurveyAnswerTK(curProjectID, DateFrom, EndDate);
                        logger.Info("Load nội dung thống kê có trong store");
                        foreach (IDictionary<string, object> rp in report)
                        {
                            // Only done call
                            if (isExportSuccess
                                && (rp.FirstOrDefault(x => x.Key == "TinhTrangCuocGoiSauCung").Value.ToString() == "KH thành công"
                                || rp.FirstOrDefault(x => x.Key == "TinhTrangCuocGoiSauCung").Value.ToString() == "KH từ chối"
                                || rp.FirstOrDefault(x => x.Key == "TinhTrangCuocGoiSauCung").Value.ToString() == "Sai số"
                                || rp.FirstOrDefault(x => x.Key == "TongSoLanGoi").Value.ToInt() >= 6))
                                continue;


                            tmpLog += "r:" + row;
                            #region Load nội dung thống kê có trong store
                            int col = 1;
                            int CallID = Convert.ToInt32(rp.FirstOrDefault(x => x.Key == "CallID").Value);
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "STT").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "MobilePhone").Value;
                            col++;
                            tmpLog += "col:" + col;
                            foreach (var item in CustomerField)
                            {
                                worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == item.FieldCode).Value;
                                col++;
                                tmpLog += "col:" + col;
                            }
                            worksheet.Cells[row, col].Style.Numberformat.Format = "dd/mm/yyyy hh:mm AM/PM";
                            worksheet.Cells[row, col].AutoFitColumns();
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "NgayGoiSauCung").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TinhTrangCuocGoiSauCung").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "NguoiGoiSauCung").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "GhiChuCuoiCung").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TongSoLanGoi").Value;
                            col++;
                            tmpLog += "col:" + col;
                            #endregion

                            #region load nội dung câu hỏi theo callID cuối cùng của khách hàng

                            foreach (var itemq in QuestionInProject)
                            {
                                int questionID = itemq.QuestionID;
                                #region CODE CŨ
                                //Lấy tất cả các câu trả lời theo mã câu hỏi
                                //var SurverForQuestion = surveyRepository.GetSurveyByQuestionIdTest(questionID: questionID, projectID: curProjectID);
                                //int NumColSur = SurverForQuestion.Count();
                                //foreach (var items in SurverForQuestion)
                                //{
                                //    int SurveyID = items.SurveyID;
                                //    //Load nội dung câu trả lời theo mã câu hỏi, mã câu trả lời
                                //    var result = reportRepository.LoadAllSurveyAnswerTest(curProjectID, CallID, questionID, SurveyID, model.DateFrom, model.DateEnd).FirstOrDefault();
                                //    if (result != null)
                                //    {
                                //        worksheet.Cells[row, col].Value = result.SurveyContent;
                                //    }
                                //    col++;

                                //}
                                #endregion

                                #region CODE MỚI
                                var surveyQuestions = allSurveyForQuestion.Where<SurveyDTO>(s => s.QuestionID == questionID);
                                if (surveyQuestions != null && allSurveyAnswers != null)
                                {
                                    foreach (var it in surveyQuestions)
                                    {

                                        var result = allSurveyAnswers.Where<Report>(sa => sa.QuestionID == questionID && sa.SurveyID == it.SurveyID && sa.CallID == CallID).FirstOrDefault();
                                        if (result != null)
                                        {
                                            worksheet.Cells[row, col].Value = result.SurveyContent;
                                        }
                                        col++;
                                    }
                                }
                                #endregion
                            }

                            #endregion
                            row++;
                        }
                        xlPackage.Save();
                    }
                    bytes = stream.ToArray();
                }
                logger.InfoFormat("END at {0}", DateTime.Now.ToString("HH:mm:ss"));
                return File(bytes, "text/xls", "Thống kê chi tiết.xlsx");
            }
            catch (Exception ex)
            {
                logger.Info(tmpLog);
                logger.Fatal(ex.ToString());
                return File("", "text/xls", "Thống kê giản lược.xlsx");
            }

        }
        public ActionResult ExportThongKeChiTietFull(string DateFrom, string EndDate, string IsSuccess)
        {

            logger.InfoFormat("BEGIN at {0}", DateTime.Now.ToString("HH:mm:ss"));
            var curProjectID = MB.Common.Helpers.CommonHelper.CurrentProject();
            bool isExportSuccess = (IsSuccess == "true");
            var model = new Report();
            model.DateFrom = string.IsNullOrEmpty(DateFrom) == true ? null : DateFrom;
            model.DateEnd = string.IsNullOrEmpty(EndDate) == true ? null : EndDate;
            model.StatusCallID = null;
            model.ProjectID = curProjectID;

            var report = reportRepository.ThongKeBaoCaoChiTietFull(model: model);
            var CustomerField = customerFieldRepository.GetCustomerFieldByProject(curProjectID);
            //logger.InfoFormat("Excute store GetCustomerFieldByProject (TotalRows={1}, Time={0})", sw.Elapsed.TotalSeconds, CustomerField.Count());
            logger.InfoFormat("Excute store GetCustomerFieldByProject(rows={0}), ThongKeBaoCaoChiTietFull(rows={1}))", CustomerField.Count(), report.Count());


            string tmpLog = "";
            try
            {
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    if (stream == null)
                        throw new ArgumentNullException("stream");

                    using (var xlPackage = new ExcelPackage(stream))
                    {
                        #region [1] get Thông tin câu hỏi và câu trả lời trong project(worksheet1)
                        var QuestionInProject = projectQuestionsRepository.GetAllByProjectID(projectID: curProjectID);
                        var worksheet = xlPackage.Workbook.Worksheets.Add("Thống kê chi tiết");
                        var worksheet1 = xlPackage.Workbook.Worksheets.Add("Thông tin câu hỏi và câu trả lời");
                        worksheet1.Cells[1, 1].Value = "Danh sách mã câu hỏi và câu hỏi";
                        worksheet1.Cells[1, 1, 1, 2].Merge = true;
                        worksheet1.Cells[1, 1, 1, 2].Style.Font.Bold = true;
                        worksheet1.Cells[1, 1, 1, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        var borders1 = worksheet.Cells[1, 1, 1, 2].Style.Border;
                        borders1.Bottom.Style = borders1.Right.Style = borders1.Left.Style = borders1.Top.Style = ExcelBorderStyle.Thin;
                        int row1 = 4;
                        foreach (var itemq in QuestionInProject)
                        {
                            int col1 = 1;
                            var question = questionRepository.GetQuestionById(itemq.QuestionID);
                            worksheet1.Cells[row1, col1].Value = question.Code;
                            col1++;
                            worksheet1.Cells[row1, col1].Value = MB.Common.Helpers.CommonHelper.Decode_RemoveHTML(question.Name);
                            row1++;
                            int questionID = itemq.QuestionID;
                            var SurverForQuestion = surveyRepository.GetSurveyByQuestionId(questionID: questionID, projectID: curProjectID);
                            foreach (var items in SurverForQuestion)
                            {
                                int col2 = 2;
                                worksheet1.Cells[row1, col2].Value = items.Code + " - " + MB.Common.Helpers.CommonHelper.Decode_RemoveHTML(items.SurveyContent);
                                row1++;
                            }
                        }
                        #endregion

                        #region Tiêu đề file excel(worksheet)
                        var propertype = new List<string>();
                        propertype.Add("No.");
                        propertype.Add("Customer's Phone");
                        propertype.Add("SourceName");

                        propertype.Add("Last day contact");
                        propertype.Add("Last status");
                        propertype.Add("Number times of call");
                        propertype.Add("Noted");
                        propertype.Add("User Agent");

                        //Thong tin cuoc goi
                        for (int i = 0; i < propertype.Count; i++)
                        {
                            worksheet.Cells[2, i + 1].Value = propertype[i];
                            worksheet.Cells[2, i + 1].Style.Font.Bold = true;
                            worksheet.Column(2 + 1).Width = 20;
                            var border1 = worksheet.Cells[2, i + 1].Style.Border;
                            border1.Bottom.Style = border1.Top.Style = border1.Left.Style = border1.Right.Style = ExcelBorderStyle.Thin;
                        }

                        int Numstart = 8;
                        int NumEnd = 0;
                        int colSurvey = 8;
                        foreach (var itemq in QuestionInProject)
                        {
                            int questionID = itemq.QuestionID;
                            var SurverForQuestion = surveyRepository.GetSurveyByQuestionId(questionID: questionID, projectID: curProjectID);
                            int NumColSur = SurverForQuestion.Count() > 0 ? SurverForQuestion.Count() : 1;
                            // Load title cau hoi
                            NumEnd += Numstart + NumColSur - 1;
                            worksheet.Cells[1,  Numstart].Value = questionRepository.GetQuestionById(questionID).Code;
                            worksheet.Cells[1,  Numstart, 1,  NumEnd].Merge = true;
                            worksheet.Cells[1,  Numstart, 1,  NumEnd].Style.Font.Bold = true;
                            worksheet.Cells[1,  Numstart, 1,  NumEnd].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            var border7 = worksheet.Cells[1,  Numstart, 1,  NumEnd].Style.Border;
                            border7.Bottom.Style = border7.Right.Style = border7.Left.Style = border7.Top.Style = ExcelBorderStyle.Thin;
                            Numstart = NumEnd + 1;
                            NumEnd = 0;
                            if (SurverForQuestion.Count() > 0)
                            {
                                // Load title cau tra loi
                                foreach (var items in SurverForQuestion)
                                {
                                    worksheet.Cells[2,  colSurvey].Value = items.Code;
                                    worksheet.Cells[2,  colSurvey, 2,  colSurvey].Merge = true;
                                    worksheet.Cells[2,  colSurvey, 2,  colSurvey].Style.Font.Bold = true;
                                    worksheet.Cells[2,  colSurvey, 2,  colSurvey].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    var border9 = worksheet.Cells[2,  colSurvey, 2,  colSurvey].Style.Border;
                                    border9.Bottom.Style = border9.Right.Style = border9.Left.Style = border9.Top.Style = ExcelBorderStyle.Thin;
                                    colSurvey++;
                                }
                            }
                            else
                            {

                                worksheet.Cells[2,  colSurvey].Value = "Chưa có cấu trả lời";
                                worksheet.Cells[2,  colSurvey, 2,  colSurvey].Merge = true;
                                worksheet.Cells[2,  colSurvey, 2,  colSurvey].Style.Font.Bold = true;
                                worksheet.Cells[2,  colSurvey, 2,  colSurvey].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                var border9 = worksheet.Cells[3,  colSurvey, 3,  colSurvey].Style.Border;
                                border9.Bottom.Style = border9.Right.Style = border9.Left.Style = border9.Top.Style = ExcelBorderStyle.Thin;
                                colSurvey++;

                            }

                        }
                        #endregion
                        logger.InfoFormat("End prepare template -TotalSecond={0}", DateTime.Now.ToString("HH:mm:ss"));
                        int row = 3;
                        logger.Info("Excute GetSurveyByQuestionIdTK");
                        var allSurveyForQuestion = surveyRepository.GetSurveyByQuestionIdTK(projectID: curProjectID);
                        logger.Info("Excute LoadAllSurveyAnswerTK");
                        var allSurveyAnswers = reportRepository.LoadAllSurveyAnswerTK(curProjectID, DateFrom, EndDate);
                        logger.Info("Load nội dung thống kê có trong store");

                        propertype = new List<string>();
                        string[] excludes = { "Họ tên khách hàng", "Mã sản phẩm", "Sản phẩm" };
                        foreach (var item in CustomerField)
                        {
                            propertype.Add(item.FieldName);
                        }

                        //Thong tin cuoc goi
                        for (int i = 0; i < propertype.Count; i++)
                        {
                            worksheet.Cells[2, i + Numstart].Value = propertype[i];
                            worksheet.Cells[2, i + Numstart].Style.Font.Bold = true;
                            worksheet.Column(2 + Numstart).Width = 20;
                            var border1 = worksheet.Cells[3, i + Numstart].Style.Border;
                            border1.Bottom.Style = border1.Top.Style = border1.Left.Style = border1.Right.Style = ExcelBorderStyle.Thin;
                        }

                        // Call logs
                        Numstart = Numstart + propertype.Count + 3;
                        for (int i = 1; i <= 6; i++)
                        {
                            worksheet.Cells[1, Numstart].Value = string.Format("Gọi lần {0}", i);
                            worksheet.Cells[1, Numstart, 1, Numstart + 2].Merge = true;
                            worksheet.Cells[1, Numstart, 1, Numstart + 2].Style.Font.Bold = true;
                            worksheet.Cells[1, Numstart, 1, Numstart + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            var border = worksheet.Cells[1, 1, Numstart, Numstart + 2].Style.Border;
                            border.Bottom.Style = border.Right.Style = ExcelBorderStyle.Thin;

                            worksheet.Cells[2, Numstart].Value = "Ngày gọi";
                            worksheet.Cells[2, Numstart + 1].Value = "Tình trạng";
                            worksheet.Cells[2, Numstart + 2].Value = "Agent";

                            worksheet.Cells[2, Numstart, 2, Numstart + 2].Style.Font.Bold = true;
                            worksheet.Column(2 + 1).Width = 20;
                            var border1 = worksheet.Cells[2, Numstart + 2].Style.Border;
                            border1.Bottom.Style = border1.Top.Style = border1.Left.Style = border1.Right.Style = ExcelBorderStyle.Thin;

                            Numstart += 3;
                        }
                        
                        foreach (IDictionary<string, object> rp in report)
                        {
                            // Only done call
                            if (isExportSuccess
                                && (rp.FirstOrDefault(x => x.Key == "TinhTrangCuocGoiSauCung").Value.ToString() == "KH thành công"
                                || rp.FirstOrDefault(x => x.Key == "TinhTrangCuocGoiSauCung").Value.ToString() == "KH từ chối"
                                || rp.FirstOrDefault(x => x.Key == "TinhTrangCuocGoiSauCung").Value.ToString() == "Sai số"
                                || rp.FirstOrDefault(x => x.Key == "TongSoLanGoi").Value.ToInt() >= 6))
                                continue;


                            tmpLog += "r:" + row;
                            #region Load nội dung thống kê có trong store
                            int col = 1;
                            int CallID = Convert.ToInt32(rp.FirstOrDefault(x => x.Key == "CallID").Value);
                            worksheet.Cells[row, col].Value = row - 2;
                            col++;
                            tmpLog += "col:" + col;

                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "MobilePhone").Value;
                            col++;
                            tmpLog += "col:" + col;
                            //sondt 2019
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "SourceName").Value;
                            col++;
                            tmpLog += "col:" + col;

                            worksheet.Cells[row, col].Style.Numberformat.Format = "dd/mm/yyyy hh:mm AM/PM";
                            worksheet.Cells[row, col].AutoFitColumns();
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "NgayGoiSauCung").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TinhTrangCuocGoiSauCung").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TongSoLanGoi").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "GhiChuCuoiCung").Value;
                            col++;
                            tmpLog += "col:" + col;
                            worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "NguoiGoiSauCung").Value;
                            col++;
                            tmpLog += "col:" + col;

                            // Thông tin đơn hàng	
                            var questionCodes = new List<int>();
                            foreach (var itemq in QuestionInProject)
                            {
                                questionCodes.Add(itemq.QuestionID);
                            }
                            var answerCodes = new List<string>() { "TL_5238", "TL_5236","TL_5240","TL_5235", "TL_5237" };
                            var surveyQuestions = allSurveyForQuestion.Where<SurveyDTO>(s => answerCodes.Contains(s.Code) && questionCodes.Contains(s.QuestionID));

                                if (surveyQuestions != null && allSurveyAnswers != null)
                                {
                                    foreach (var it in surveyQuestions)
                                    {

                                        var result = allSurveyAnswers.Where<Report>(sa => sa.SurveyID == it.SurveyID && sa.CallID == CallID && it.Code == "TL_5240").FirstOrDefault();
                                        if (result != null)
                                        {
                                            worksheet.Cells[row, col].Value = result.SurveyContent.Length > 0 ? "x" : "";
                                        }

                                        result = allSurveyAnswers.Where<Report>(sa => sa.SurveyID == it.SurveyID && sa.CallID == CallID && it.Code == "TL_5238").FirstOrDefault();
                                        if (result != null)
                                        {
                                            worksheet.Cells[row, col+1].Value = result.SurveyContent;
                                        }


                                        result = allSurveyAnswers.Where<Report>(sa => sa.SurveyID == it.SurveyID && sa.CallID == CallID && it.Code == "TL_5236").FirstOrDefault();
                                        if (result != null)
                                        {
                                            worksheet.Cells[row, col+2].Value = result.SurveyContent;
                                        }


                                        result = allSurveyAnswers.Where<Report>(sa => sa.SurveyID == it.SurveyID && sa.CallID == CallID && it.Code == "TL_5240").FirstOrDefault();
                                        if (result != null)
                                        {
                                            worksheet.Cells[row, col+3].Value = result.SurveyContent;
                                        }


                                        result = allSurveyAnswers.Where<Report>(sa => sa.SurveyID == it.SurveyID && sa.CallID == CallID && it.Code == "TL_5235").FirstOrDefault();
                                        if (result != null)
                                        {
                                            worksheet.Cells[row, col+4].Value = result.SurveyContent;
                                        }


                                        result = allSurveyAnswers.Where<Report>(sa => sa.SurveyID == it.SurveyID && sa.CallID == CallID && it.Code == "TL_5237").FirstOrDefault();
                                        if (result != null)
                                        {
                                            worksheet.Cells[row, col+5].Value = result.SurveyContent;
                                        }
                                        
                                    }
                                    //col += 6;
                            }
                            #endregion



                            #region load nội dung câu hỏi theo callID cuối cùng của khách hàng
                            foreach (var itemq in QuestionInProject)
                            {
                                int questionID = itemq.QuestionID;
                                #region CODE CŨ
                                //Lấy tất cả các câu trả lời theo mã câu hỏi
                                //var SurverForQuestion = surveyRepository.GetSurveyByQuestionIdTest(questionID: questionID, projectID: curProjectID);
                                //int NumColSur = SurverForQuestion.Count();
                                //foreach (var items in SurverForQuestion)
                                //{
                                //    int SurveyID = items.SurveyID;
                                //    //Load nội dung câu trả lời theo mã câu hỏi, mã câu trả lời
                                //    var result = reportRepository.LoadAllSurveyAnswerTest(curProjectID, CallID, questionID, SurveyID, model.DateFrom, model.DateEnd).FirstOrDefault();
                                //    if (result != null)
                                //    {
                                //        worksheet.Cells[row, col].Value = result.SurveyContent;
                                //    }
                                //    col++;

                                //}
                                #endregion

                                #region CODE MỚI
                                surveyQuestions = allSurveyForQuestion.Where<SurveyDTO>(s => s.QuestionID == questionID);
                                if (surveyQuestions != null && allSurveyAnswers != null)
                                {
                                    foreach (var it in surveyQuestions)
                                    {

                                        var result = allSurveyAnswers.Where<Report>(sa => sa.QuestionID == questionID && sa.SurveyID == it.SurveyID && sa.CallID == CallID).FirstOrDefault();
                                        if (result != null)
                                        {
                                            worksheet.Cells[row, col].Value = result.SurveyContent;
                                        }
                                        col++;
                                    }
                                }
                                #endregion
                            }

                            #endregion
                            foreach (var item in CustomerField)
                            {
                                    worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == item.FieldCode).Value;
                                    col++;
                                    tmpLog += "col:" + col;
                            }

                            #region Call logs
                            for (int i = 1; i <= 6; i++)
                            {
                                var dt = rp.FirstOrDefault(x => x.Key == string.Format("CD{0}", i)).Value;
                                if (null != dt)
                                    worksheet.Cells[row, col].Value = dt.ToDateTime().ToString("dd/MM/yyyy HH:mm");
                                else
                                    worksheet.Cells[row, col].Value = "";
                                col++;
                                tmpLog += "col:" + col;

                                worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == string.Format("CStatus{0}", i)).Value;
                                col++;
                                tmpLog += "col:" + col;

                                worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == string.Format("CUser{0}", i)).Value;
                                col++;
                                tmpLog += "col:" + col;
                            }
                            #endregion


                            row++;
                        }
                        xlPackage.Save();
                    }
                    bytes = stream.ToArray();
                }
                logger.InfoFormat("END at {0}", DateTime.Now.ToString("HH:mm:ss"));
                return File(bytes, "text/xls", "Thống kê chi tiết.xlsx");
            }
            catch (Exception ex)
            {
                logger.Info(tmpLog);
                logger.Fatal(ex.ToString());
                return File("", "text/xls", "Thống kê chi tiết.xlsx");
            }

        }

        #endregion

        #region Return Customer for Source
        public ActionResult ReturnCustomerForSourceID(Guid id)
        {
            if (id != Guid.Empty)
            {
                reportRepository.ReturnCustomerForSource(id);
            }
            return RedirectToAction("GetListThongKeTheoDTV");
        }
        #endregion


        #region report for Trusting Social
        public ActionResult OverviewData(int sourceId=0, string ds = "", string de = "")
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemThongKeBaoCaoChiTiet.ToString()))
                return AccessDeniedView();
            var reportView = new TsReportModel();
            if (!string.IsNullOrEmpty(ds) & !string.IsNullOrEmpty(de))
            {
                reportView.DateStart = Converter.ParseDatetime(ds, "MM/dd/yyyy", false);
                reportView.DateEnd = Converter.ParseDatetime(de, "MM/dd/yyyy", false);
            }

            reportView.AvaiableSources.AddRange(sourcesRepository.GetAllSourcesByProject(CommonHelper.CurrentProject()).Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.SourceID.ToString(),
                Selected = u.SourceID == sourceId
            }));
            reportView.ReportData = reportRepository.OverviewDataByStatus(sourceId, dateFrom: reportView.DateStart, dateEnd: reportView.DateEnd.AddHours(23));
            return View(reportView);
        }

        public FileResult ExportOverviewData(int sourceId,string ds, string de)
        {
            DateTime dateStart = DateTime.Now.AddDays(-7);
            DateTime dateEnd = DateTime.Now.AddHours(23);

            if (!string.IsNullOrEmpty(ds) & !string.IsNullOrEmpty(de))
            {
                dateStart = Converter.ParseDatetime(ds, "MM/dd/yyyy", false);
                dateEnd = Converter.ParseDatetime(de, "MM/dd/yyyy", false);
            }

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                using (var xlPackage = new ExcelPackage(stream))
                {
                    var report = reportRepository.OverviewDataByStatus(sourceId,dateFrom: dateStart, dateEnd: dateEnd.AddHours(23)).ToList();
                    #region Tiêu đề file excel(worksheet)
                    var worksheet = xlPackage.Workbook.Worksheets.Add("HieuQuaSuDung_Data");
                    worksheet.Cells[1, 1].Value = "STT";
                    worksheet.Cells[1, 2].Value = "Nội dung";
                    worksheet.Cells[1, 3].Value = "Số lượng";
                    worksheet.Cells[1, 4].Value = "Tỷ lệ";
                    #endregion

                    int row = 2;
                    int stt = 1;
                    double dataChuaTiepCan = (double)report[1].Number;
                    double dataDaTiepCan = (double)report[2].Number;

                    foreach (var rp in report)
                    {
                        worksheet.Cells[row, 1].Value = stt;
                        worksheet.Cells[row, 2].Value = rp.RowTitle;
                        worksheet.Cells[row, 3].Value = rp.Number;
                        worksheet.Cells[row, 4].Value = 0;
                        //if (row == 2)
                        //{
                        //    worksheet.Cells[row, 4].Value = rp.TongSoKHDaGoi; // ratio
                        //}
                        //if (row == 2)
                        //{
                        //    worksheet.Cells[row, 4].Value = rp.TongSoKHDaGoi; // ratio
                        //}

                        row++;
                        stt++;
                    }

                    xlPackage.Save();
                }
                bytes = stream.ToArray();
            }

            return File(bytes, "text/xls", "HieuQuaSuDung_Data_"+DateTime.Now.ToString("yyyyMMddHHmm")+".xlsx");
        }

        public ActionResult AnalyticRejected(int sourceId=0,string ds="", string de="")
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemThongKeBaoCaoChiTiet.ToString()))
                return AccessDeniedView();

            var reportView = new TsReportModel();
            if (!string.IsNullOrEmpty(ds) & !string.IsNullOrEmpty(de))
            {
                reportView.DateStart = Converter.ParseDatetime(ds,"MM/dd/yyyy",false);
                reportView.DateEnd = Converter.ParseDatetime(de, "MM/dd/yyyy", false);
            }

            reportView.AvaiableSources.AddRange(sourcesRepository.GetAllSourcesByProject(CommonHelper.CurrentProject()).Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.SourceID.ToString(),
                Selected = u.SourceID == sourceId
            }));

            reportView.ReportData = reportRepository.GetReportStatusCallId(sourceId: sourceId, statusId: 2, dateFrom: reportView.DateStart, dateEnd: reportView.DateEnd.AddHours(23));
            return View(reportView);
        }
        public FileResult ExportAnalyticRejected(int sourceId, string ds = "", string de = "")
        {
            DateTime dateStart = DateTime.Now.AddDays(-7);
            DateTime dateEnd = DateTime.Now.AddHours(23);

            if (!string.IsNullOrEmpty(ds) & !string.IsNullOrEmpty(de))
            {
                dateStart = Converter.ParseDatetime(ds, "MM/dd/yyyy", false);
                dateEnd = Converter.ParseDatetime(de, "MM/dd/yyyy", false);
            }



            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                using (var xlPackage = new ExcelPackage(stream))
                {
                    var report = reportRepository.GetReportStatusCallId(sourceId: sourceId, statusId: 2, dateFrom: dateStart, dateEnd: dateEnd.AddHours(23)).ToList();

                    #region Tiêu đề file excel(worksheet)
                    var worksheet = xlPackage.Workbook.Worksheets.Add("PhanTichTuChoiThamGia");
                    worksheet.Cells[1, 1].Value = "STT";
                    worksheet.Cells[1, 2].Value = "Nội dung";
                    worksheet.Cells[1, 3].Value = "Số lượng";
                    worksheet.Cells[1, 4].Value = "Tỷ lệ";
                    worksheet.Cells[1, 5].Value = "Tỷ lệ/Data tiếp cận";
                    #endregion

                    int row = 2;
                    int stt = 1;

                    double total = report.Sum(d => d.Number);
                    double totalDataOfSource = report.First().TotalDataOfSource;

                    double totalPercent = 0;
                    double totalPercenOverSource = 0;



                    foreach (var rp in report)
                    {
                        double ratio = 0;
                        if (total != 0) Math.Round((double)(100 * rp.Number) / total);
                        double ratioOverData = 0;
                        if (totalDataOfSource != 0) Math.Round((double)(100 * rp.Number) / totalDataOfSource);

                        totalPercent += ratio;
                        totalPercenOverSource += ratioOverData;

                        worksheet.Cells[row, 1].Value = stt;
                        worksheet.Cells[row, 2].Value = rp.RowTitle;
                        worksheet.Cells[row, 3].Value = rp.Number;
                        worksheet.Cells[row, 4].Value = ratio + " %";
                        worksheet.Cells[row, 5].Value = ratioOverData + " %";


                        row++;
                        stt++;
                    }
                    worksheet.Cells[row, 1].Value = "";
                    worksheet.Cells[row, 2].Value = "Tổng cộng";
                    worksheet.Cells[row, 3].Value = total;
                    worksheet.Cells[row, 4].Value = totalPercent + " %";
                    worksheet.Cells[row, 5].Value = totalPercenOverSource + " %";

                    xlPackage.Save();
                }
                bytes = stream.ToArray();
            }

            return File(bytes, "text/xls", "PhanTichTuChoiThamGia_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx");
        }

        public ActionResult ReportCustomerFail(int sourceId=0, string ds = "", string de = "")
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemThongKeBaoCaoChiTiet.ToString()))
                return AccessDeniedView();

            var reportView = new TsReportModel();
            if (!string.IsNullOrEmpty(ds) & !string.IsNullOrEmpty(de))
            {
                reportView.DateStart = Converter.ParseDatetime(ds, "MM/dd/yyyy", false);
                reportView.DateEnd = Converter.ParseDatetime(de, "MM/dd/yyyy", false);
            }

            reportView.AvaiableSources.AddRange(sourcesRepository.GetAllSourcesByProject(CommonHelper.CurrentProject()).Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.SourceID.ToString(),
                Selected = u.SourceID == sourceId
            }));
            reportView.ReportData = reportRepository.GetReportStatusCallId(sourceId: sourceId, statusId: 3, dateFrom: reportView.DateStart, dateEnd: reportView.DateEnd.AddHours(23));
            return View(reportView);
        }

        public FileResult ExportReportCustomerFail(int sourceId, string ds = "", string de = "")
        {
            DateTime dateStart = DateTime.Now.AddDays(-7);
            DateTime dateEnd = DateTime.Now.AddHours(23);

            if (!string.IsNullOrEmpty(ds) & !string.IsNullOrEmpty(de))
            {
                dateStart = Converter.ParseDatetime(ds, "MM/dd/yyyy", false);
                dateEnd = Converter.ParseDatetime(de, "MM/dd/yyyy", false);
            }

            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                using (var xlPackage = new ExcelPackage(stream))
                {
                    var report = reportRepository.GetReportStatusCallId(sourceId: sourceId, statusId: 3, dateFrom: dateStart, dateEnd: dateEnd.AddHours(23)).ToList();

                    #region Tiêu đề file excel(worksheet)
                    var worksheet = xlPackage.Workbook.Worksheets.Add("PhanTichKhachHangKhongDuDieuKien");
                    worksheet.Cells[1, 1].Value = "STT";
                    worksheet.Cells[1, 2].Value = "Nội dung";
                    worksheet.Cells[1, 3].Value = "Số lượng";
                    worksheet.Cells[1, 4].Value = "Tỷ lệ";
                    worksheet.Cells[1, 5].Value = "Tỷ lệ/Data tiếp cận";
                    #endregion

                    int row = 2;
                    int stt = 1;
                    double dataChuaTiepCan = (double)report[1].Number;
                    double dataDaTiepCan = (double)report[2].Number;


                    double total =report.Sum(d => d.Number);
                    double totalDataOfSource = report[0].TotalDataOfSource;

                    double totalPercent = 0;
                    double totalPercenOverSource = 0;


                    foreach (var rp in report)
                    {
                        double ratio = 0;
                        if(total !=0) Math.Round((double)(100 * rp.Number) / total);
                        double ratioOverData = 0;
                        if(totalDataOfSource!=0) Math.Round((double)(100 * rp.Number) / totalDataOfSource);

                        totalPercent += ratio;
                        totalPercenOverSource += ratioOverData;

                        worksheet.Cells[row, 1].Value = stt;
                        worksheet.Cells[row, 2].Value = rp.RowTitle;
                        worksheet.Cells[row, 3].Value = rp.Number;
                        worksheet.Cells[row, 4].Value = ratio + " %";
                        worksheet.Cells[row, 5].Value = ratioOverData + " %";


                        row++;
                        stt++;
                    }
                    worksheet.Cells[row, 1].Value = "";
                    worksheet.Cells[row, 2].Value = "Tổng cộng";
                    worksheet.Cells[row, 3].Value = total;
                    worksheet.Cells[row, 4].Value = totalPercent + " %";
                    worksheet.Cells[row, 5].Value = totalPercenOverSource + " %";

                    xlPackage.Save();
                }
                bytes = stream.ToArray();
            }

            return File(bytes, "text/xls", "PhanTichKhachHangKhongDuDieuKien_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx");
        }


        public ActionResult TotalCallByDaily(int sourceId=0, string ds = "", string de = "")
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemThongKeBaoCaoChiTiet.ToString()))
                return AccessDeniedView();

            var reportView = new ReportTotalCallByDaily();
            if (!string.IsNullOrEmpty(ds) & !string.IsNullOrEmpty(de))
            {
                reportView.DateStart = Converter.ParseDatetime(ds, "MM/dd/yyyy", false);
                reportView.DateEnd = Converter.ParseDatetime(de, "MM/dd/yyyy", false);
            }

            reportView.AvaiableSources.AddRange(sourcesRepository.GetAllSourcesByProject(CommonHelper.CurrentProject()).Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.SourceID.ToString(),
                Selected = u.SourceID == sourceId
            }));

            return View(reportView);
        }

        [HttpPost]
        public JsonResult RenderTotalCallByDaily(int sourceId = 0, string ds = "", string de="")
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemThongKeBaoCaoChiTiet.ToString()))
                return Json("Bạn không được phép xem trang báo cáo này.");

            DateTime dateStart = DateTime.Now.AddDays(-7);
            DateTime dateEnd = DateTime.Now.AddHours(23);

            if (!string.IsNullOrEmpty(ds) & !string.IsNullOrEmpty(de))
            {
                dateStart = Converter.ParseDatetime(ds, "MM/dd/yyyy", false);
                dateEnd = Converter.ParseDatetime(de, "MM/dd/yyyy", false);
            }

            #region build header
            StringBuilder sbHeader = new StringBuilder();
            StringBuilder sbSubHeader = new StringBuilder();

            sbHeader.Append("<tr>");
            sbHeader.AppendFormat("<th {0}>{1}</th>", "rowspan=\"2\" class=\"ce-cntr\"", "Nội dung");
            sbHeader.AppendFormat("<th {0}>{1}</th>", "colspan=\"2\" class=\"ce-cntr\"", "Tổng");

            sbSubHeader.Append("<th class=\"ce-cntr\">SL</th><th class=\"ce-cntr\">Tỷ lệ</th>");

            DateTime dtm = dateStart;

            while (dtm <=dateEnd)
            {
                sbHeader.AppendFormat("<th {0}>{1}</th>", "colspan=\"2\" class=\"ce-cntr\"", dtm.ToString("dd/MM/yyyy"));

                sbSubHeader.AppendFormat("<th {0}>{1}</th>", "class=\"ce-cntr\"","SL");
                sbSubHeader.AppendFormat("<th {0}>{1}</th>", "class=\"ce-cntr\"", "TL");
                dtm = dtm.AddDays(1);


            }
            sbHeader.Append("</tr>");
            sbHeader.Append("<tr>" + sbSubHeader.ToString() + "</tr>");

            #endregion

            StringBuilder sbBody = new StringBuilder();
            var lstStatus = statuscallRepository.GetAllStatus();
            var lstCallLogs = reportRepository.GetDailyReport(sourceId: sourceId, statusId: 0, ProjectID: CommonHelper.CurrentProject(), dateFrom: dateStart, dateEnd: dateEnd);
            if(lstCallLogs != null)
            {
                double totalLogCall = lstCallLogs.Count();
                foreach (var st in lstStatus)
                {
                    dtm = dateStart;
                    sbBody.Append("<tr>");
                    sbBody.AppendFormat("<td class=\"ce-left\">{0}</td>",st.Name);
                    int totalCallOfStatus = lstCallLogs.Where(p=>p.StatusID == st.StatusID).Count();
                    double ratio= Math.Round((double)(100 * totalCallOfStatus) / totalLogCall);
                    sbBody.AppendFormat("<td {0}>{1}</td>", "class=\"ce-cntr ce-bg\"", totalCallOfStatus);
                    sbBody.AppendFormat("<td {0}>{1}</td>", "class=\"ce-cntr\"", ratio + "%");

                    while (dtm <= dateEnd)
                    {
                        var lstCallofDay = lstCallLogs.Where(p => p.StatusID == st.StatusID && p.CreatedDate.ToString("dd/MM/yyyy") == dtm.Date.ToString("dd/MM/yyyy"));
                        
                        sbBody.AppendFormat("<td {0}>{1}</td>", "class=\"ce-cntr ce-bg\"", lstCallofDay.Count());
                        sbBody.AppendFormat("<td {0}>{1}</td>", "class=\"ce-cntr\"", "0");

                        dtm = dtm.AddDays(1);
                    }
                    sbBody.Append("</tr>");
                }
            }

            return Json( new { success = true, ReportData = "<thead>" + sbHeader.ToString() + "</thead><tbody>" + sbBody.ToString() + "</tbody>" });
       }

        public ActionResult ReportProjectData(int sourceId=0, string ds="",string de="")
        {
            if (!CommonHelper.CheckPermisionExist(Permissions.XemThongKeBaoCaoChiTiet.ToString()))
                return AccessDeniedView();

            var reportView = new TsReportModel();
            if (!string.IsNullOrEmpty(ds) & !string.IsNullOrEmpty(de))
            {
                reportView.DateStart = Converter.ParseDatetime(ds, "MM/dd/yyyy", false);
                reportView.DateEnd = Converter.ParseDatetime(de, "MM/dd/yyyy", false);
            }

            reportView.AvaiableSources.AddRange(sourcesRepository.GetAllSourcesByProject(CommonHelper.CurrentProject()).Select(u => new SelectListItem()
            {
                Text = u.Name,
                Value = u.SourceID.ToString(),
                Selected = u.SourceID == sourceId
            }));

            return View(reportView);
        }

        public FileResult ExportReportProjectData(int sourceId = 0, int statusId = 0, string ds = "", string de = "")
        {
            DateTime dateStart = DateTime.Now.AddDays(-7);
            DateTime dateEnd = DateTime.Now.AddHours(23);

            if (!string.IsNullOrEmpty(ds) & !string.IsNullOrEmpty(de))
            {
                dateStart = Converter.ParseDatetime(ds, "MM/dd/yyyy", false);
                dateEnd = Converter.ParseDatetime(de, "MM/dd/yyyy", false);
            }

            var report = reportRepository.GetReportProjectData(sourceId: sourceId, statusId: statusId, projectID: CommonHelper.CurrentProject(), dateFrom: dateStart, dateEnd: dateEnd);
            var lstCustomerField = customerFieldRepository.GetCustomerFieldByProject(CommonHelper.CurrentProject());
            byte[] bytes;
            using (var stream = new MemoryStream())
            {
                if (stream == null)
                    throw new ArgumentNullException("stream");

                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("DataHeThong");
                    #region Tiêu đề file excel(worksheet)
                    var propertype = new[]{
                        "STT",
                        "Ngày Nhận Data",
                        "Tên Khách Hàng",
                        "National ID",
                        "DOB",
                        "Số Điện Thoại",
                        "Nhà Mạng",
                        "Email",
                        "Dự Án",
                        "Nhu cầu khách hàng",
                        "Loại data",
                        "Nguồn Data",
                        "Điểm tín dụng",
                        "Product Code",
                        "Loan Amount",
                        "Desired Tenor",
                        "Sản Phẩm",
                        "Monthly Income",
                        "Income Type",
                        "Ngày Phân Bổ Data cho CCA",
                        "Tên CCA",
                        "Tên Quản Lý CCA",
                        "Ngày Gọi Điện",
                        "Tình Trạng Cuộc Gọi",
                        "Chi Tiết Tình Trạng Cuộc Gọi",
                        "Note",
                        "Ngày gặp",
                        "Tỉnh/TP",
                        "Quận/huyện",
                        "Địa chỉ gặp",
                    };

                    for (int i = 0; i < propertype.Length; i++)
                    {
                        worksheet.Cells[1, i + 1].Value = propertype[i];
                        worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                        worksheet.Column(i + 1).Width = 20;
                        var border = worksheet.Cells[1, i + 1].Style.Border;
                        border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
                    }

                    int row = 2;
                    #endregion
                    foreach (IDictionary<string, object> rp in report)
                    {
                        #region Load nội dung thống kê có trong store
                        int col = 1;
                        worksheet.Cells[row, col].Value = (row - 1).ToString();
                        col++;
                        worksheet.Cells[row, col].Value = Convert.ToDateTime(rp.FirstOrDefault(x => x.Key == "NgayNhanData").Value).ToString("dd/MM/yyyy HH:mm:ss");
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "full_name").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "national_id").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "dob").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "MobilePhone").Value;
                        col++;
                        worksheet.Cells[row, col].Value = "";// nha mang
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "email").Value;
                        col++;
                        worksheet.Cells[row, col].Value = "";// dự án
                        col++;
                        worksheet.Cells[row, col].Value = "";// Nhu cau khach hang
                        col++;
                        worksheet.Cells[row, col].Value = "";// loai data
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "SourceName").Value;
                        col++;
                        //thieu diem tin dung
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "product_code").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "loan_amount").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "desired_tenor").Value;
                        col++;

                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "target_bank_code").Value;
                        col++;
                        worksheet.Cells[row, col].Value = "";// san pham
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "monthly_income").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "income_type").Value;
                        col++;
                        worksheet.Cells[row, col].Value = Convert.ToDateTime(rp.FirstOrDefault(x => x.Key == "NgayPhanBoData").Value).ToString("dd/MM/yyyy HH:mm:ss");// ngay phan bo data
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TenCCA").Value;//ten cca
                        col++;
                        worksheet.Cells[row, col].Value = ""; // ten ql cca
                        col++;
                        worksheet.Cells[row, col].Value = Convert.ToDateTime(rp.FirstOrDefault(x => x.Key == "NgayGoiSauCung").Value).ToString("dd/MM/yyyy HH:mm:ss"); // ngay goi
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TinhTrangCuocGoi").Value; // tinh trang
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "ChiTietTinhTrangCuocGoi").Value; // chi tiet tinh trang
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "note_call").Value; // note
                        col++;
                        worksheet.Cells[row, col].Value = ""; // ngay gap
                        col++;

                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TS_Teclco").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TS_Source").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TS_Score").Value;
                        col++;
                        worksheet.Cells[row, col].Value = rp.FirstOrDefault(x => x.Key == "TS_Province").Value;
                        col++;
                        #endregion
                        row++;
                    }



                    xlPackage.Save();
                }
                bytes = stream.ToArray();
            }

            return File(bytes, "text/xls", "DataHeThong_" + DateTime.Now.ToString("yyyyMMddHHmm") + ".xlsx");
        }

        public ActionResult GetReportUserCapacity()
        {

            return View();
        }
        #endregion


    }
}
