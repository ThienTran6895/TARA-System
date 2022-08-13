using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Mvc;

namespace MB.Common
{
    [DataContract]
    public class Sort
    {
        /// <summary>
        /// Gets or sets the name of the sorted field (property).
        /// </summary>
        [DataMember(Name = "field")]
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the sort direction. Should be either "asc" or "desc".
        /// </summary>
        [DataMember(Name = "dir")]
        public string Dir { get; set; }

        /// <summary>
        /// Converts to form required by Dynamic Linq e.g. "Field1 desc"
        /// </summary>
        public string ToExpression()
        {
            return Field + " " + Dir;
        }
    }

    public class DataSourceRequest
    {
        /// <summary>
        /// Specifies how many items to take.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Specifies how many items to skip.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Specifies the requested sort order.
        /// </summary>
        public List<Sort> Sort { get; set; }

        /// <summary>
        /// Specifies the requested filter.
        /// </summary>
        public Filter Filter { get; set; }
    }
    public class PagingInfo
    {
        public string GridId { get; set; }
        public bool IsCountPageTotal { get; set; }
        public bool IsPaging { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<Sort> sort { get; set; }
        public string SortDirection { get; set; }
     
    }

    public class DataSourceResult
    {
        public IEnumerable Data { get; set; }
        public object Errors { get; set; }
        public int Total { get; set; }
    }

    public class GridDataSource<T>
    {
        public List<T> DataSource { get; set; }
        public long TotalRows { get; set; }
        public DataSourceResult DataSourceResult { get; set; }
    }

    public static class GridDataSourceExtension
    {
        public static GridDataSource<T> To<T>(this List<T> list)
        {
            long totalRow = 0;
            if (list.Count > 0)
            {
                dynamic obj = list[0];
                if (obj.GetType().GetProperty("TotalRows") != null)
                    totalRow = obj.TotalRows;
            }

            DataSourceResult dataSource = new DataSourceResult()
            {
                Data = list,
                Total = (int)totalRow
            };

            return new GridDataSource<T>()
            {
                DataSource = list,
                TotalRows = totalRow,
                DataSourceResult = dataSource
            };
        }

        public static GridDataSource<TDestination> ToGridDataSource<TSource, TDestination>(this List<TSource> source)
        {
            return source.To<TSource, TDestination>().To<TDestination>();
        }

        public static JsonResult ToJsonDataSource(this object data, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            return new JsonResult()
            {
                Data = data,
                JsonRequestBehavior = behavior
            };
        }
    }
}
