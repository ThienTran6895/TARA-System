using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class GreaterThanAttribute : ValidationBase, IClientValidatable
    {
        public GreaterThanAttribute()
        {
            this.ErrorMessage = ResxValidationMessages.Resources.InvalidGreaterThan;
        }
        public string MinValue { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string mess = String.Format(this.ErrorMessage, validationContext.DisplayName, MinValue);
                try
                {
                    if (decimal.Parse(value.ToString()) <= decimal.Parse(MinValue))
                    {
                        return new ValidationResult(mess, new List<string>() { validationContext.MemberName });
                    }
                }
                catch
                {
                    return new ValidationResult(mess, new List<string>() { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            ModelClientValidationRule modelClientValidationRule = new ModelClientValidationRule()
            {
                ValidationType = "greaterthanvalue"
            };
            modelClientValidationRule.ValidationParameters.Add("value", MinValue);
            modelClientValidationRule.ErrorMessage = String.Format(this.ErrorMessage, metadata.GetDisplayName(), MinValue);
            return new List<ModelClientValidationRule> { modelClientValidationRule };
        }
    }
}
