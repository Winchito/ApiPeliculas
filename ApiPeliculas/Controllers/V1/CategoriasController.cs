using ApiPeliculas.Models;
using ApiPeliculas.Models.Dtos;
using ApiPeliculas.Repositorio.IRepositorio;
using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiPeliculas.Controllers.V1
{
    // [Route("api/[controller]")] //Opcion estatica
    //[Authorize(Roles = "Admin")] //Autorizacion a nivel de clase
    //[ResponseCache(Duration = 20)] //Cache
    [Route("api/v{version:apiVersion}/categorias")] //Opcion dinamica
    [ApiController]
    // [EnableCors("PoliticaCors")] //Aplica la politica CORS al controlador entero
    [ApiVersion("1.0")]
    //[ApiVersion("1.0", Deprecated = true)] //esto quiere decir api obsoleta
    //[Obsolete("Esta version del controlador está obsoleta")] //esto quiere decir api obsoleta
    //[ApiVersion("2.0")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaRepositorio _ctRepo;
        private readonly IMapper _mapper;

        public CategoriasController(ICategoriaRepositorio ctRepo, IMapper mapper)
        {
            _ctRepo = ctRepo;
            _mapper = mapper;
        }

        [AllowAnonymous] //Permite el uso del metodo sin autorizacion
        [HttpGet]
        //[MapToApiVersion("1.0")] 
        //[ResponseCache(Duration = 20)] //Como se deben almacenar en cache las respuestas para reducir la carga del servidor y que no se tenga que procesar la solicitud nuevamente 
        [ResponseCache(CacheProfileName = "PorDefecto30Segundos")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // [EnableCors("PoliticaCors")] //Aplica la politica CORS a este metodo
        public IActionResult GetCategorias()
        {
            var listaCategorias = _ctRepo.GetCategorias();

            var listaCategoriasDto = new List<CategoriaDto>();

            foreach (var lista in listaCategorias)
            {
                listaCategoriasDto.Add(_mapper.Map<CategoriaDto>(lista));
            }

            return Ok(listaCategoriasDto);
        }

        [AllowAnonymous]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("{categoriaId:int}", Name = "GetCategoria")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCategoria(int categoriaId)
        {
            var itemCategoria = _ctRepo.GetCategoria(categoriaId);

            if (itemCategoria == null)
            {
                return NotFound();
            }

            var imtemCategoriaDto = _mapper.Map<CategoriaDto>(itemCategoria);

            return Ok(imtemCategoriaDto);

        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CrearGategoria([FromBody] CrearCategoriaDto crearCategoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (crearCategoriaDto == null)
            {
                return BadRequest(ModelState);
            }

            if (_ctRepo.ExisteCategoria(crearCategoriaDto.Name))
            {
                ModelState.AddModelError("", $"La categoria ya existe");
                return StatusCode(404, ModelState);
            }

            var categoria = _mapper.Map<Categoria>(crearCategoriaDto);

            if (!_ctRepo.CrearCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal guardando el registro{categoria.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategoria", new { categoriaId = categoria.Id }, categoria);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{categoriaId:int}", Name = "ActualizarPatchCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPatchCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (categoriaDto == null || categoriaId != categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro{categoria.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{categoriaId:int}", Name = "ActualizarPutCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ActualizarPutCategoria(int categoriaId, [FromBody] CategoriaDto categoriaDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (categoriaDto == null || categoriaId != categoriaDto.Id)
            {
                return BadRequest(ModelState);
            }

            var categoriaExistente = _ctRepo.GetCategoria(categoriaId);
            if (categoriaExistente == null)
            {
                return NotFound($"No se encontro la categoria con ID {categoriaId}");
            }

            var categoria = _mapper.Map<Categoria>(categoriaDto);

            if (!_ctRepo.ActualizarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal actualizando el registro{categoria.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{categoriaId:int}", Name = "BorrarCategoria")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BorrarCategoria(int categoriaId)
        {

            if (!_ctRepo.ExisteCategoria(categoriaId))
            {
                return NotFound($"No se encontro la categoria con ID {categoriaId}");
            }

            var categoria = _ctRepo.GetCategoria(categoriaId);

            if (!_ctRepo.BorrarCategoria(categoria))
            {
                ModelState.AddModelError("", $"Algo salio mal eliminando el registro{categoria.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }
    }
}

