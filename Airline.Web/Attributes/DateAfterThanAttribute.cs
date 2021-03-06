﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Attributes
{
    public class DateAfterThanAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _nomePropriedade;
        private readonly string _nomePropriedadeValidar;

        public DateAfterThanAttribute(string nomePropriedade, string nomePropriedadeValidar)
        {
            _nomePropriedade = nomePropriedade;
            _nomePropriedadeValidar = nomePropriedadeValidar;
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var property = validationContext.ObjectType.GetProperty(_nomePropriedade); // Obter a propriedade através do nome


            var beginDAte = (DateTime)property.GetValue(validationContext.ObjectInstance); // Obter o valor que está na propriedade que foi passada


            // Valor que está para validação
            var currentValue = (DateTime)value;

            if (currentValue <= beginDAte )
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
            MergeAttribute(context.Attributes, "data-val-DateLessThan", GetErrorMessage(_nomePropriedade, _nomePropriedadeValidar));
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

        protected string GetErrorMessage(string date1, string date2)
        {
            return $"The {date2} date should be after the {date1} date!";
        }
    }
}
