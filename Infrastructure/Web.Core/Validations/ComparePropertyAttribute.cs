using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MB.Common;
using MB.Common.Helpers;
using MB.Common.Resource;

namespace MB.Web.Core.Validations
{
    public class ComparePropertyAttribute : ValidationBase, IClientValidatable
    {
        public ComparePropertyAttribute()
        {
            this.ErrorMessage = ResxValidationMessages.Resources.CompareFirstFieldWithSecondField;
        }

        public string RefProperty { get; set; }
        public Operator ValidationOperator { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            double referenceValue = 0;
            bool isValid = true;
            var propertyInfo = validationContext.ObjectInstance.GetType().GetProperty(RefProperty);

            if (propertyInfo.GetValue(validationContext.ObjectInstance) != null)
            {
                referenceValue = double.Parse(propertyInfo.GetValue(validationContext.ObjectInstance).ToString());
            }

            string refDisplayName = CommonHelper.GetDisplayNameMetaData(validationContext.ObjectType, RefProperty);
            string operatorDisplayName = CommonHelper.GetDescription(ValidationOperator);

            if (value != null)
            {
                switch (ValidationOperator)
                {
                    case Operator.GreaterThan:
                        if (double.Parse(value.ToString()) <= referenceValue)
                            isValid = false;
                        break;
                    case Operator.GreaterThanOrEqual:
                        if (double.Parse(value.ToString()) < referenceValue)
                            isValid = false;
                        break;
                    case Operator.LessThan:
                        if (double.Parse(value.ToString()) >= referenceValue)
                            isValid = false;
                        break;
                    case Operator.LessThanOrEqual:
                        if (double.Parse(value.ToString()) > referenceValue)
                            isValid = false;
                        break;
                    case Operator.EqualTo:
                        if (double.Parse(value.ToString()) != referenceValue)
                            isValid = false;
                        break;

                }
                if (!isValid)
                    return new ValidationResult(string.Format(FormatErrorMessage(ErrorMessageString), validationContext.DisplayName, operatorDisplayName, refDisplayName),
                    new[] { validationContext.MemberName });
            }
            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {
            var modelClientValidationRule = new ModelClientValidationRule
            {
                ValidationType = "comparepropertyvalidaton"
            };

            string refDisplayName = CommonHelper.GetDisplayNameMetaData(metadata.ContainerType, RefProperty);
            string operatorDisplayName = CommonHelper.GetDescription(ValidationOperator);

            modelClientValidationRule.ValidationParameters.Add("refproperty", RefProperty);
            modelClientValidationRule.ValidationParameters.Add("validationoperator", ValidationOperator.ToString());

            modelClientValidationRule.ErrorMessage = String.Format(this.ErrorMessage, metadata.GetDisplayName(), operatorDisplayName, refDisplayName);

            return new List<ModelClientValidationRule> { modelClientValidationRule };
        }
    }
}
