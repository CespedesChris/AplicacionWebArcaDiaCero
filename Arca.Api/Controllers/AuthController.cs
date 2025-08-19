using Microsoft.AspNetCore.Mvc;
using Arca.Data.Repositories;
using Arca.Data.Models;
using Arca.Api.Helpers;


namespace Arca.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UsuariosRepository _usuariosRepository;

        public AuthController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _usuariosRepository = new UsuariosRepository(connectionString);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
                return BadRequest("Email y Password son requeridos");

            var usuario = _usuariosRepository.ObtenerPorEmail(request.Email);
            if (usuario == null)
                return Unauthorized("Credenciales incorrectas");

            if (!PasswordHelper.VerifyPassword(request.Password, usuario.PasswordHash))
                return Unauthorized("Credenciales incorrectas");

            // Aquí podrías generar JWT, pero por ahora solo devuelvo OK
            return Ok(new
            {
                usuario.IdUsuario,
                usuario.Nombre,
                usuario.Email,
                usuario.IdRol
            });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}