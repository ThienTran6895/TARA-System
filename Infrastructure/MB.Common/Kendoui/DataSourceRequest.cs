using System.Collections.Generic;

namespace MB.Common.Kendoui
{
    public class DataSourceRequest
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public IEnumerable<Sort> Sort { get; set; }

        public DataSourceRequest()
        {
            this.Page = 1;
            this.PageSize = 50;
            this.Sort = null;
        }
    }
}
