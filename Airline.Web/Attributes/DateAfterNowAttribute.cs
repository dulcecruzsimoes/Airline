using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Attributes
{
    public class DateAfterNowAttribute : ValidationAttribute, IClientModelValidator
    {
        public DateAfterNowAttribute()
        {

        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            // Valor que está para validação
            var currentValue = (DateTime)value;


            if (currentValue < DateTime.Now)
                return new ValidationResult(GetErrorMessage());

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }


            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-DateAfterNow", GetErrorMessage());
        }


        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }

            attributes.Add(key, value);
            return true;
        }

        protected string GetErrorMessage()
        {
            return $"Please, select a date bigger than now!";
        }
    }
}
