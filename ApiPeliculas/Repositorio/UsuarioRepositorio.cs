using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiPeliculas.Data;
using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using XSystem.Security.Cryptography;

namespace ApiPeliculas.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly AppContextDB _db;
        private string claveSecreta;
        private readonly UserManager<AppUsuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        public UsuarioRepositorio(AppContextDB db, IConfiguration config, UserManager<AppUsuario> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _db = db;
            claveSecreta = config.GetValue<string>("ApiSettings:Secreta");
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public AppUsuario GetUsuario(string usuarioId) => _db.AppUsuarios.FirstOrDefault(c => c.Id == usuarioId);

        public ICollection<AppUsuario> GetUsuarios() => _db.AppUsuarios.OrderBy(c => c.UserName).ToList();

        public bool IsUniqueUser(string usuario)
        {
            var usuarioBd = _db.AppUsuarios.FirstOrDefault(u => u.UserName == usuario);
            if (usuarioBd == null)
            {
                return true;
            }
            return false;
        }
        public async Task<UsuarioLoginRespuestaDto> Login(UsuarioLoginDto usuarioLoginDto)
        {
           // var passwordEncriptado = obtenermd5(usuarioLoginDto.Password);
            var usuario = _db.AppUsuarios.FirstOrDefault(u =>
            u.UserName.ToLower() == usuarioLoginDto.NombreUsuario.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(usuario, usuarioLoginDto.Password);

            //Validamos si el ususario no existe con la combinación de usuario o contraseña correcta
            if (usuario == null || isValid == false)
            {
                return new UsuarioLoginRespuestaDto()
                {
                    Token = "",
                    Usuario = null
                };
            }
            //Aqui existe el usuario entonces podemos procesar el login 
            var roles = await _userManager.GetRolesAsync(usuario);
            var manejadoToken = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(claveSecreta);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.UserName.ToString()),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = manejadoToken.CreateToken(tokenDescriptor);

            UsuarioLoginRespuestaDto usuarioLoginRespuestaDto = new UsuarioLoginRespuestaDto()
            {
                Token = manejadoToken.WriteToken(token),
                Usuario = _mapper.Map<UsuarioDatosDto>(usuario),
            };

            return usuarioLoginRespuestaDto;
        }

        public async Task<UsuarioDatosDto> Registro(UsuarioRegistroDto usuarioRegistroDto)
        {
            //var passwordEncriptada = obtenermd5(usuarioRegistroDto.Password);

            /*
            Usuario usuario = new Usuario()
            {
                NombreUsuario = usuarioRegistroDto.NombreUsuario,
                Password = passwordEncriptada,
                Nombre = usuarioRegistroDto.Nombre,
                Role = usuarioRegistroDto.Role
            };
            */
            AppUsuario usuario = new AppUsuario()
            {
                UserName = usuarioRegistroDto.NombreUsuario,
                Email = usuarioRegistroDto.NombreUsuario,
                NormalizedEmail = usuarioRegistroDto.NombreUsuario.ToUpper(),
                Nombre = usuarioRegistroDto.Nombre
            };

            var result = await _userManager.CreateAsync(usuario, usuarioRegistroDto.Password);
            if(result.Succeeded)
            {
                if(!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("Registrado"));
                }

                await _userManager.AddToRoleAsync(usuario, "Admin");
                var usuarioRetornado = _db.AppUsuarios.FirstOrDefault(u => u.UserName == usuarioRegistroDto.NombreUsuario);
                return _mapper.Map<UsuarioDatosDto>(usuarioRetornado);
            }

            //_db.Usuarios.Add(usuario);
            //await _db.SaveChangesAsync();
            //usuario.Password = passwordEncriptada;
            //return usuario;

            return new UsuarioDatosDto();
        }

        //Metodo para encriptar contraseña con MD5 que se usa tanto para el acceso como en el registro
        //public static string obtenermd5(string valor)
        //{
        //    MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
        //    byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
        //    data = x.ComputeHash(data);
        //    string resp = "";
        //    for (int i = 0; i < data.Length; i++)
        //    {
        //        resp += data[i].ToString("x2").ToLower();
        //    }
        //    return resp;
        //}
    }
}