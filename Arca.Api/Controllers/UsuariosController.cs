using Microsoft.AspNetCore.Mvc;
using Arca.Data.Repositories;
using Arca.Data.Models;
using System.Net.Http.Json;

namespace Arca.Api.Controllers
{
    [Route("api/[controller]")]
    //[Route("api/semillas")]
    //[ApiController]
    public class UsuariosApiController : ControllerBase
    {
        private readonly UsuariosRepository _usuariosRepository;
        // Recibe la instancia desde DI
        public UsuariosApiController(IConfiguration configuration)
        {
            // Inicializa el repositorio con la cadena de conexión desde appsettings.json
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _usuariosRepository = new UsuariosRepository(connectionString);
        }
        #region Endpoints de Registro y Búsqueda

        //--------------------------------------------------------------------------------------
        /// REGISTRO DE NUEVA ESPECIE
        //--------------------------------------------------------------------------------------

        [HttpPost("registrar")]
        public IActionResult RegistrarUsuarios([FromBody] Usuarios usuarios)
        {

            if (usuarios == null)
                return BadRequest("Datos de Usuario inválidos");
            try
            {
                _usuariosRepository.RegistrarUsuarios(usuarios);
                return Ok("Usuario registrado correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar el usuario: {ex.Message}{ex.InnerException?.Message}");
            }
        }

        //--------------------------------------------------------------------------------------
        /// FIN DE REGISTRO DE NUEVO USUARIO
        //--------------------------------------------------------------------------------------

        //-------------------------------------------------------------------
        //PARA OBTENER TODAS LOS USUARIOS
        //-------------------------------------------------------------------

        [HttpGet("todos")]
        public IActionResult GetAll()
        {
            try
            {
                var usuarios = _usuariosRepository.ObtenerTodas();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener usuarios: {ex.InnerException?.Message}");
            }
        }
        //-------------------------------------------------------------------
        //FIN DE OBTENER TODOS LOS USUARIOS
        //-------------------------------------------------------------------

        //--------------------------------------------------------------------------------------
        /// OBTENER USUARIOS POR ID
        //--------------------------------------------------------------------------------------

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var usuarios = _usuariosRepository.ObtenerPorId(id);
            if (usuarios == null) return NotFound();
            return Ok(usuarios);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Usuarios usu)
        {
            if (usu is null || usu.IdUsuario != id) return BadRequest("Datos inválidos");
            var existente = _usuariosRepository.ObtenerPorId(id);
            if (existente is null) return NotFound();
            _usuariosRepository.Actualizar(usu);
            return NoContent();
        }
        //--------------------------------------------------------------------------------------
        /// FIN DE OBTENER USUARIOS POR ID.
        //--------------------------------------------------------------------------------------

        //--------------------------------------------------------------------------------------
        /// DELETE USUARIO POR ID
        //--------------------------------------------------------------------------------------

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existente = _usuariosRepository.ObtenerPorId(id);
            if (existente is null) return NotFound();

            bool eliminado = _usuariosRepository.Eliminar(id);
            if (!eliminado)
            {
                return BadRequest("No se puede eliminar Usuario por alguna razón.");
            }
            return Ok("Usuario eliminado correctamente.");

        }
        //--------------------------------------------------------------------------------------
        /// FIN DELETE USUARIO POR ID.
        //--------------------------------------------------------------------------------------
    }
}
#endregion

