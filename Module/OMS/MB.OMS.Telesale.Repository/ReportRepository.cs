using DAL.Core;
using Dapper;
using MB.Common;
using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Interface;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Repository
{
    public class ReportRepository : IReportRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.ReportRepository");
        public DataSourceResult ThongKeTheoOverviewDatasource(DataSourceRequest dsRequest, Report model)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", model.ProjectID, DbType.Int32);

                var r = DALHelpers.Query<Report>("ThongKeBaoCaoOverview @ProjectID", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ThongKeBaoCaoOverview, Detail: " + ex.ToString());
                return null;
            }
        }
        public DataSourceStringResult ThongKeTheoNgayDatasource(DataSourceRequest dsRequest, Report model)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", model.ProjectID, DbType.Int32);
                param.Add("@DateFrom", model.DateFrom, DbType.DateTime);
                param.Add("@DateEnd", model.DateEnd, DbType.DateTime);

                var r = DALHelpers.Query<dynamic>("ThongKeBaoCaoDaily @ProjectID,@DateFrom,@DateEnd", new UserLogin(), param).AsEnumerable();
                string jsonRowTemplate = "{{\"StatusID\":{0},\"StatusName\":\"{1}\",\"Total\":\"{2}\",\"SL\":\"{3}\",\"Vol\":\"{4}\",";
                string jsonRowDataTemplate = "\"Total{0}\":\"{1}\",\"SL{0}\":\"{2}\",\"Vol{0}\":\"{3}\"";
                string jsonData = "";
                string json = "{{\"ExtraData\": null,\"Data\": [{0}],\"Errors\":null,\"Total\":{1}}}";
                int days = (int)(DateTime.Parse(model.DateEnd) - DateTime.Parse(model.DateFrom)).TotalDays;

                
                var t = DALHelpers.ToDataTable(r);
                int count = 1;
                foreach (DataRow row in t.Rows)
                {
                    // Build the json text of this line
                    jsonData += string.Format(jsonRowTemplate, row["StatusID"], row["StatusName"], row["Total"], row["SL"], row["Vol"]);
                    for(int i = 1; i <= days; i++)
                    {
                        jsonData += string.Format(jsonRowDataTemplate, i, row["Total" + i], row["SL" + i], row["Vol" + i]);
                        if (i != days)
                            jsonData += ",";
                    }
                    if (count != t.Rows.Count)
                        jsonData += "},";
                    else
                        jsonData += "}";
                    count++;
                }

                json = string.Format(json, jsonData, r.Count());
                return new DataSourceStringResult(json);
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ThongKeBaoCaoDaily, Detail: " + ex.ToString());
                return null;
            }
        }
        public DataSourceResult ThongKeTheoNguonDatasource(DataSourceRequest dsRequest, Report model)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", model.ProjectID, DbType.Int32);
                param.Add("@DateFrom", model.DateFrom, DbType.DateTime);
                param.Add("@DateEnd", model.DateEnd, DbType.DateTime);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);

                var r = DALHelpers.Query<Report>("ThongKeTheoNguonByDate @ProjectID,@DateFrom,@DateEnd,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ThongKeTheoNguonDatasource, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<Report> ThongKeTheoNguon(Report model)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", model.ProjectID, DbType.Int32);
                param.Add("@DateFrom", model.DateFrom, DbType.DateTime);
                param.Add("@DateEnd", model.DateEnd, DbType.DateTime);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                var r = DALHelpers.Query<Report>("ThongKeTheoNguonByDate @ProjectID,@DateFrom,@DateEnd,@PageSize,@Page", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ThongKeTheoNguon, Detail: " + ex.ToString());
                return null;
            }
        }
        public DataSourceResult ThongKeTheoDTVDatasource(DataSourceRequest dsRequest, Report model)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", model.ProjectID, DbType.Int32);
                param.Add("@DateFrom", model.DateFrom, DbType.DateTime);
                param.Add("@DateEnd", model.DateEnd, DbType.DateTime);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);

                var r = DALHelpers.Query<Report>("ThongKeTheoDTVByDate @ProjectID,@DateFrom,@DateEnd,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ThongKeTheoDTVDatasource, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<Report> ThongKeTheoDTV(Report model)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", model.ProjectID, DbType.Int32);
                param.Add("@DateFrom", model.DateFrom, DbType.DateTime);
                param.Add("@DateEnd", model.DateEnd, DbType.DateTime);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);

                var r = DALHelpers.Query<Report>("ThongKeTheoDTVByDate @ProjectID,@DateFrom,@DateEnd,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ThongKeTheoDTV, Detail: " + ex.ToString());
                return null;
            }
        }
        public DataSourceResult ThongKeNangSuatDatasource(DataSourceRequest dsRequest, Report model)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", model.ProjectID, DbType.Int32);
                param.Add("@DateFrom", model.DateFrom, DbType.DateTime);
                param.Add("@DateEnd", model.DateEnd, DbType.DateTime);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);

                var r = DALHelpers.Query<Report>("ThongKeNangSuatByDate @ProjectID,@DateFrom,@DateEnd,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ThongKeNangSuatDatasource, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<Report> ThongKeNangSuat(Report model)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", model.ProjectID, DbType.Int32);
                param.Add("@DateFrom", model.DateFrom, DbType.DateTime);
                param.Add("@DateEnd", model.DateEnd, DbType.DateTime);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);

                var r = DALHelpers.Query<Report>("ThongKeNangSuatByDate @ProjectID,@DateFrom,@DateEnd,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ThongKeNangSuat, Detail: " + ex.ToString());
                return null;
            }
        }
        public DataSourceResult ThongKeBaoCaoChiTietDatasource(DataSourceRequest dsRequest, Report model)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@DateFrom", model.DateFrom, DbType.DateTime);
                param.Add("@DateEnd", model.DateEnd, DbType.DateTime);
                param.Add("@ProjectID", model.ProjectID, DbType.Int32);
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);

                var r = DALHelpers.Query<object>("ThongKeBaoCaoChiTiet @DateFrom,@DateEnd,@ProjectID,@PageSize,@Page", new UserLogin(), param).AsEnumerable();

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.Count() };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ThongKeBaoCaoChiTietDatasource, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<object> ThongKeBaoCaoChiTiet(Report model)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@DateFrom", model.DateFrom, DbType.DateTime);
                param.Add("@DateEnd", model.DateEnd, DbType.DateTime);
                param.Add("@ProjectID", model.ProjectID, DbType.Int32);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);

                var r = DALHelpers.Query<object>("ThongKeBaoCaoChiTiet @DateFrom,@DateEnd,@ProjectID,@PageSize,@Page", new UserLogin(), param).ToList();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ThongKeBaoCaoChiTiet, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<object> ThongKeBaoCaoChiTietSimple(Report model)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@DateFrom", model.DateFrom, DbType.DateTime);
                param.Add("@DateEnd", model.DateEnd, DbType.DateTime);
                param.Add("@ProjectID", model.ProjectID, DbType.Int32);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);

                var r = DALHelpers.Query<object>("ThongKeBaoCaoChiTietSimple @DateFrom,@DateEnd,@ProjectID,@PageSize,@Page", new UserLogin(), param).ToList();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ThongKeBaoCaoChiTiet, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<object> ThongKeBaoCaoChiTietFull(Report model)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@DateFrom", model.DateFrom, DbType.DateTime);
                param.Add("@DateEnd", model.DateEnd, DbType.DateTime);
                param.Add("@ProjectID", model.ProjectID, DbType.Int32);
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);

                var r = DALHelpers.Query<object>("ThongKeBaoCaoChiTietFull @DateFrom,@DateEnd,@ProjectID,@PageSize,@Page", new UserLogin(), param).ToList();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ThongKeBaoCaoChiTiet, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<Report> LoadAllSurveyAnswer(int ProjectID, int CallID, int QuestionID, int SurveyID, string DateFrom, string DateEnd)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@DateFrom", DateFrom, DbType.DateTime);
                param.Add("@DateEnd", DateEnd, DbType.DateTime);
                param.Add("@ProjectID", ProjectID, DbType.Int32);
                param.Add("@CallID", CallID, DbType.Int32);
                param.Add("@QuestionID", QuestionID, DbType.Int32);
                param.Add("@SurveyID", SurveyID, DbType.Int32);
                var r = DALHelpers.Query<Report>("Telesales_LoadAllSurveyAnswer @ProjectID,@CallID,@QuestionID,@SurveyID,@DateFrom,@DateEnd", new UserLogin(), param).ToList();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error LoadAllSurveyAnswer, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<Report> LoadAllSurveyAnswerTK(int ProjectID, string DateFrom, string DateEnd)
        {
            try
            {
                var param = new DynamicParameters();
                if(!string.IsNullOrEmpty(DateFrom))
                    param.Add("@DateFrom", DateFrom, DbType.DateTime);
                else
                    param.Add("@DateFrom", null, DbType.DateTime);
                if (!string.IsNullOrEmpty(DateEnd))
                    param.Add("@DateEnd", DateEnd, DbType.DateTime);
                else
                    param.Add("@DateEnd", null, DbType.DateTime);
                param.Add("@ProjectID", ProjectID, DbType.Int32);
                var r = DALHelpers.Query<Report>("Telesales_LoadAllSurveyAnswerTK @ProjectID,@DateFrom,@DateEnd", new UserLogin(), param).ToList();

                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error LoadAllSurveyAnswerTK, Detail: " + ex.ToString());
                return null;
            }
        }
        public int ReturnCustomerForSource(Guid Id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", Id, DbType.Guid);
                var r = DALHelpers.Execute("telesales_ReturnCustomerForSource", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ReturnCustomerForSource, Detail: " + ex.ToString());
                return 0;
            }
        }

        //sondt - 2019
        public IEnumerable<ReportOverviewData> OverviewDataByStatus(int sourceId, DateTime dateFrom, DateTime dateEnd)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@SourceId", sourceId, DbType.Int32);
                param.Add("@DateStart", dateFrom, DbType.DateTime);
                param.Add("@DateEnd", dateEnd, DbType.DateTime);
                
                var r = DALHelpers.Query<ReportOverviewData>("telesales_Status_GetOverviewDataReport @SourceId,@DateStart,@DateEnd", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error OverviewDataByStatus, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<ReportOverviewData> GetReportStatusCallId(int sourceId, int statusId, DateTime dateFrom, DateTime dateEnd)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@SourceId", sourceId, DbType.Int32);
                param.Add("@StatusId", statusId, DbType.Int32);
                param.Add("@DateStart", dateFrom, DbType.DateTime);
                param.Add("@DateEnd", dateEnd, DbType.DateTime);

                var r = DALHelpers.Query<ReportOverviewData>("telesales_StatusCall_GetReportByStatusId @StatusId,@SourceId,@DateStart,@DateEnd", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error OverviewDataByStatus, Detail: " + ex.ToString());
                return null;
            }
        }

        //
        public IEnumerable<CallLogReportDaily> GetDailyReport(int sourceId, int statusId, int ProjectID, DateTime dateFrom, DateTime dateEnd)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@ProjectID", ProjectID, DbType.Int32);
                param.Add("@SourceId", sourceId, DbType.Int32);
                param.Add("@StatusId", statusId, DbType.Int32);
                param.Add("@DateStart", dateFrom, DbType.DateTime);
                param.Add("@DateEnd", dateEnd, DbType.DateTime);
                

                var r = DALHelpers.Query<CallLogReportDaily>("telesales_Calllog_GetDailyReport @ProjectId,@SourceId,@StatusId,@DateStart,@DateEnd", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error OverviewDataByStatus, Detail: " + ex.ToString());
                return null;
            }
        }

        public IEnumerable<object> GetReportProjectData(int sourceId, int statusId, int projectID, DateTime dateFrom, DateTime dateEnd)
        {
            logger.Info("Starting GetReportProjectData");
            try
            {
                var param = new DynamicParameters();
                param.Add("@DateFrom", dateFrom, DbType.DateTime);
                param.Add("@DateEnd", dateEnd, DbType.DateTime);
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@statusId", statusId, DbType.Int32);
                param.Add("@sourceId", sourceId, DbType.Int32);

                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);

                var r = DALHelpers.Query<object>("spa_Report_GetReportProjectData @DateFrom,@DateEnd,@ProjectID,@statusId,@sourceId,@PageSize,@Page", new UserLogin(), param).ToList();
                logger.Info("Completed");
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error spa_Report_GetReportProjectData, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetReportUserCapacity(int sourceId, int statusId, int projectID, DateTime dateFrom, DateTime dateEnd)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@DateFrom", dateFrom, DbType.DateTime);
                param.Add("@DateEnd", dateEnd, DbType.DateTime);
                param.Add("@ProjectID", projectID, DbType.Int32);
                param.Add("@statusId", statusId, DbType.Int32);
                param.Add("@sourceId", sourceId, DbType.Int32);


                var r = DALHelpers.Query<Report>("spa_Report_GetUserCapacity @DateFrom,@DateEnd,@ProjectID,@statusId,@sourceId", new UserLogin(), param).AsEnumerable();
                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error ThongKeNangSuatDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

    }
}
