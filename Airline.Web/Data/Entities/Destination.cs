namespace Airline.Web.Data.Entities
{
    using System.ComponentModel.DataAnnotations;


    public class Destination : IEntity
    {
        // No .net core não é necessário definir como key --> Quando o entity framework for mapear vai assumir esta propriedade como chave primária identity
        public int Id { get; set; }

        [Display(Name = "Country")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} character long.")]
        [Required(ErrorMessage = "The field {0} is required")]
        public string Country { get; set; }


        [Display(Name = "City")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} character long.")]
        [Required(ErrorMessage = "The field {0} is required")]
        public string City { get; set; }

        
        //Se a cidade só tiver este aeroporto o campo não é necessário (Esta validação será realizada do lado do cliente)
        [Display(Name = "Airport")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} character long.")]
        public string Airport { get; set; }


        //Aceitar apenas 3 Letras
        [Display(Name = "IATA")]
        [Required(ErrorMessage ="The filed {0} is required")]
        [StringLength(3, ErrorMessage = "The field {0} need to contain {1} characters long", MinimumLength = 3)]
        public string IATA { get; set; }


        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        // Relação de um para muitos (um user pode criar muitos destinos) - Outra diferença em relação ao .Net Framework ( não preciso das propriedades virtuais)
        public User User { get; set; }




        // Propriedade para aparecer o link completo para a imagem (Se não existir imagem retornar null) - Para ser consumido no mobile
        // Se existir imagem, ao link do Azure adicionar o caminho especifico para a imagem (ImagemUrl sem o ~)
        public string FullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return null;
                }

                return $"https://airlinewebdulce.azurewebsites.net/{this.ImageUrl.Substring(1)}"; //Substring (1) estou a retirar o ~ do caminho
            }
        }
    }   
}
