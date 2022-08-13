using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Reflection;
using System.Text;

namespace MB.Common
{
    public static partial class Converter
    {
        public static string ToStringDefault(this object value, string defaultValue = "")
        {
            return value == null ? defaultValue : value.ToString();
        }

        public static bool ToBoolean(this object value, bool defaultValue = false)
        {
            return value == null ? defaultValue : Convert.ToBoolean(value);
        }

        public static Int32 ToInt(this object value, int defaultValue = 0)
        {
            return value == null ? defaultValue : Convert.ToInt32(value);
        }

        public static Decimal ToDecimal(this object value, decimal defaultValue = 0)
        {
            return value == null ? defaultValue : Convert.ToDecimal(value);
        }

        public static Int16 ToShort(this object value, short defaultValue = 0)
        {
            return value == null ? defaultValue : Convert.ToInt16(value);
        }

        public static DateTime ToDateTime(this object value)
        {
            return Convert.ToDateTime(value);
        }

        public static string ToDateTimeString(this object value, bool includeTime = false)
        {
            if (includeTime)
            {
                return value == null ? string.Empty : string.Format("{0:MM/dd/yyyy hh:mm:ss tt}", value);
            }
            else
            {
                return value == null ? string.Empty : string.Format("{0:MM/dd/yyyy}", value);
            }
        }

        public static string Concatenate<T>(IEnumerable<T> collection, char seperator)
        {
            StringBuilder sb = new StringBuilder();
            foreach (T t in collection)
                sb.Append(t).Append(seperator);
            return sb.Remove(sb.Length - 1, 1).ToString();
        }

        public static object ToDbType(this object value)
        {
            object result = DbType.String;
            var type = value.GetType();
            if (type == typeof(string) || type == typeof(String))
            {
                result = DbType.String;
            }
            else if (type == typeof(int) || type == typeof(Int32))
            {
                result = DbType.Int32;
            }
            else if (type == typeof(Int64) || type == typeof(long))
            {
                result = DbType.Int64;
            }
            else if (type == typeof(bool) || type == typeof(Boolean))
            {
                result = DbType.Boolean;
            }
            else if (type == typeof(bool) || type == typeof(Boolean))
            {
                result = DbType.Boolean;
            }
            else if (type == typeof(DateTime))
            {
                result = DbType.DateTime;
            }
            else if (type == typeof(double) || type == typeof(Double))
            {
                result = DbType.Double;
            }
            else if (type == typeof(decimal) || type == typeof(Decimal))
            {
                result = DbType.Decimal;
            }
            else if (type == typeof(Guid))
            {
                result = DbType.Guid;
            }
            return result;
        }

