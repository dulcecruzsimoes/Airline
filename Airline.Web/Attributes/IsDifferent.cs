using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Attributes
{
    public class IsDifferent : ValidationAttribute, IClientModelValidator
    {
        private readonly string _nomePropriedade;
        private readonly string _nomePropriedadeValidar;


        public IsDifferent(string nomePropriedade, string nomePropriedadeValidar)
        {
            _nomePropriedade = nomePropriedade;
            _nomePropriedadeValidar = nomePropriedadeValidar;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var property = validationContext.ObjectType.GetProperty(_nomePropriedade); // Obter a propriedade através do nome


            var propertyId = (int)property.GetValue(validationContext.ObjectInstance); // Obter o valor que está na propriedade que foi passada


            // Valor que está para validação
            var currentValue = (int)value;


            if (propertyId == currentValue)
                return new ValidationResult(GetErrorMessage(_nomePropriedade, _nomePropriedadeValidar));

            return ValidationResult.Success;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }


            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-IsDifferent", GetErrorMessage(_nomePropriedade, _nomePropriedadeValidar));
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

        protected string GetErrorMessage(string destination1, string destination2)
        {
            return $"The {destination1} and {destination2} locations must be different!";
        }
    }
}
