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
    public class SourcesRepository : ISourcesRepository
    {
        internal static readonly log4net.ILog logger = log4net.LogManager.GetLogger("OMS.SourcesRepository");
        public IEnumerable<Sources> GetAll(string name = null, string link = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@Name", name, DbType.String);
                param.Add("@Link", link, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                var r = DALHelpers.Query<Sources>("telesales_GetAllSources @PageSize,@Page,@Name,@Link,@Visiable", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAll, Detail: " + ex.ToString());
                return null;
            }
        }

        public DataSourceResult GetSourcesDatasource(DataSourceRequest dsRequest, string name = null, string link = null, bool? visiable = null)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", dsRequest.PageSize, DbType.Int32);
                param.Add("@Page", dsRequest.Page, DbType.Int32);
                param.Add("@Name", name, DbType.String);
                param.Add("@Link", link, DbType.String);
                param.Add("@Visiable", visiable, DbType.Boolean);
                var r = DALHelpers.Query<Sources>("telesales_GetAllSources @PageSize,@Page,@Name,@Link,@Visiable", new UserLogin(), param).AsQueryable().Sort(dsRequest.Sort);

                return new DataSourceResult() { Data = r, Total = r.Count() == 0 ? 0 : r.First().Total };
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetSourcesDatasource, Detail: " + ex.ToString());
                return null;
            }
        }

        public int AddNewSources(Sources sources)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Name", sources.Name, DbType.String);
                param.Add("@Link", sources.Link, DbType.String);
                param.Add("@Visiable", sources.Visiable, DbType.Boolean);
                var r = DALHelpers.Query<int>("telesales_AddNewSources @Name,@Link,@Visiable", new UserLogin(), param).AsEnumerable().FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error AddNewSources, Detail: " + ex.ToString());
                return 0;
            }
        }

        public int UpdateSources(Sources sources)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@SourceID", sources.SourceID, DbType.Int32);
                param.Add("@Name", sources.Name, DbType.String);
                param.Add("@Link", sources.Link, DbType.String);
                param.Add("@Visiable", sources.Visiable, DbType.Boolean);
                var r = DALHelpers.Execute("telesales_UpdateSources", new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error UpdateSources, Detail: " + ex.ToString());
                return 0;
            }
        }
        public int DeleteSourcesNew(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@SourceID", id, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteSourcesNew",
                    new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteSourcesNew, Detail: " + ex.ToString());
                return 0;
            }
        }
        public int DeleteSources(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@SourceID", id, DbType.Int32);
                var r = DALHelpers.Execute("telesales_DeleteSources",
                    new UserLogin(), param);
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error DeleteSources, Detail: " + ex.ToString());
                return 0;
            }
        }

        public Sources GetSourcesById(int id)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Id", id, DbType.Int32);
                var r = DALHelpers.Query<Sources>("telesales_GetSourcesById @Id", new UserLogin(), param).FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetSourcesById, Detail: " + ex.ToString());
                return null;
            }
        }
        public IEnumerable<Sources> GetAllSourcesByProject(int ProjectID)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@PageSize", null, DbType.Int32);
                param.Add("@Page", null, DbType.Int32);
                param.Add("@ProjectID", ProjectID, DbType.Int32);
                var r = DALHelpers.Query<Sources>("telesales_GetAllSourcesByProject @PageSize,@Page,@ProjectID", new UserLogin(), param).ToList();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetAllSourcesByProject, Detail: " + ex.ToString());
                return null;
            }
        }
        public Sources GetSourcesByName(string name)
        {
            try
            {
                var param = new DynamicParameters();
                param.Add("@Name", name, DbType.String);
                var r = DALHelpers.Query<Sources>("telesales_GetSourcesByName @Name", new UserLogin(), param).FirstOrDefault();
                return r;
            }
            catch (Exception ex)
            {
                logger.Fatal("Error GetSourcesByName, Detail: " + ex.ToString());
                return null;
            }
        }
    }
}
