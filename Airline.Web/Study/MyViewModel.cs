using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data
{
    public class MyViewModel
    {
        [DateLessThan("End")]
        public DateTime Begin { get; set; }

        public DateTime End { get; set; }
    }


    public class DateLessThanAttribute : ValidationAttribute, IClientModelValidator
    {
        private readonly string _nomePropriedade;


        public DateLessThanAttribute(string nomePropriedade)
        {
            _nomePropriedade = nomePropriedade;
        }


        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var model = (MyViewModel)validationContext.ObjectInstance; // Obter uma instância do objecto


            var property = validationContext.ObjectType.GetProperty(_nomePropriedade); // Obter a propriedade através do nome


            var beginDAte = (DateTime)property.GetValue(validationContext.ObjectInstance); // Obter o valor que está na propriedade que foi passada

            
            // Valor que está para validação
            var currentValue = (DateTime)value;


            if (currentValue > beginDAte)
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
            MergeAttribute(context.Attributes, "data-val-DateLessThan", GetErrorMessage());
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
            return $"The end date should be after the beginning date!.";
        }
    }

}
