using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class UrlAttribute : ValidationBase, IClientValidatable
    {
        private const string Regex = @"^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$";

        public UrlAttribute()
        {
            this.ErrorMessage = ResxValidationMessages.Resources.InvalidUrl;
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
                        var mail = new Regex(Regex);
                        if (!mail.IsMatch(value.ToString().ToLower()))
                        {
                            return new ValidationResult(errorMessage, new[] { validationContext.MemberName });
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
                ValidationType = "urlvalidation",
                ErrorMessage = string.Format(base.ErrorMessage, metadata.GetDisplayName())
            };

            rule.ValidationParameters.Add("regex", Regex);
            list.Add(rule);

            return list;
        }
    }
}