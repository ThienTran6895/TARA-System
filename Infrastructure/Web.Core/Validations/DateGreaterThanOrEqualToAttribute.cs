using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class DateGreaterThanOrEqualToAttribute : ValidationBase, IClientValidatable
    {
        private DateTime _comparedDate = DateTime.Today;
        public DateGreaterThanOrEqualToAttribute()//DateTime? date = null)
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
            if (dateEntered < _comparedDate) 
            {
                var error = new ValidationResult(FormatErrorMessage(ErrorMessageString),
                new List<string>() { validationContext.MemberName });

                return error;                
            }
            return ValidationResult.Success; 
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            var rule = new ModelClientValidationRule();
            rule.ErrorMessage = FormatErrorMessage(ErrorMessageString);
            rule.ValidationType = "datemustbeequalorgreaterthancurrentdate";
            yield return rule;
        }
    }
}