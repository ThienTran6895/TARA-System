using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Handlers;
using System.Web.Hosting;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;

namespace MB.Common.Helpers
{
    public static class CommonHelper
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());
            DescriptionAttribute attribute =
                Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static string GetDisplayNameMetaData(Type objectType, string propertyName)
        {
            var metadata = ModelMetadataProviders.Current.GetMetadataForProperty(null, objectType, propertyName);
            string displayName = string.Empty;

            if (metadata != null)
                displayName = metadata.GetDisplayName();

            return displayName;
        }

        public static string GetPropertyName<TSource, TField>(this TSource model, Expression<Func<TSource, TField>> field)
        {
            if (object.Equals(field, null))
            {
                return string.Empty;
            }
            return (field.Body as MemberExpression ?? ((UnaryExpression)field.Body).Operand as MemberExpression).Member.Name;
        }
        public static IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }
        #region resources
        static MethodInfo s_resourceUrlMethod = null;
        public static string GetResourceUrl<TAssemblyObject>(string resourcePath)
        {
            if (s_resourceUrlMethod == null)
            {
                var methodCandidates = typeof(AssemblyResourceLoader).GetMember("GetWebResourceUrlInternal", BindingFlags.NonPublic | BindingFlags.Static).ToList();
                foreach (var methodCandidate in methodCandidates)
                {
                    var method = methodCandidate as MethodInfo;
                    if (method == null || method.GetParameters().Length != 5) continue;
                    s_resourceUrlMethod = method;
                    break;
                }
            }
            return string.Format("{0}", s_resourceUrlMethod.Invoke(
                        null,
                        new object[] { typeof(TAssemblyObject).Assembly, resourcePath, false, false, null })
                    );
        }
        #endregion

        public static int GetColumnIndex(List<String> properties, string columnName)
        {
            for (int i = 0; i < properties.Count; i++)
                if (properties[i].Equals(columnName))
                    return i + 1; //excel indexes start from 1
            return 0;
        }

        public static string ConvertColumnToString(object columnValue)
        {
            if (columnValue == null)
                return "";

            return Convert.ToString(columnValue).Trim();
        }

        public static int CurrentProject()
        {
            var projectID =  CookieStore.GetCookie("ProjectID");
            if (string.IsNullOrEmpty(projectID))
                return 0;
            else
                return Convert.ToInt32(projectID);
        }

        public static bool CheckPermisionExist(string permision)
        {
            if (HttpContext.Current.Session["UserPermissions"] == null)
            {
                // Session expired but still logged in
                HttpContext.Current.Response.Redirect("~/Account/User/Login");
                return false;
            }
            else if (System.Web.HttpContext.Current.Session["UserPermissions"].ToString().Contains(permision))
                return true;
            else
                return false;
        }

        public static string MapPath(string path)
        {
            if (HostingEnvironment.IsHosted)
            {
                //hosted
                return HostingEnvironment.MapPath(path);
            }

            //not hosted. For example, run in unit tests
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(baseDirectory, path);
        }

        public static void SendEmail(string subject, string body, string toAddress, string toName,
             IEnumerable<string> bcc = null, IEnumerable<string> cc = null)
        {
            var message = new MailMessage();

            //to
            message.To.Add(new MailAddress(toAddress, toName));

            //BCC
            if (bcc != null)
            {
                foreach (var address in bcc.Where(bccValue => !String.IsNullOrWhiteSpace(bccValue)))
                {
                    message.Bcc.Add(address.Trim());
                }
            }

            //CC
            if (cc != null)
            {
                foreach (var address in cc.Where(ccValue => !String.IsNullOrWhiteSpace(ccValue)))
                {
                    message.CC.Add(address.Trim());
                }
            }

            //content
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            //send email
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Send(message);
            }
        }

        #region cắt định dạng html

        public static string CutString(int numOfChar, string str)
        {
            str = cuthtml(str);
            if (numOfChar > str.Length)
                return str;
            str = str.Substring(0, numOfChar);
            str = str.Substring(0, str.LastIndexOf(' ')) + "...";
            return str;
        }
        public static string cuthtml(string str)
        {
            str = Regex.Replace(str, "<(.[^>]*)>", "");
            return str;
        }
        public static string Decode_RemoveHTML(string input)
        {
            string output = string.Empty;

            output = HttpUtility.HtmlDecode(input);
            output = Regex.Replace(output, "<.*?>", String.Empty);

            return output;
        }
        #endregion
    }

    public class OldSystemPasswordHasher : PasswordHasher
    {
        public override string HashPassword(string password)
        {
            return base.HashPassword(password);
        }

        public override PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            return base.VerifyHashedPassword(hashedPassword, providedPassword);
            //Here we will place the code of password hashing that is there in our current solucion.This will take cleartext anad hash
            //Just for demonstration purpose I always return true.	
            if (true)
            {


                return PasswordVerificationResult.SuccessRehashNeeded;
            }
            else
            {
                return PasswordVerificationResult.Failed;
            }
        }
    }
}
