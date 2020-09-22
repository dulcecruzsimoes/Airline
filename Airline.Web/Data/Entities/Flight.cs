using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Airline.Web.Data.Entities
{
    public class Flight : IEntity
    {
        // Não é preciso dizer que é chave primária
        public int Id { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        public Destination From { get; set; }

        // Campo obrigatório
        [Required(ErrorMessage = "The field {0} is required")]
        public Destination To { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Departure { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Arrival { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        public Airplane Airplane { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        public Status Status { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        public int Economic  // Lugares da classe económica
        {
            get; set;
        }

        [Required(ErrorMessage = "The field {0} is required")]
        public int Business // Lugares da classe executiva
        {
            get; set;
        }

        // Criar uma tabela de detalhe para apresentar a lista de bilhetes
        // A lista de bilhetes terá o voo, o user e a classe

        // Mais prático para fazer as querys
        public IEnumerable<Ticket> Tickets { get; set; }

    }
}
