using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.IO;

namespace Shopping
{
    /// <summary>
    /// Summary description for uploadify
    /// </summary>
    public class uploadifyslider : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            try
            {
                HttpPostedFile file = context.Request.Files["Filedata"];
                string id = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString();
                string extImage = "";
                if (Regex.IsMatch(file.FileName, "."))
                {
                    var arr = file.FileName.Split('.');
                    extImage = arr[arr.Length - 1];
                }

                string strReturn = "{\"id\":" + id + ",\"name\":\"" + id + "_" + file.FileName + "\",\"url\":\"\\/uploads\\/images\\/slider\\/" + id + "." + extImage + "\"}";
                if (!Directory.Exists(HttpContext.Current.Request.MapPath("~/uploads/images/slider/")))
                {
                    Directory.CreateDirectory(HttpContext.Current.Request.MapPath("~/uploads/images/slider/"));
                }
                file.SaveAs(HttpContext.Current.Request.MapPath("~/uploads/images/slider/") + "\\" + id + "." + extImage);
                context.Response.ContentType = "text/plain";
                context.Response.Write(strReturn);
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "text/plain";
                context.Response.Write(ex);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}