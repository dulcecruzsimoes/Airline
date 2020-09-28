using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Attributes
{
    public class RequieredIfRoundTripAttribute : ValidationAttribute, IClientModelValidator
    {

        private readonly string _nomePropriedade;
        private readonly string _nomePropriedadeValidar;


        public RequieredIfRoundTripAttribute(string nomePropriedade, string nomePropriedadeValidar)
        {
            _nomePropriedade = nomePropriedade;
            _nomePropriedadeValidar = nomePropriedadeValidar;
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var property = validationContext.ObjectType.GetProperty(_nomePropriedade); // Obter a propriedade através do nome


            var trip = (int)property.GetValue(validationContext.ObjectInstance); // Obter o valor que está na propriedade que foi passada


            // Valor que está para validação
            var returnDate = value;

            if (trip == 1 && returnDate == null)
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
            MergeAttribute(context.Attributes, "data-val-RequiredIfRoundTrip", GetErrorMessage());
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
            return $"The field is required";
        }
    }
}
