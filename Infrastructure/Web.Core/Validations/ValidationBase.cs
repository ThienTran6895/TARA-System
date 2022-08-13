using System.ComponentModel.DataAnnotations;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class ValidationBase : ValidationAttribute
    {
        public string ErrorMessageKey { get; set; }
        /// <summary>
        /// Get validation message from resource language
        /// </summary>
        /// <param name="name">ErrorMessageString variable</param>
        /// <returns>Validation message</returns>
        public override string FormatErrorMessage(string name)
        {
            if (!string.IsNullOrEmpty(ErrorMessageKey))
                return ResxValidationMessages.Resources.GetMember(ErrorMessageKey);
            if (!string.IsNullOrEmpty(name))
                return name;
            return ErrorMessage;
        }
    }
}
