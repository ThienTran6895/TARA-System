using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class EqualValueAttribute : ValidationBase,IClientValidatable
    {
        public EqualValueAttribute()
        {
            this.ErrorMessage = ResxValidationMessages.Resources.EqualValue;
        }

        public string EqualValue { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string errorMessage = string.Format(FormatErrorMessage(ErrorMessageString), validationContext.DisplayName,
                    this.EqualValue);
                if (!string.IsNullOrEmpty(value.ToString()) && value.ToString() != this.EqualValue)
                {
                    return new ValidationResult(errorMessage, new[]
                    {
                        validationContext.MemberName
                    });
                }
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {
            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = "equalvalue",
                ErrorMessage = string.Format(FormatErrorMessage(ErrorMessageString), metadata.DisplayName,
                    this.EqualValue)
            };

            modelClientValidationRule.ValidationParameters.Add("equalvalue",EqualValue);

            yield return modelClientValidationRule;
        }
    }
}
