using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IProjectsRepository
    {
        IEnumerable<Projects> GetAll(int? campaignID = null, string code = null, string name = null, bool? visiable = null);
        DataSourceResult GetProjectsDatasource(DataSourceRequest dsRequest, int? campaignID = null, string code = null, string name = null, bool? visiable = null);
        DataSourceResult GetProjectsByUserDatasource(Guid? userId = null);
        int AddNewProjects(Projects projects);
        int UpdateProjects(Projects projects);
        int DeleteProjects(int id);
        ProjectsDTO GetProjectsById(int id);
    }
}
