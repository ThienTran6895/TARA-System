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
    public class CampaignsRepository : ICampaignsRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.CampaignsRepository");
        public IEnumerable<Campaigns> GetAll(string name = null, bool? visiable = null, bool? isDeleted = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@Name", name, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@IsDeleted", isDeleted, DbType.Boolean);
                var r = DALHelpers.Query<Campaigns>("telesales_GetAllCampaign @PageSize,@Page,@Name,@Visiable,@IsDeleted", new UserLogin(), param).ToList();
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error GetAll, Detail:" + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetCampaignsDatasource(DataSourceRequest dsRequest, string name = null, bool? visiable = null, bool? isDeleted = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@Name", name, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                param.Add("@IsDeleted", isDeleted, DbType.Boolean);
                var r = DALHelpers.Query<Campaigns>("telesales_GetAllCampaign @PageSize,@Page,@Name,@Visiable,@IsDeleted", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetCampaignsDatasource, Detail:" + ex.ToString());
                return null;
            }
        }

        public int AddNewCampaigns(Campaigns campaigns)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Name", campaigns.Name, DbType.String);
                param.Add("@Visiable", campaigns.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", campaigns.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Query<int>("telesales_AddNewCampaign @Name,@Visiable,@IsDeleted", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error AddNewCampaigns, Detail:" + ex.ToString());
                return 0;
            }
        }

        public int UpdateCampaigns(Campaigns campaigns)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CampaignID", campaigns.CampaignID, DbType.Int32);
                param.Add("@Name", campaigns.Name, DbType.String);
                param.Add("@Visiable", campaigns.Visiable, DbType.Boolean);
                param.Add("@IsDeleted", campaigns.IsDeleted, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateCampaign", new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error UpdateCampaigns, Detail:" + ex.ToString());
                return 0;
            }
        }

        public int DeleteCampaigns(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@CampaignID", id, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteCampaign", new UserLogin(), param);
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error DeleteCampaigns, Detail:" + ex.ToString());
                return 0;
            }
        }

        public Campaigns GetCampaigns(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Int32);
                var r = DALHelpers.Query<Campaigns>("telesales_GetCampaignById @Id", new UserLogin(), param).First();
                return r;
            }
            catch(Exception ex)
            {
                logger.Fatal("Error GetCampaigns, Detail:" + ex.ToString());
                return null;
            }
        }
    }
}
