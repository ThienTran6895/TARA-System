using System.Text.RegularExpressions;

namespace MB.Common.Helpers
{
    public static class StringHelper
    {
        /// <summary>
        /// Use this function to build a constant string for javascript template;
        /// </summary>
        /// <param name="s">normal string</param>
        /// <returns>a new string which is modified to avoid error syntax in javascript template</returns>
        public static string EncodeJTemplate(this string s)
        {
            return Regex.Replace(s, "#", "\\#");
        }
    }
}
