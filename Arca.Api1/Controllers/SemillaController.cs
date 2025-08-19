using Microsoft.AspNetCore.Mvc;
using Arca.Data.Repositories;
using Arca.Data.Models;
using System.Net.Http.Json;

namespace Arca.Api.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/semillas")]
    //[ApiController]
    public class SemillaApiController : ControllerBase
    {
        private readonly SemillaRepository _semillasRepository;
        // Recibe la instancia desde DI
        //public SemillaController(IConfiguration configuration)
        public SemillaApiController(IConfiguration configuration)
        {
            // Inicializa el repositorio con la cadena de conexión desde appsettings.json
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _semillasRepository = new SemillaRepository(connectionString);
        }
        #region Endpoints de Registro y Búsqueda

        //--------------------------------------------------------------------------------------
        /// REGISTRO DE NUEVA SEMILLA
        //--------------------------------------------------------------------------------------

        [HttpPost("registrar")]
        public IActionResult RegistrarSemilla([FromBody] Semilla semilla)
        {

            if (semilla == null) 
                return BadRequest("Datos de semilla inválidos");
            try
            {
                _semillasRepository.RegistrarSemilla(semilla);
                return Ok("Semilla registrado correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar el usuario: {ex.Message}{ex.InnerException?.Message}");
            }
        }

        //--------------------------------------------------------------------------------------
        /// FIN DE REGISTRO DE NUEVA SEMILLA
        //--------------------------------------------------------------------------------------



        //-------------------------------------------------------------------
        //PARA OBTENER TODAS LAS SEMILLAS
        //-------------------------------------------------------------------

        [HttpGet("todos")]
        public IActionResult GetAll()
        {
            try
            {
                var semillas = _semillasRepository.ObtenerTodas();
                return Ok(semillas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener semillas: {ex.InnerException?.Message}");
            }
        }
        //-------------------------------------------------------------------
        //FIN DE OBTENER TODAS LAS SEMILLAS
        //-------------------------------------------------------------------



        //--------------------------------------------------------------------------------------
        /// OBTENER SEMILLA POR ID
        //--------------------------------------------------------------------------------------

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var semilla = _semillasRepository.ObtenerPorId(id);
            if (semilla == null) return NotFound();
            return Ok(semilla);
        }




        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Semilla s)
        {
            if (s is null || s.IdSemilla != id) return BadRequest("Datos inválidos");
            var existente = _semillasRepository.ObtenerPorId(id);
            if (existente is null) return NotFound();
            _semillasRepository.Actualizar(s);
            return NoContent();
        }
        //--------------------------------------------------------------------------------------
        /// FIN DE OBTENER SEMILLA POR ID.
        //--------------------------------------------------------------------------------------


        //--------------------------------------------------------------------------------------
        /// DELETE SEMILLA POR ID
        //--------------------------------------------------------------------------------------

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existente = _semillasRepository.ObtenerPorId(id);
            if (existente is null) return NotFound();
            _semillasRepository.Eliminar(id);
            return NoContent();
        }
        //--------------------------------------------------------------------------------------
        /// FIN DELETE SEMILLA POR ID.
        //--------------------------------------------------------------------------------------
    }
}
#endregion

