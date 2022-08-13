using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class RequiredAttribute : ValidationAttribute, IClientValidatable
    {
        public RequiredAttribute()
            : base()
        {
            ErrorMessage = ResxValidationMessages.Resources.InvalidRequired;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString().Trim()))
            {
                return
                    new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ValidationType = "required",
                ErrorMessage = FormatErrorMessage(metadata.GetDisplayName())
            };
            return new List<ModelClientValidationRule> { rule };
        }
    }
}
