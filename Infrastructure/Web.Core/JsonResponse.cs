using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using MB.Common;

namespace MB.Web.Core
{


    public class JsonResponse
    {
        public string Result { get; set; }
        public object Data { get; set; }
        public string Message { get; set; }
        public string Title { get; set; }
        public string URLList { get; set; }
        public string URLEdit { get; set; }
        public string Id { get; set; }

        public JsonResponse() { }
        public JsonResponse(object entity = null, string result = JsonMessage.Success, string message = null, string title = null, string urllist = null, string urledit = null, string id = null)
        {
            Result = result;
            Data = entity;
            Message = message;
            Title = title;
            URLList = urllist;
            URLEdit = urledit;
            Id = id;
        }
    }
}
