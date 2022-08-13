using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using MB.Common;
using System.Linq;
using System.Dynamic;
using System.Web.Script.Serialization;

namespace DAL.Core
{
    public static class DALHelpers
    {
        internal static string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        private const string defaultParam = " @UserId,@AppId";
        public static IEnumerable<T> Query<T>(string sql, UserLogin userInfo, DynamicParameters param = null)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                if (param == null)
                {
                    param = new DynamicParameters();
                    sql = sql + defaultParam;
                }
                else
                {
                    sql = sql.Replace(" ", defaultParam + ",");
                }

                param.Add("@UserId", userInfo.UserId, DbType.String);
                param.Add("@AppId", userInfo.AppId, DbType.Int32);
                //return conn.Query<T>(sql, param);
                return conn.Query<T>(sql, param,commandTimeout:720);
            }
        }

        public static dynamic Query(string sql, UserLogin userInfo, DynamicParameters param = null)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                if (param == null)
                {
                    param = new DynamicParameters();
                    sql = sql + defaultParam;
                }
                else
                {
                    sql = sql.Replace(" ", defaultParam + ",");
                }

                param.Add("@UserId", userInfo.UserId, DbType.String);
                param.Add("@AppId", userInfo.AppId, DbType.Int32);
                //return conn.Query<T>(sql, param);
                return conn.Query(sql, param, commandTimeout: 720);
            }
        }

        public static int Execute(string sql, UserLogin userInfo, DynamicParameters param = null)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                if (param == null)
                {
                    param = new DynamicParameters();
                    sql = sql + defaultParam;
                }
                else
                {
                    sql = sql.Replace(" ", defaultParam + ",");
                }

                param.Add("@UserId", userInfo.UserId, DbType.String);
                param.Add("@AppId", userInfo.AppId, DbType.Int32);

                //try
                //{
                //    conn.Execute(sql, param, commandType: CommandType.StoredProcedure);
                //}
                //catch (Exception exception)
                //{
                //    //TODO: write log
                //    return 0;
                //}
                return conn.Execute(sql, param, commandType: CommandType.StoredProcedure);
            }
        }
        public static DataTable ToDataTable(IEnumerable<dynamic> items)
        {
            if (items == null) return null;
            var data = items.ToArray();
            if (data.Length == 0) return null;

            var dt = new DataTable();
            foreach (var pair in ((IDictionary<string, object>)data[0]))
            {
                dt.Columns.Add(pair.Key, (pair.Value ?? string.Empty).GetType());
            }
            foreach (var d in data)
            {
                dt.Rows.Add(((IDictionary<string, object>)d).Values.ToArray());
            }
            return dt;
        }
        public static IEnumerable<dynamic> DataTableToList(DataTable dt)
        {
            var data = new List<dynamic>();
            foreach (var item in dt.AsEnumerable())
            {
                // Expando objects are IDictionary<string, object>  
                IDictionary<string, object> dn = new ExpandoObject();

                foreach (var column in dt.Columns.Cast<DataColumn>())
                {
                    dn[column.ColumnName] = item[column];
                }

                data.Add(dn);
            }
            return data;
        }
        public static string ConvertDataTabletoString(System.Data.DataTable dt)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            Dictionary<string, object> row;
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                row = new Dictionary<string, object>();
                foreach (System.Data.DataColumn col in dt.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return serializer.Serialize(rows);
        }
    }
}
