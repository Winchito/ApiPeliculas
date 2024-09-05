using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }
        [Required]
        // [Display(Name = "Fecha de creación")] 
        public DateTime  CreationDate { get; set; }


    }
}
