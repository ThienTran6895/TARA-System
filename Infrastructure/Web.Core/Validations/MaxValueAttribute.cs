using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MB.Common;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class MaxValueAttribute : ValidationAttribute, IClientValidatable
    {
        public MaxValueAttribute()
        {
            this.ErrorMessage = ResxValidationMessages.Resources.InvalidMaxValue;
        }

        public MaxValueAttribute(string maxValue)
            : this()
        {
            MaxValue = maxValue;
        }

        public string MaxValue { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && value.ToString() != string.Empty)
            {
                int tmp;
                string mess = int.TryParse(MaxValue, out tmp) ?
                    String.Format(this.ErrorMessage, validationContext.DisplayName, MaxValue + 1) :
                    String.Format(this.ErrorMessage, validationContext.DisplayName, Math.Round(decimal.Parse(MaxValue), 0).ToString());
                try
                {
                    if (decimal.Parse(value.ToString()) > decimal.Parse(MaxValue))
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
                ValidationType = "maxvalue"
            };

            int tmp;
            string max = string.Empty;
            if (int.TryParse(MaxValue, out tmp))
                max = (MaxValue.ToInt() + 1).ToString();
            else
                max = Math.Round(decimal.Parse(MaxValue), 0).ToString();
            modelClientValidationRule.ValidationParameters.Add("value", MaxValue);
            modelClientValidationRule.ErrorMessage = String.Format(this.ErrorMessage,
                metadata.GetDisplayName(), max);
            return new List<ModelClientValidationRule> { modelClientValidationRule };
        }
    }
}