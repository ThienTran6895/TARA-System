using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class MinLengthAttribute: ValidationBase, IClientValidatable
    {
        public MinLengthAttribute()
        {
            this.ErrorMessage = ResxValidationMessages.Resources.InvalidMinLength;
        }

        public MinLengthAttribute(int minLength): this()
        {
            Length = minLength;
        }

        public int Length { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value!= null && value.ToString().Length<this.Length)
            {
                return new ValidationResult(string.Format(FormatErrorMessage(ErrorMessageString), validationContext.DisplayName, this.Length), new[]
                {
                    validationContext.MemberName
                });
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule()
            {
                ValidationType = "minlength",
                ErrorMessage = string.Format(FormatErrorMessage(ErrorMessageString), metadata.GetDisplayName(), this.Length)
            };

            rule.ValidationParameters.Add("value", this.Length);

            return new List<ModelClientValidationRule> { rule };
        }
    }
}