        #region ParseDateTime
        /// <summary>
        /// Kiểm tra ngày hợp lệ, lấy chuỗi định dạng "DateFormat" trong Web.config
        /// </summary>
        /// <param name="strDate">Chuỗi ngày</param>
        /// <returns></returns>
        public static bool IsValidDate(string strDate)
        {
            return IsValidDate(strDate, "DateFormat", true);
        }
        /// <summary>
        /// Kiểm tra ngày hợp lệ theo chuỗi định dạng
        /// </summary>
        /// <param name="strDate">Chuỗi ngày</param>
        /// <param name="strFormat">Chuỗi định dạng</param>
        /// <param name="bolConfig">Chuỗi định dạng là tên của web.config không?</param>
        /// <returns></returns>
        public static bool IsValidDate(string strDate, string strFormat, bool bolConfig)
        {
            try
            {
                string strDateFormat = "";
                if (bolConfig)
                    strDateFormat = ConfigurationManager.AppSettings[strFormat];
                else
                    strDateFormat = strFormat;
                DateTime dtm = DateTime.ParseExact(strDate, strDateFormat, new System.Globalization.DateTimeFormatInfo());
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Chuyển chuỗi thành ngày tháng dùng định dạng "DateFormat" trong web.config
        /// </summary>
        /// <param name="strDate">Chuỗi ngày</param>
        /// <returns></returns>
        public static DateTime ParseDate(string strDate)
        {
            try
            {
                return ParseDate(strDate, "DateFormat", true);
            }
            catch (Exception objEx)
            {
                throw objEx;
            }
        }
        /// <summary>
        /// Chuyển chuỗi sang kiểu ngày tháng
        /// </summary>
        /// <param name="strDate">Chuỗi ngày</param>
        /// <param name="strFormat">Chuỗi định dạng</param>
        /// <param name="bolConfig">Chuỗi định dạng là tên config hay không?</param>
        /// <returns></returns>
        public static DateTime ParseDate(string strDate, string strFormat, bool bolConfig)
        {
            if (IsValidDate(strDate, strFormat, bolConfig))
            {
                String strDateFormat = "";
                if (bolConfig)
                    strDateFormat = ConfigurationManager.AppSettings[strFormat];
                else
                    strDateFormat = strFormat;
                return DateTime.ParseExact(strDate, strDateFormat, new System.Globalization.DateTimeFormatInfo());
            }
            else
                throw new Exception("Chuỗi không đúng định dạng " + (bolConfig ? ConfigurationManager.AppSettings[strFormat] : strFormat));
        }
        /// <summary>
        /// Chuyển chuỗi thành ngày tháng dùng định dạng "DatetimeFormat" trong web.config
        /// </summary>
        /// <param name="strDate">Chuỗi ngày giờ</param>
        /// <returns></returns>
        public static DateTime ParseDatetime(string strDatetime)
        {
            try
            {
                return ParseDate(strDatetime, "DatetimeFormat", true);
            }
            catch (Exception objEx)
            {
                throw objEx;
            }
        }
        /// <summary>
        /// Chuyển chuỗi sang kiểu ngày giờ
        /// </summary>
        /// <param name="strDate">Chuỗi ngày giờ</param>
        /// <param name="strFormat">Chuỗi định dạng</param>
        /// <param name="bolConfig">Chuỗi định dạng là tên config hay không?</param>
        /// <returns></returns>
        public static DateTime ParseDatetime(string strDatetime, string strFormat, bool bolConfig)
        {
            if (IsValidDate(strDatetime, strFormat, bolConfig))
            {
                String strDateFormat = "";
                if (bolConfig)
                    strDateFormat = ConfigurationManager.AppSettings[strFormat];
                else
                    strDateFormat = strFormat;
                return DateTime.ParseExact(strDatetime, strDateFormat, new System.Globalization.DateTimeFormatInfo());
            }
            else
                throw new Exception("Chuỗi không đúng định dạng " + (bolConfig ? ConfigurationManager.AppSettings[strFormat] : strFormat));
        }

        #endregion
    }

    /// <summary>
    /// Convert an object from TSource to TDestination
    /// </summary>
    public static class EntityConverter
    {
        /// <summary>
        /// Convert from Source Object to Destination Object
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDestination To<TSource, TDestination>(this TSource source)
        {
            var objDest = (TDestination)Activator.CreateInstance(typeof(TDestination));
            foreach (PropertyInfo p in source.GetType().GetProperties())
            {
                foreach (PropertyInfo dest in objDest.GetType().GetProperties())
                {
                    if (dest.Name.Equals(p.Name, StringComparison.Ordinal) && dest.CanWrite && p.GetValue(source) != null && dest.PropertyType.Name == p.PropertyType.Name)
                    {
                        if (dest.PropertyType.Name == "Guid" && p.GetValue(source).ToString() == Guid.Empty.ToString())
                            break;
                        dest.SetValue(objDest, p.GetValue(source), null);
                        break;
                    }
                }
            }

            return objDest;
        }

        /// <summary>
        /// Convert from List of Source Objects to List of Destination Objects
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static List<TDestination> To<TSource, TDestination>(this List<TSource> source)
        {
            List<TDestination> resultList = new List<TDestination>();
            foreach (var item in source)
            {
                resultList.Add(item.To<TSource, TDestination>());
            }
            return resultList;
        }

        /// <summary>
        /// Convert from DataRow to Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T To<T>(this DataRow row)
        {
            var obj = (T)Activator.CreateInstance(typeof(T));
            if (row != null)
            {
                foreach (PropertyInfo p in obj.GetType().GetProperties())
                {
                    if (p.CanWrite)
                    {
                        if (row.Table.Columns.Contains(p.Name) && row[p.Name] != null)
                        {
                            if (p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?))
                            {
                                p.SetValue(obj, decimal.Parse(row[p.Name].ToString()), null);
                            }
                            else if (p.PropertyType == typeof(double) || p.PropertyType == typeof(double?))
                            {
                                p.SetValue(obj, double.Parse(row[p.Name].ToString()), null);
                            }
                            else
                            {
                                p.SetValue(obj, row[p.Name], null);
                            }
                        }
                        else
                        {
                            p.SetValue(obj, null, null);
                        }
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// Convert from DataTable to List of Objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static List<T> To<T>(this DataTable table)
        {
            List<T> ret = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                T t = To<T>(row);
                ret.Add(t);
            }
            return ret;
        }

        /// <summary>
        /// Convert from DataRow to Object by dictionary of mapping defination
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="mappingConfigs">[PropertyName, ColumnName]</param>
        /// <returns></returns>
        public static T To<T>(this DataRow row, Dictionary<string, string> mappingConfigs)
        {
            var obj = (T)Activator.CreateInstance(typeof(T));
            if (row != null)
            {
                PropertyInfo[] props = obj.GetType().GetProperties();
                foreach (PropertyInfo propertyInfo in props)
                {
                    if (propertyInfo.CanWrite)
                    {
                        string propertyName = propertyInfo.Name;
                        string columnName = string.Empty;
                        if (mappingConfigs.ContainsKey(propertyName))
                            columnName = mappingConfigs[propertyName];
                        else
                            columnName = propertyName;
                        bool isExisted = false;
                        if (row.Table.Columns.Contains(columnName))
                            isExisted = true;
                        row.To<T>(obj, propertyInfo, columnName, isExisted);
                    }
                }
            }
            return obj;
        }
        public static IEnumerable<T> To<T>(this DataTable table, Dictionary<string, string> mappingConfigs = null)
        {
            int totalRows = 0;
            return table.To<T>(ref totalRows, mappingConfigs);
        }

        public static IEnumerable<T> To<T>(this DataTable table, ref int totalRows, Dictionary<string, string> mappingConfigs = null)
        {
            List<T> ret = new List<T>();
            if (mappingConfigs == null)
            {
                foreach (DataRow row in table.Rows)
                {
                    T t = row.To<T>();
                    ret.Add(t);
                }
            }
            else
            {
                foreach (DataRow row in table.Rows)
                {
                    T t = row.To<T>(mappingConfigs);
                    ret.Add(t);
                }
            }
            if (table.Rows.Count > 0 && table.Columns.Contains("TotalRows"))
                totalRows = Convert.ToInt32(table.Rows[0]["TotalRows"]);
            return ret;
        }

        private static void To<T>(this DataRow row, T obj, PropertyInfo p, string colName, bool isExistedColumn)
        {
            if (isExistedColumn && row[colName] != null)
            {
                if (p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?))
                {
                    p.SetValue(obj, decimal.Parse(row[colName].ToString()), null);
                }
                else if (p.PropertyType == typeof(double) || p.PropertyType == typeof(double?))
                {
                    p.SetValue(obj, double.Parse(row[colName].ToString()), null);
                }
                else if (p.PropertyType == typeof(string))
                {
                    p.SetValue(obj, row[colName].ToString(), null);
                }
                else
                {
                    p.SetValue(obj, row[colName], null);
                }
            }
            else
            {
                p.SetValue(obj, null, null);
            }
        }

       
    }
}
