using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El numero máximo de caracteres es de 100")]
        public string Name { get; set; }
        public DateTime CreationDate { get; set; }


    }
}
