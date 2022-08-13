using System.ComponentModel;

namespace MB.Common
{
    public static class JsonMessage
    {
        public const string Success = "success";
        public const string Failed = "failed";
    }
    public class SpecialDateFormat
    {
        public const string MMYY = "MM/YY";
    }

    public enum Operator
    {
        [Description("greater than")]
        GreaterThan = 1,
        [Description("greater than or equal to")]
        GreaterThanOrEqual = 2,
        [Description("less than")]
        LessThan = 3,
        [Description("less than or equal to")]
        LessThanOrEqual = 4,
        [Description("equal to")]
        EqualTo = 5
    }

    public enum MBFormat
    {
        MMYY = 1,
        Time = 2,
        AlphaNumeric = 3,
        Alpha = 4,
        Numeric = 5,
        Phone = 6,
        Fax = 7,
        WebsiteAddress = 8,
        Number=9,
        Date = 10,
        DateTime=11,
    }
}