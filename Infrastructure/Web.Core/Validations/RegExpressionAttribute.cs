using System.ComponentModel.DataAnnotations;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class RegExpressionAttribute : RegularExpressionAttribute
    {
        public RegExpressionAttribute(string pattern)
            : base(pattern)
        {
            ErrorMessage = ResxValidationMessages.Resources.InvalidField;
        }
    }
}
