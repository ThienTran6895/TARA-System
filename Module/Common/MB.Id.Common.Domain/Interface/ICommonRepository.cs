using System.Collections.Generic;
using MB.OMS.Common.Domain.Model;

namespace MB.OMS.Common.Domain.Interface
{
    public interface ICommonRepository
    {
        IEnumerable<StaticData> GetStaticData(StaticDataKey datasourceName);
    }
}
