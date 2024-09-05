

namespace ApiPeliculas.Models.Dtos
{
    public class PeliculaDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Lenght { get; set; }
        public string? ImageRoute { get; set; }
        public string? RutaLocalImagen { get; set; }
        public enum TipoClasificacion { Seven, Thirteen, Sixteen, Eighteen }
        public TipoClasificacion Rate { get; set; }
        public DateTime CreationDate { get; set; }

        //Relacion con Categoria
        public int categoriaId { get; set; }

    }
}
