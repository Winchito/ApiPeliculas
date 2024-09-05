using ApiPeliculas.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Data
{
    public class AppContextDB: IdentityDbContext<AppUsuario>
    {
        public AppContextDB(DbContextOptions<AppContextDB> options): base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        //Aqui pasar todas las entidades (Modelos)

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<AppUsuario> AppUsuarios { get; set; }
    }
}
