using Microsoft.AspNetCore.Mvc;
using Arca.Data.Models;
using Arca.Data.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Arca.MVC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arca.MVC.Controllers
{
    public class UbicacionController : Controller
    {
        private readonly UbicacionRepository _ubicacionRepository;

        public UbicacionController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _ubicacionRepository = new UbicacionRepository(connectionString);
        }

        // ===============================
        // LISTAR UBICACIONES
        // ===============================
        public IActionResult Index()
        {
            var ubicacion = _ubicacionRepository.ObtenerTodas();
            return View(ubicacion);
        }

        // ===============================
        // CREAR UBICACION
        // ===============================
        [HttpGet]
        public IActionResult Create() 
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(UbicacionFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                //vm = CargarCombos(vm);
                return View(vm);
            }

            var u = new Ubicacion
            {
                Nombre = vm.Nombre,
                Descripcion = vm.Descripcion,
                Condiciones = vm.Condiciones
            };


            _ubicacionRepository.RegistrarUbicacion(u);
            TempData["SuccessMessage"] = "Ubicación registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // EDITAR UBICACION
        // ===============================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var ubicacion = _ubicacionRepository.ObtenerPorId(id);
            if (ubicacion == null) return NotFound();

            var vm = new UbicacionFormViewModel
            {
                IdUbicacion = ubicacion.IdUbicacion,
                Nombre = ubicacion.Nombre,
                Descripcion = ubicacion.Descripcion,
                Condiciones = ubicacion.Condiciones,
            };
            // Cargar combos y seleccionar el valor de la semilla // CREO QUE ESTO NO VA PORQUE NO HAY COMBOS
            //vm = CargarCombos(vm);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(UbicacionFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // vm = CargarCombos(vm);
                return View(vm);
            }

            var u = _ubicacionRepository.ObtenerPorId(vm.IdUbicacion);
            if (u == null) return NotFound();

            u.Nombre = vm.Nombre;
            u.Descripcion = vm.Descripcion;
            u.Condiciones = vm.Condiciones;
            _ubicacionRepository.Actualizar(u);
            TempData["SuccessMessage"] = "Ubicación registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // BORRAR UBICACION
        // ===============================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var ubicacion = _ubicacionRepository.ObtenerPorId(id);
            if (ubicacion == null) return NotFound();
            return View(ubicacion); // Muestra Delete.cshtml
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            var ubicacion = _ubicacionRepository.ObtenerPorId(id);
            if (ubicacion == null) return NotFound();
            bool eliminado = _ubicacionRepository.Eliminar(id);
            if (!eliminado)
            {
                TempData["ErrorMessage"] = "No se puede eliminar la ubicacion.";
                return RedirectToAction(nameof(Index));
            }
            TempData["SuccessMessage"] = "Ubicacion eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}