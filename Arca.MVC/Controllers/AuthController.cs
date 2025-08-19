using Arca.Data.Models;
using Arca.Data.Repositories; // tu namespace real de repositorio
using Microsoft.AspNetCore.Mvc;
namespace Arca.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly UsuariosRepository _usuariosRepo;

        public AuthController(UsuariosRepository usuariosRepo)
        {
            _usuariosRepo = usuariosRepo;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var usuario = _usuariosRepo.Login(email, password);
            if (usuario != null)
            {
                HttpContext.Session.SetInt32("IdUsuario", usuario.IdUsuario);
                HttpContext.Session.SetString("Nombre", usuario.Nombre);
                HttpContext.Session.SetInt32("IdRol", usuario.IdRol);

                return RedirectToAction("Index", "Home"); // redirige al inicio
            }

            ViewBag.Error = "Usuario o contraseña incorrectos";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
