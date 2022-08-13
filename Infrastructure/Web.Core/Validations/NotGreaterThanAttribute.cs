using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class NotGreaterThanAttribute: ValidationBase, IClientValidatable
    {
        public int RefValue { get; set; }

        public NotGreaterThanAttribute()
        {
            ErrorMessage = ResxValidationMessages.Resources.InvalidField;            
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value!=null && (int)value > RefValue)
            {
                return new ValidationResult(String.Format(FormatErrorMessage(ErrorMessageString), validationContext.DisplayName), new[]
                {
                    validationContext.MemberName
                });
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {

            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = "notgreaterthan",
                ErrorMessage = String.Format(FormatErrorMessage(ErrorMessageString), metadata.GetDisplayName())
            };

            modelClientValidationRule.ValidationParameters.Add("refvalue", RefValue);

            return new List<ModelClientValidationRule> { modelClientValidationRule };
        }
    }
}
