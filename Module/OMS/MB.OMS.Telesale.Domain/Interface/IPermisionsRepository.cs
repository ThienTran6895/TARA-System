using MB.Common.Kendoui;
using MB.OMS.Telesale.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MB.OMS.Telesale.Domain.Interface
{
    public interface IPermisionsRepository
    {
        IEnumerable<PermisionsDTO> GetPermisionsByParent(int? parent = null);
        PermisionsDTO GetPermisionsById(int id);
        DataSourceResult GetAllPermisionsInRolesByParentDatasource(DataSourceRequest dsRequest, int roleID, string permisionName = null, bool? isVisible = null);
        DataSourceResult GetAllPermisionsNotInRolesByParentDatasource(DataSourceRequest dsRequest, int roleID, string permisionName = null, bool? isVisible = null);
        IEnumerable<PermisionsDTO> GetAllPermisionsInRolesByParent(int roleID, string permisionName = null, bool? isVisible = null);
    }
}
