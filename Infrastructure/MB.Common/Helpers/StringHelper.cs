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

        public static string PhoneValidate()
        {
            //return @"^0((1\d{9})|(2(0|1([0-1]|[8-9])|2|(3|4|8)[0-1]|[5-7]|9)\d{7})|(3([0-1]|(2|5)[0-1]|[3-4]|[6-9])\d{7})|(4\d{8,9})|(5([2-9]|[0-1][0-1])\d{7})|(6([0-4]|[6-9]|5[0-1])\d{7})|(7(0|[2-7]|(1|8)[0-1]|9)\d{7})|((8|9)\d{8}))$";
            return @"^0((1\d{9})|(2\d{9})|(2(0|1([0-1]|[8-9])|2|(3|4|8)[0-1]|[5-7]|9)\d{7})|(3([0-1]|(2|5)[0-1]|[3-4]|[6-9])\d{7})|(4\d{8,9})|(5([2-9]|[0-1][0-1])\d{7})|(6([0-4]|[6-9]|5[0-1])\d{7})|(7(0|[2-7]|(1|8)[0-1]|9)\d{7})|((8|9)\d{8}))$";
        }
       
    }
}
