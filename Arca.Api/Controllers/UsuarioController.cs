using Microsoft.AspNetCore.Mvc;
using Arca.Data.Repositories;
using Arca.Data.Models;

namespace Arca.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioRepository _usuarioRepository;

        public UsuarioController(IConfiguration configuration)
        {
            // Inicializa el repositorio con la cadena de conexión desde appsettings.json
            string connectionString = configuration.GetConnectionString("DefaultConnection");
            _usuarioRepository = new UsuarioRepository(connectionString);
        }

        #region Endpoints de Registro y Búsqueda

        /// <summary>
        /// Registra un nuevo usuario.
        /// </summary>
        [HttpPost("registrar")]
        public IActionResult RegistrarUsuario([FromBody] Usuario usuario)
        {
            if (usuario == null)
                return BadRequest("Datos del usuario inválidos");

            try
            {
                _usuarioRepository.RegistrarUsuario(usuario);
                return Ok("Usuario registrado correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al registrar el usuario: {ex.Message}{ex.InnerException?.Message}");
            }
        }

        /// <summary>
        /// Obtiene un usuario por su email.
        /// </summary>
        [HttpGet("por-email/{email}")]
        public IActionResult ObtenerPorEmail(string email)
        {
            try
            {
                var usuario = _usuarioRepository.ObtenerPorEmail(email);
                if (usuario == null)
                    return NotFound("Usuario no encontrado");

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el usuario: {ex.Message}");
            }
        }

        #endregion

        #region Endpoints CRUD



        // PUT: api/usuario/actualizar
        [HttpPut("actualizar")]
        public IActionResult ActualizarUsuario([FromBody] Usuario usuario)
        {
            if (usuario == null || usuario.IdUsuario == 0)
                return BadRequest("Datos del usuario inválidos o IdUsuario no especificado.");

            try
            {
                var usuarioExistente = _usuarioRepository.ObtenerUsuarioPorId(usuario.IdUsuario);
                if (usuarioExistente == null)
                    return NotFound("Usuario no encontrado para actualizar.");

                _usuarioRepository.ActualizarUsuario(usuario);
                return Ok("Usuario actualizado correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar el usuario: {ex.Message}");
            }
        }

        // DELETE: api/usuario/eliminar/{id}
        [HttpDelete("eliminar/{id}")]
        public IActionResult EliminarUsuario(int id)
        {
            if (id <= 0)
                return BadRequest("IdUsuario inválido.");

            try
            {
                var usuarioExistente = _usuarioRepository.ObtenerUsuarioPorId(id);
                if (usuarioExistente == null)
                    return NotFound("Usuario no encontrado para eliminar.");

                _usuarioRepository.EliminarUsuario(id);
                return Ok("Usuario eliminado correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar el usuario: {ex.Message}");
            }
        }






        /// <summary>
        /// Obtiene todos los usuarios.
        /// </summary>
        [HttpGet]
        public ActionResult<IEnumerable<Usuario>> GetUsuarios()
        {
            try
            {
                var usuarios = _usuarioRepository.ObtenerTodosUsuarios();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener los usuarios: {ex.Message}");
            }
        }

        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        [HttpGet("{id}")]
        public ActionResult<Usuario> GetUsuarioById(int id)
        {
            try
            {
                var usuario = _usuarioRepository.ObtenerUsuarioPorId(id);
                if (usuario == null)
                    return NotFound("Usuario no encontrado");

                return Ok(usuario);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener el usuario: {ex.Message}");
            }
        }

        #endregion
    }
}