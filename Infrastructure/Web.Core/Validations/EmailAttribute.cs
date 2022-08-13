using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class EmailAttribute : ValidationBase, IClientValidatable
    {
        public readonly string Regex = @"[\w-]+@([\w-]+\.)+[\w-]+";

        public EmailAttribute()
        {
            this.ErrorMessage = ResxValidationMessages.Resources.InvalidEmail;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                var errorMessage = string.Format(this.ErrorMessage, validationContext.DisplayName);
                try
                {
                    if (!string.IsNullOrEmpty(value.ToString()))
                    {
                        var mail = new Regex(this.Regex);
                        if (!mail.IsMatch(value.ToString()))
                        {
                            return new ValidationResult(errorMessage, new[]
                            {
                                validationContext.MemberName
                            });
                        }
                    }
                }
                catch
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
            var list = new List<ModelClientValidationRule>();
            var rule = new ModelClientValidationRule
            {
                ValidationType = "emailvalidation",
                ErrorMessage = string.Format(base.ErrorMessage, metadata.GetDisplayName())
            };

            rule.ValidationParameters.Add("regex", Regex);
            list.Add(rule);

            return list;
        }
    }
}
