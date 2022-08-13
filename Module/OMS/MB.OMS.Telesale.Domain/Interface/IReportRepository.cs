using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IReportRepository
    {
        DataSourceResult ThongKeTheoOverviewDatasource(DataSourceRequest dsRequest, Report model);
        DataSourceStringResult ThongKeTheoNgayDatasource(DataSourceRequest dsRequest, Report model);
        DataSourceResult ThongKeTheoNguonDatasource(DataSourceRequest dsRequest, Report model);
        IEnumerable<Report> ThongKeTheoNguon(Report model);
        DataSourceResult ThongKeTheoDTVDatasource(DataSourceRequest dsRequest, Report model);
        DataSourceResult ThongKeNangSuatDatasource(DataSourceRequest dsRequest, Report model);
        IEnumerable<Report> ThongKeNangSuat(Report model);
        IEnumerable<Report> ThongKeTheoDTV(Report model);
        DataSourceResult ThongKeBaoCaoChiTietDatasource(DataSourceRequest dsRequest, Report model);
        IEnumerable<object> ThongKeBaoCaoChiTietSimple(Report model);
        IEnumerable<object> ThongKeBaoCaoChiTietFull(Report model);
        IEnumerable<object> ThongKeBaoCaoChiTiet(Report model);
        IEnumerable<Report> LoadAllSurveyAnswer(int ProjectID, int CallID, int QuestionID, int SurveyID, string DateFrom, string DateEnd);
        int ReturnCustomerForSource(Guid Id);
        IEnumerable<Report> LoadAllSurveyAnswerTK(int ProjectID, string DateFrom, string DateEnd);

        // sondt - 2019
        IEnumerable<ReportOverviewData> OverviewDataByStatus(int sourceId, DateTime dateFrom, DateTime dateEnd);
        IEnumerable<ReportOverviewData> GetReportStatusCallId(int sourceId, int statusId, DateTime dateFrom, DateTime dateEnd);
        IEnumerable<CallLogReportDaily> GetDailyReport(int sourceId, int statusId, int ProjectID, DateTime dateFrom, DateTime dateEnd);
        IEnumerable<object> GetReportProjectData(int sourceId, int statusId, int projectID, DateTime dateFrom, DateTime dateEnd);
    }
}
