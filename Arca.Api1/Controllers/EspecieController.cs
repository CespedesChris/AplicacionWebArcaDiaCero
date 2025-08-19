using Microsoft.AspNetCore.Mvc;
using Arca.Data.Repositories;
using Arca.Data.Models;
using System.Net.Http.Json;

namespace Arca.Api.Controllers
{
    [Route("api/[controller]")]
    //[Route("api/semillas")]
    //[ApiController]
    public class EspecieApiController : ControllerBase
    {
        private readonly EspecieRepository _especiesRepository;
        // Recibe la instancia desde DI
        public EspecieApiController(IConfiguration configuration)
        {
            // Inicializa el repositorio con la cadena de conexión desde appsettings.json
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _especiesRepository = new EspecieRepository(connectionString);
        }
        #region Endpoints de Registro y Búsqueda

        //--------------------------------------------------------------------------------------
        /// REGISTRO DE NUEVA ESPECIE
        //--------------------------------------------------------------------------------------

        [HttpPost("registrar")]
        public IActionResult RegistrarEspecie([FromBody] Especie especie)
        {

            if (especie == null)
                return BadRequest("Datos de Especie inválidos");
            try
            {
                _especiesRepository.RegistrarEspecie(especie);
                return Ok("Especie registrada correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar la especie: {ex.Message}{ex.InnerException?.Message}");
            }
        }

        //--------------------------------------------------------------------------------------
        /// FIN DE REGISTRO DE NUEVA ESPECIE
        //--------------------------------------------------------------------------------------

        //-------------------------------------------------------------------
        //PARA OBTENER TODAS LAS ESPECIES
        //-------------------------------------------------------------------

        [HttpGet("todos")]
        public IActionResult GetAll()
        {
            try
            {
                var especies = _especiesRepository.ObtenerTodas();
                return Ok(especies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener especies: {ex.InnerException?.Message}");
            }
        }
        //-------------------------------------------------------------------
        //FIN DE OBTENER TODAS LAS ESPECIES
        //-------------------------------------------------------------------

        //--------------------------------------------------------------------------------------
        /// OBTENER ESPECIE POR ID
        //--------------------------------------------------------------------------------------

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var especie = _especiesRepository.ObtenerPorId(id);
            if (especie == null) return NotFound();
            return Ok(especie);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Especie e)
        {
            if (e is null || e.IdEspecie != id) return BadRequest("Datos inválidos");
            var existente = _especiesRepository.ObtenerPorId(id);
            if (existente is null) return NotFound();
            _especiesRepository.Actualizar(e);
            return NoContent();
        }
        //--------------------------------------------------------------------------------------
        /// FIN DE OBTENER ESPECIES POR ID.
        //--------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------
        /// DELETE ESPECIE POR ID
        //--------------------------------------------------------------------------------------

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existente = _especiesRepository.ObtenerPorId(id);
            if (existente is null) return NotFound();

            bool eliminado = _especiesRepository.Eliminar(id);
            if (!eliminado)
            {
                return BadRequest("No se puede eliminar la especie porque tiene semillas asociadas.");
            }
            return Ok("Especie eliminada correctamente.");

        }
        //--------------------------------------------------------------------------------------
        /// FIN DELETE ESPECIE POR ID.
        //--------------------------------------------------------------------------------------
    }
}
#endregion
