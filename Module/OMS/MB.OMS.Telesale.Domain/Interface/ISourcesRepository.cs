using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface ISourcesRepository
    {
        IEnumerable<Sources> GetAll(string name = null, string link = null, bool? visiable = null);
        DataSourceResult GetSourcesDatasource(DataSourceRequest dsRequest, string name = null, string link = null, bool? visiable = null);
        int AddNewSources(Sources sources);
        int UpdateSources(Sources sources);
        int DeleteSources(int id);
        int DeleteSourcesNew(int id);
        Sources GetSourcesById(int id);
        Sources GetSourcesByName(string name);
        IEnumerable<Sources> GetAllSourcesByProject(int ProjectID);
    }
}
