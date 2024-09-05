using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Models.Dtos
{
    public class CrearCategoriaDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El numero máximo de caracteres es de 100")]
        public string Name { get; set; }
    }
}
