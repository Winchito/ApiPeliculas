using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Repositorio.IRepositorio;

namespace ApiPeliculas.Repositorio
{
    public class CategoriaRepositorio: ICategoriaRepositorio
    {
        private readonly AppContextDB _db;

        public CategoriaRepositorio(AppContextDB db)
        {
            _db = db;
        }

        public bool ActualizarCategoria(Categoria categoria)
        {
            categoria.CreationDate = DateTime.Now;
            var categoriaExistente = _db.Categorias.Find(categoria.Id);
            if (categoriaExistente != null)
            {
                _db.Entry(categoriaExistente).CurrentValues.SetValues(categoria);
            }
            else
            {
                _db.Categorias.Update(categoria);
            }
            
            return Guardar();
        }

        public bool BorrarCategoria(Categoria categoria)
        {
            _db.Remove(categoria);
            return Guardar();
        }

        public bool CrearCategoria(Categoria categoria)
        {
            categoria.CreationDate = DateTime.Now;
            _db.Categorias.Add(categoria);
            return Guardar();
        }

        public bool ExisteCategoria(int CategoriaId) => _db.Categorias.Any(c => c.Id == CategoriaId);

        public bool ExisteCategoria(string nombre) => _db.Categorias.Any(c => c.Name.ToLower().Trim() == nombre.ToLower().Trim());

        public Categoria GetCategoria(int CategoriaId) => _db.Categorias.FirstOrDefault(c => c.Id == CategoriaId);
    
        public ICollection<Categoria> GetCategorias() => _db.Categorias.OrderBy(c => c.Name).ToList();

        public bool Guardar() => _db.SaveChanges() >= 0 ? true : false;

    }
}
