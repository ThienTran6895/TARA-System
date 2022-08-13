using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MB.OMS.Telesale.Domain.Model
{
    public class Report
    {
        // Overview
        public string TimePeriod { get; set; }
        public int TotalPlan { get; set; }
        public int TotalTarget { get; set; }
        public string PercentTarget { get; set; }
        public int MonthlyPlan { get; set; }
        public string MonthlyTarget { get; set; }
        public int TotalReceived { get; set; }
        public string PercentReceived { get; set; }
        public int TotalHandled { get; set; }
        public string PercentHandled { get; set; }
        public int TotalIncorrect { get; set; }
        public string PercentIncorrect { get; set; }
        public int TotalOrder { get; set; }
        public string PercentOrder { get; set; }
        public string GAP { get; set; }

        //Thong Ke Theo DTV
        public int STT { get; set; }
        public Guid Id { get; set; }
        public string HoTen { get; set; }
        public string UserName { get; set; }
        public int DaNhan { get; set; }
        public int DaXuLy { get; set; }
        public int ChuaGoi { get; set; }
        public int DangHenGoiLai { get; set; }

        //Thong Ke Theo Nguon
        public int SourceID { get; set; }
        public int Thang { get; set; }
        public string Name { get; set; }
        public int TongSo { get; set; }
        public int DaPhanCong { get; set; }
        public int ChuaXuLy { get; set; }

        //Thong Ke Nang Suat
        public int ProjectID { get; set; }
        public int TongSoKHDaGoi { get; set; }
        public int KHThanhCong { get; set; }
        public int KHTiemNang { get; set; }
        public int KHTuChoi { get; set; }
        public int KHSaiSo { get; set; }
        public int KHHenGoiLai { get; set; }
        public string FullName { get; set; }
        public string DateFrom { get; set; }
        public string DateEnd { get; set; }
        public string Phone { get; set; }
        public int? StatusCallID { get; set; }
        public int Total { get; set; }


        // CallLog
        public int CallLogID { set; get; }
        public int CallID { get; set; }
        public int QuestionID { set; get; }
        public int SurveyID { set; get; }
        public string SurveyContent { set; get; }

    }

    public class TsReportModel
    {
        public TsReportModel()
        {
            AvaiableSources = new List<SelectListItem>();
            ReportData = new List<ReportOverviewData>();
            DateStart = DateTime.Now.AddDays(-7);
            DateEnd = DateTime.Now;
        }

        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public List<SelectListItem> AvaiableSources { get; set; }
        public IEnumerable<ReportOverviewData> ReportData { get; set; }
    }


    public class ReportOverviewData
    {
        public string RowTitle { get; set; }
        public int Number { get; set; }
        public int Ratio { get; set; }
        public double TotalDataOfSource { get; set; }
    }

    public class ReportTotalCallByDaily
    {
        public ReportTotalCallByDaily()
        {
            AvaiableSources = new List<SelectListItem>();
            DateStart = DateTime.Now.AddDays(-7);
            DateEnd = DateTime.Now;

        }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public List<SelectListItem> AvaiableSources { get; set; }
    }
    public class ReportTotalCallByDailyData
    {
        public StringBuilder ReportDataHeader { get; set; }
        public StringBuilder ReportData { get; set; }
    }

    public class CallLogReportDaily
    {
        public int StatusID { get; set; }
        public DateTime CreatedDate { get; set; }
        public int StatusCallID { get; set; }

    }

}
