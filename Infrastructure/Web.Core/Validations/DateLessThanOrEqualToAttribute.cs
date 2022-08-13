using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class DateLessThanOrEqualToAttribute : ValidationBase, IClientValidatable
    {
        private DateTime _comparedDate = DateTime.Today;
        public DateLessThanOrEqualToAttribute()//DateTime? date = null)
        {
            ErrorMessage = ResxValidationMessages.Resources.InvalidField;   
            //if (date.HasValue)
            //    _comparedDate = date.Value;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;
          
            var dateEntered = (DateTime)value;
            if (dateEntered > _comparedDate)
            {
                var message = FormatErrorMessage(validationContext.DisplayName);
                return new ValidationResult(message, new[]
                {
                    validationContext.MemberName
                });
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {
            var rule = new ModelClientValidationRule
            {
                ErrorMessage = FormatErrorMessage(metadata.DisplayName),
                ValidationType = "datemustbeequalorpriorthancurrentdate"
            };

            yield return rule;
        }
    }
}
