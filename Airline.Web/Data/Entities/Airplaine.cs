namespace Airline.Web.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Airplaine : IEntity
    {
        // No .net core não é necessário definir como key
        public int Id { get; set; }


        [Display(Name = "Brand")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} character long.")]
        [Required(ErrorMessage = "The field {0} is required")]
        public string Brand { get; set; }


        [Display(Name = "Model")]
        [MaxLength(50, ErrorMessage = "The field {0} only can contain {1} character long.")]
        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.Text)]
        public string Model { get; set; }


        [Display(Name = "Economy Seats")]
        [Required(ErrorMessage = "The field {0} is required")]
        [Range(0, 100, ErrorMessage = "The number of seats should be between {1} and {2}")]
        public int EconomySeats { get; set; }


        [Display(Name = "Business Seats")]
        [Required(ErrorMessage = "The field {0} is required")]
        [Range(0, 100, ErrorMessage = "The number of seats should be between {1} and {2}")]
        public int BusinessSeats { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        // Relação de um para muitos (um user pode criar muitos aviões) - Outra diferença em relação ao .Net Framework ( não preciso das propriedades virtuais)
        public User User { get; set; }

        //// Criar uma propriedade para obter o link da imagem na api:
        //public string ImageFullPath 
        //{

        //    get 
        //    {
        //        if (string.IsNullOrEmpty(ImageUrl))
        //        {
        //            return null;
        //        }

        //        return $"https://airlinewebdulce.azurewebsites.net/{this.ImageUrl.Substring(1)}"; //Substring (1) estou a retirar o ~ do caminho
        //    }
        //}
    }
}
