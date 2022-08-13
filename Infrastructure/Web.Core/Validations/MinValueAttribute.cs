using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MB.Common;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class MinValueAttribute : ValidationBase, IClientValidatable
    {
        public MinValueAttribute()
        {
            this.ErrorMessage = ResxValidationMessages.Resources.InvalidMinValue;
        }
        public string MinValue { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            if (value != null && value.ToString() != string.Empty)
            {
                int tmp;
                string mess = int.TryParse(MinValue, out tmp) ?
                    String.Format(this.ErrorMessage, validationContext.DisplayName, MinValue.ToInt() - 1) :
                    String.Format(this.ErrorMessage, validationContext.DisplayName, Math.Truncate(decimal.Parse(MinValue)));
                try
                {
                    if (decimal.Parse(value.ToString()) < decimal.Parse(MinValue))
                    {
                        return new ValidationResult(mess, new[] { validationContext.MemberName });
                    }
                }
                catch
                {
                    return new ValidationResult(mess, new[] { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {
            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = "minvalue"
            };
            int tmp;
            string min = string.Empty;
            if (int.TryParse(MinValue, out tmp))
                min = (MinValue.ToInt() - 1).ToString();
            else
                min = Math.Truncate(decimal.Parse(MinValue)).ToString();
            //min = Math.Round(decimal.Parse(MinValue), 0).ToString();
            modelClientValidationRule.ValidationParameters.Add("value", MinValue);
            modelClientValidationRule.ErrorMessage = String.Format(this.ErrorMessage,
                metadata.GetDisplayName(), min);

            return new List<ModelClientValidationRule> { modelClientValidationRule };
        }
    }
}
