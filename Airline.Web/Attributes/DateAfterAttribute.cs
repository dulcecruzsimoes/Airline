using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Airline.Web.Models;


namespace Airline.Web.Attributes
{
    public class DateAfterAttribute : ValidationAttribute, IClientModelValidator
{


        private readonly string _comparisonProperty;

        public DateAfterAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);

            if (property == null)
                throw new ArgumentException("Property with this name not found");

            var comparisonValue = (DateTime)property.GetValue(validationContext.ObjectInstance);

            //if (currentValue > comparisonValue)
            //    return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;

        }

        public void AddValidation(ClientModelValidationContext context)
        {
            var error = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-error", error);
        }
    }
}
