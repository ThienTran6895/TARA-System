using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using DAL.Core;
using MB.Common;
using MB.Common.Cache;
using MB.OMS.Common.Domain;
using MB.OMS.Common.Domain.Model;

namespace MB.OMS.Common.Repository
{
    public static class StaticCommonRepository
    {
        public static IEnumerable<StaticData> GetStaticData(StaticDataKey datasourceName, string language)
        {
            object refInfo = CacheManager.Get(datasourceName.ToString());

            var refInfos = refInfo as IEnumerable<StaticData>;
            if (refInfos != null && refInfos.Any())
                return refInfo as IEnumerable<StaticData>;
            var result = GetStaticDatasource(datasourceName.ToString(), language);
            CacheManager.Set(datasourceName.ToString(), result, CacheDuration.Long);
            return result;
        }


        private static List<StaticData> GetStaticDatasource(string datasourceName, string language)
        {
            var param = new DynamicParameters();
            param.Add("@DatasourceName", datasourceName, DbType.String);
            param.Add("@Language", language, DbType.String);
            return DALHelpers.Query<StaticData>("spa_GetStaticData @DatasourceName,@Language", new UserLogin() { UserId = "2978C3E7-B198-47EF-BDF7-A6DF5FEEB7B0", AppId = 1 }, param).ToList();
        }
        
    }
}
