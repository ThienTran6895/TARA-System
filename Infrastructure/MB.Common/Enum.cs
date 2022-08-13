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

    public enum TypeSurvey
    {
        [Description("Nhập liệu")]
        TEXTBOX = 1,
        [Description("Hộp chọn")]
        CHECKBOX = 2,
        [Description("Nhóm nút chọn")]
        RADIOBUTTON = 3
    }

    public enum StatusCallEnum
    {
        Success = 1,
        Fail = 2,
        Wrong = 3,
        Recall = 5,
        Recall_2 = 1002,
        Potential = 1003
    }

    public enum NotifyType
    {
        Success,
        Error
    }

    public enum Role
    {
        Administrator = 1,
        Manager = 2,
        User = 3
    }

    // sondt-2019
    public enum TypeCustomerField
    {
        TEXTBOX = 1,
        CHECKBOX = 2,
        RADIOBUTTON = 3,
        DROPDOWNLIST = 4
    }
}