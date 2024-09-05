using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.V2
{
    // [Route("api/[controller]")] //Opcion estatica
    //[Authorize(Roles = "Admin")] //Autorizacion a nivel de clase
    //[ResponseCache(Duration = 20)] //Cache
    [Route("api/v{version:apiVersion}/categorias")] //Opcion dinamica
    [ApiController]
    // [EnableCors("PoliticaCors")] //Aplica la politica CORS al controlador entero
    [ApiVersion("2.0")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _ctRepo;
        private readonly IMapper _mapper;

        public CategoriasController(ICategoriaRepositorio ctRepo, IMapper mapper)
        {
            _ctRepo = ctRepo;
            _mapper = mapper;
        }

        [HttpGet("GetString")]
        [MapToApiVersion("2.0")]
        public IEnumerable<string> Get()
        {
            return new string[] { "valor1", "valor2", "valor3" };
        }


    }
}

