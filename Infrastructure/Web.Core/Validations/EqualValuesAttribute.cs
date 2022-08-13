using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class EqualValuesAttribute : ValidationBase, IClientValidatable
    {
        public EqualValuesAttribute()
        {
            this.ErrorMessage = ResxValidationMessages.Resources.EqualValue;
        }

        public string RefValue { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string[] lstValues = RefValue.Split(new [] {'|'}, StringSplitOptions.RemoveEmptyEntries);
            if (value!=null && lstValues.All(x => x != value.ToString()))
            {
                return new ValidationResult(this.ErrorMessage,new []
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
                ValidationType = "equalvalues",
                ErrorMessage = this.ErrorMessage
            };

            modelClientValidationRule.ValidationParameters.Add("refvalue", RefValue);

            return new List<ModelClientValidationRule> { modelClientValidationRule };
        }
    }
}
