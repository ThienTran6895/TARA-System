using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MB.Common;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class FormatAttribute : ValidationBase, IClientValidatable
    {

        public MBFormat MBFormat;
        readonly bool _asMasked;
        public FormatAttribute(MBFormat format, bool masked = true)
        {
            MBFormat = format;
            _asMasked = masked;
            switch (MBFormat)
            {
                case MBFormat.Date:
                    this.ErrorMessage = ResxValidationMessages.Resources.InvalidDate;
                    break;
                case MBFormat.DateTime:
                    this.ErrorMessage = ResxValidationMessages.Resources.InvalidDate;
                    break;
                case MBFormat.MMYY:
                    this.ErrorMessage = ResxValidationMessages.Resources.InvalidDate;
                    break;
                case MBFormat.Time:
                    _asMasked = false;
                    this.ErrorMessage = ResxValidationMessages.Resources.InvalidTime;
                    break;
                case MBFormat.AlphaNumeric:
                    this.ErrorMessage = ResxValidationMessages.Resources.InvalidField;
                    break;
                case MBFormat.Alpha:
                    this.ErrorMessage = ResxValidationMessages.Resources.InvalidAlphaOnly;
                    break;
                case MBFormat.Numeric:
                    this.ErrorMessage = ResxValidationMessages.Resources.InvalidNumeric;
                    break;
                case MBFormat.Phone:
                    this.ErrorMessage = ResxValidationMessages.Resources.InvalidField;
                    break;
                case MBFormat.WebsiteAddress:
                    this.ErrorMessage = ResxValidationMessages.Resources.InvalidField;
                    break;
                case MBFormat.Number:
                    this.ErrorMessage = ResxValidationMessages.Resources.InvalidField;
                    break;
                default:
                    this.ErrorMessage = ResxValidationMessages.Resources.InvalidField;
                    break;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var error = new ValidationResult(string.Format(this.ErrorMessage, validationContext.DisplayName)
                    , new[]
                    {
                        validationContext.MemberName
                    });

                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    if (MBFormat == MBFormat.Number)
                    {
                        if (!DetectNumber(value.ToString()))
                            return error;
                    }
                    else
                    {
                        var pattern = GetPattern(MBFormat);
                        if (!Regex.IsMatch(value.ToString(), pattern))
                        {
                            return error;
                        }    
                    }
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var list = new List<ModelClientValidationRule>();
            var rule = new ModelClientValidationRule
            {
                ValidationType = "MBFormat",
                ErrorMessage = string.Format(base.ErrorMessage, metadata.GetDisplayName())
            };
            rule.ValidationParameters.Add("type", MBFormat.ToString());
            rule.ValidationParameters.Add("regex", GetPattern(MBFormat));
            if (_asMasked)
            {
                rule.ValidationParameters.Add("mask", GetMask(MBFormat));
            }
            list.Add(rule);

            return list;
        }

        public string GetPattern(MBFormat MBFormat)
        {
            switch (MBFormat)
            {
                case MBFormat.Date:
                    return @"^([0]\d|[1][0-2])\/([0-2]\d|[3][0-1])\/([2][01]|[1][6-9])\d{2}(\s([0-1]\d|[2][0-3])(\:[0-5]\d){1,2})?$";
                case MBFormat.DateTime:
                    return @"^([0]\d|[1][0-2])\/([0-2]\d|[3][0-1])\/([2][01]|[1][6-9])\d{2}(\s([0-1]\d|[2][0-3])(\:[0-5]\d){1,2})?$";
                case MBFormat.MMYY: 
                    return @"((^0[1-9])|(^1[0-2]))\/[0-9]{2}$";
                case MBFormat.AlphaNumeric: 
                    return @"^[a-zA-Z0-9]+$";
                case MBFormat.Alpha: 
                    return @"^[a-zA-Z]+$";
                case MBFormat.Numeric: 
                    return @"^[0-9]+$";
                case MBFormat.Phone:
                case MBFormat.Fax: 
                    return @"^\D?(\d{3})\D?\D?(\d{3})\D?(\d{4})$";
                case MBFormat.WebsiteAddress: return @"^((http|https|ftp|www)://)?([a-zA-Z0-9]+(\.[a-zA-Z0-9]+)+.*)$";
            }
            return "";
        }

        private string GetMask(MBFormat MBFormat)
        {
            switch (MBFormat)
            {
                case MBFormat.DateTime:
                    return "00/00/0000 00:00 00";
                case MBFormat.Date:
                    return "00/00/0000";
                case MBFormat.MMYY: 
                    return "00/00";
                case MBFormat.Phone:
                case MBFormat.Fax: 
                    return "(000) 000-0000";
            }
            return string.Empty;
        }

        private bool DetectNumber(string number)
        {
            string[] arrayNumber = number.Split(new char[] { '.' });
            string leftHandPattern = @"^(0|[-]?[1-9][0-9]*)$";
            string rightHandPattern = "^[0-9]+$";
            if (arrayNumber.Length > 2)
                return false;
            if (arrayNumber.Length == 1)
            {
                return Regex.IsMatch(number, leftHandPattern);
            }
            else
            {
                if (string.IsNullOrEmpty(arrayNumber[0]))
                    return Regex.IsMatch(arrayNumber[1], rightHandPattern);
                else if (string.IsNullOrEmpty(arrayNumber[1]))
                    return false;
                else
                {
                    return Regex.IsMatch(arrayNumber[0].Replace("-", ""), leftHandPattern) &&
                           Regex.IsMatch(arrayNumber[1], rightHandPattern);
                }
            }
        }

    }

    
}
