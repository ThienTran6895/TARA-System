using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface ICampaignsRepository
    {
        IEnumerable<Campaigns> GetAll(string name = null, bool? visiable = null, bool? isDeleted = null);
        DataSourceResult GetCampaignsDatasource(MB.Common.Kendoui.DataSourceRequest dsRequest, string name = null, bool? visiable = null, bool? isDeleted = null);
        int AddNewCampaigns(Campaigns campaigns);
        int UpdateCampaigns(Campaigns campaigns);
        int DeleteCampaigns(int id);
        Campaigns GetCampaigns(int id);
    }
}
