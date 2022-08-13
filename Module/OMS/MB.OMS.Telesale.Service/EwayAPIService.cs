using log4net;
using MB.Common;
using MB.OMS.Telesale.Domain.Interface;
using MB.OMS.Telesale.Domain.Model;
using MB.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MB.Common.Helpers;
using MB.Common.Kendoui;
using System.Configuration;
using Microsoft.Practices.Unity;
using System.Net;
using System.Web;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace MB.OMS.Telesale.Service
{
    public class EwayAPIService : IEwayAPIService
    { 

        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.Eway");
        #region Fields
        [Dependency]
        public ICampaignsRepository campaignsRepository { get; set; }

        [Dependency]
        public IProjectsRepository projectsRepository { get; set; }

        [Dependency]
        public ISourcesRepository sourcesRepository { get; set; }

        [Dependency]
        public IStatusCallRepository statusCallRepository { get; set; }

        [Dependency]
        public ICustomerRepository customerRepository { get; set; }

        [Dependency]
        public ISurveyRepository surveyRepository { get; set; }

        [Dependency]
        public ICustomerFieldRepository customerFieldRepository { get; set; }

        [Dependency]
        public ICustomerFieldValueRepository customerFieldValueRepository { get; set; }

        [Dependency]
        public ICustomerExistRepository customerExistRepository { get; set; }

        [Dependency]
        public ICustomerExistFieldValueRepository customerExistFieldValueRepository { get; set; }

        [Dependency]
        public ICustomerErrorRepository customerErrorRepository { get; set; }

        [Dependency]
        public ICustomerErrorFieldValueRepository customerErrorFieldValueRepository { get; set; }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="status">Conversion status with format: (0 | 1 | -1) corressponding to (Pending | Success | Cancel)</param>
        /// <returns></returns>
        public bool EwayExportData(NameValueCollection fields, int status)
        {
            try
            {
                using (var client = new WebClient())
                {
                    string requestUrl = ConfigurationManager.AppSettings["EWayApiUrl"].Replace("{advertiser_id}", ConfigurationManager.AppSettings["EWayAdvertiserId"]);
                    string query = string.Format("?click_id={0}&advertiser_id={1}&api_key={2}&offer_type={3}&offer_id={4}&advertiser_offer_id={5}&transaction_id={6}&status={7}&status_message={8}",
                        fields["click_id"],
                        ConfigurationManager.AppSettings["EWayAdvertiserId"],
                        ConfigurationManager.AppSettings["EWayApiKey"],
                        ConfigurationManager.AppSettings["EWayOfferType"],
                        fields["offer_id"],
                        fields["product_id"],
                        fields["transaction_id"],
                        status,
                        HttpUtility.UrlEncode(fields["status_message"])
                        );
                    requestUrl += query;
                    string result = client.DownloadString(requestUrl);

                    // Log
                    logger.Info(string.Format("API export send: {0}\n{1}", JsonConvert.SerializeObject(fields), query));
                    logger.Info(string.Format("API export received: {0} \n", result));
                }
            }
            catch (Exception ex)
            {
                logger.Fatal(string.Format("API export exception at {0}: \n{1}", ex.Message, ex.StackTrace));
            }
            return true;
        }

    }
}
