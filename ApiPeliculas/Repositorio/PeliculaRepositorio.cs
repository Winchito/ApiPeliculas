using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Repositorio
{
    public class PeliculaRepositorio: IPeliculaRepositorio
    {
        private readonly AppContextDB _db;

        public PeliculaRepositorio(AppContextDB db)
        {
            _db = db;
        }

        public bool ActualizarPelicula(Pelicula pelicula)
        {
            pelicula.CreationDate = DateTime.Now;
            var peliculaExistente = _db.Peliculas.Find(pelicula.Id);
            if (peliculaExistente != null)
            {
                _db.Entry(peliculaExistente).CurrentValues.SetValues(pelicula);
            }
            else
            {
                _db.Peliculas.Update(pelicula);
            }
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula pelicula)
        {
            _db.Remove(pelicula);
            return Guardar();
        }

        public IEnumerable<Pelicula> BuscarPelicula(string nombre)
        {
            IQueryable<Pelicula> query = _db.Peliculas;

            if (!string.IsNullOrEmpty(nombre))
            {
                query = query.Where(e => e.Name.Contains(nombre) || e.Description.Contains(nombre));
            }
            return query.ToList();
        }

        public bool CrearPelicula(Pelicula pelicula)
        {
            pelicula.CreationDate = DateTime.Now;
            _db.Peliculas.Add(pelicula);
            return Guardar();
        }

        public bool ExistePelicula(int peliculaId) => _db.Peliculas.Any(c => c.Id == peliculaId);

        public bool ExistePelicula(string nombre) => _db.Peliculas.Any(c => c.Name.ToLower().Trim() == nombre.ToLower().Trim());

        public Pelicula GetPelicula(int peliculaId) => _db.Peliculas.FirstOrDefault(c => c.Id == peliculaId);
    
        public ICollection<Pelicula> GetPeliculas() => _db.Peliculas.OrderBy(c => c.Name).ToList();

        public ICollection<Pelicula> GetPeliculasEnCategoria(int catId) => _db.Peliculas.Include(ca => ca.Categoria).Where(ca => ca.categoriaId == catId).ToList();

        public bool Guardar() => _db.SaveChanges() >= 0 ? true : false;

    }
}
