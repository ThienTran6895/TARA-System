using System.Collections;
using System.Web.Mvc;

namespace MB.Common.Kendoui
{
    public class DataSourceStringResult : ContentResult
    {
        public DataSourceStringResult(string json)
        {
            Content = json;
            ContentType = "application/json";
        }
    }
}
