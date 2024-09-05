

namespace ApiPeliculas.Models.Dtos
{
    public class CrearPeliculaDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Lenght { get; set; }
        public string? ImageRoute { get; set; }
        public IFormFile Imagen { get; set; }
        public enum CrearTipoClasificacion { Seven, Thirteen, Sixteen, Eighteen }
        public CrearTipoClasificacion Rate { get; set; }
        public int categoriaId { get; set; }

    }
}
