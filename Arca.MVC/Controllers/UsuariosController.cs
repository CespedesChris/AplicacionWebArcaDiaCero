using Arca.Api.Helpers;
using Arca.Data.Models;
using Arca.Data.Repositories;
using Arca.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arca.MVC.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly UsuariosRepository _usuariosRepository;

        public UsuariosController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _usuariosRepository = new UsuariosRepository(connectionString);
        }

        //-----------------------*---------------------------*-------------------------*-----------------------------*-----------------------*--------------
        // =============================================================
        // *------------------* VISTA ADMINISTRADOR
        // =============================================================
        [HttpGet]
        public IActionResult VistaAdministrador()
        {
            // Revisar que haya un usuario logueado
            if (HttpContext.Session.GetInt32("IdUsuario") == null)
                return RedirectToAction("Login");

            // Revisar que el rol sea Administrador (IdRol = 1)
            int rol = HttpContext.Session.GetInt32("IdRol") ?? 0;
            if (rol != 1)
                return RedirectToAction("Index"); // o mostrar mensaje de acceso denegado

            return View();
        }


        // ===============================
        // LOGIN (GET)
        // ===============================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ===============================
        // LOGIN (POST)
        // ===============================

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var usuario = _usuariosRepository.ObtenerPorEmail(email);
            if (usuario == null)
            {
                ViewBag.Error = "Usuario no encontrado";
                return View();
            }

            if (!PasswordHelper.VerifyPassword(password, usuario.PasswordHash))
            {
                ViewBag.Error = "Contraseña incorrecta";
                return View();
            }

            // Guardamos datos en sesión
            HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
            HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
            HttpContext.Session.SetInt32("UsuarioRol", usuario.IdRol);


            if (usuario.IdRol == 1)
            {
                return RedirectToAction("VistaAdministrador", "Home"); // Devuelve la Vista Admin
            }
            else
            {
                return RedirectToAction("VistaBasica", "Home"); // Devuelve la Vista Básica
            }

        }

        // ===============================
        // LOGOUT
        // ===============================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }





        // ===============================
        // LISTAR USUARIOS
        // ===============================
        public IActionResult Index()
        {
            var usuarios = _usuariosRepository.ObtenerTodas();
            return View(usuarios);
        }

        // ===============================
        // CREAR USUARIO
        // ===============================
        [HttpGet]

        public IActionResult Create()
        {
            var vm = new UsuariosFormViewModel
            {
                
            
            };
            vm = CargarCombos(vm); // carga combo IdRol para crear
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(UsuariosFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm = CargarCombos(vm); // carga combo IdRol para crear
                return View(vm);
            }

            var usu = new Usuarios
            {
                Nombre = vm.Nombre,
                Apellido = vm.Apellido,
                Email = vm.Email,
                IdRol = vm.IdRol,
                // Aquí generamos el hash seguro
                PasswordHash = PasswordHelper.HashPassword(vm.PasswordHash)

                // PasswordHash = vm.PasswordHash,

            };


            _usuariosRepository.RegistrarUsuarios(usu);
            TempData["SuccessMessage"] = "Usuario registrado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // EDITAR USUARIOS
        // ===============================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var usuarios = _usuariosRepository.ObtenerPorId(id);
            if (usuarios == null) return NotFound();

            var vm = new UsuariosFormViewModel
            {
                IdUsuario = usuarios.IdUsuario,
                Nombre = usuarios.Nombre,
                Apellido = usuarios.Apellido,
                Email = usuarios.Email,
                PasswordHash = usuarios.PasswordHash,
                IdRol = usuarios.IdRol
            };
            // Cargar combo DE LOS ROLES
            vm = CargarCombos(vm);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(UsuariosFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm = CargarCombos(vm);
                return View(vm);
            }

            var usu = _usuariosRepository.ObtenerPorId(vm.IdUsuario);
            if (usu == null) return NotFound();

            usu.Nombre = vm.Nombre;
            usu.Apellido = vm.Apellido;
            usu.Email = vm.Email;   
            usu.PasswordHash = vm.PasswordHash;
            usu.IdRol = vm.IdRol;
            _usuariosRepository.Actualizar(usu);
            TempData["SuccessMessage"] = "Usuario modificado correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // BORRAR USUARIO
        // ===============================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var usuarios = _usuariosRepository.ObtenerPorId(id);
            if (usuarios == null) return NotFound();
            return View(usuarios); // Muestra Delete.cshtml
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            var usuarios = _usuariosRepository.ObtenerPorId(id);
            if (usuarios == null) return NotFound();
            bool eliminado = _usuariosRepository.Eliminar(id);
            if (!eliminado)
            {
                TempData["ErrorMessage"] = "No se puede eliminar usuario";
                return RedirectToAction(nameof(Index));
            }
            TempData["SuccessMessage"] = "Usuario eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }


        // ===============================
        // MÉTODO AUXILIAR CARGA COMBO ROL
        // ===============================
        private UsuariosFormViewModel CargarCombos(UsuariosFormViewModel vm)
        {
            var roles = _usuariosRepository.ObtenerTodosRoles();
            vm.Roles = roles.Select(e => new SelectListItem
            {
                Value = e.IdRol.ToString(),
                Text = $"{e.NombreRol} ({e.NombreRol})",
                Selected = e.IdRol == vm.IdRol // selecciona el usuario actual
            }).ToList();

            return vm;
        }






    }
}