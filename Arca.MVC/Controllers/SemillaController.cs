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
    public class SemillaController : Controller
    {
        private readonly SemillaRepository _semillaRepository;

        public SemillaController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            _semillaRepository = new SemillaRepository(connectionString);
        }

        // ===============================
        // LISTAR SEMILLAS
        // ===============================
        public IActionResult Index()
        {
            var semillas = _semillaRepository.ObtenerTodas();
            return View(semillas);
        }

        // ===============================
        // CREAR SEMILLA
        // ===============================
        [HttpGet]
        public IActionResult Create()
        {
            var vm = new SemillaFormViewModel
            {
                FechaAlmacenamiento = DateTime.Today
            };

            vm = CargarCombos(vm); // carga combos para crear

            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(SemillaFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm = CargarCombos(vm);
                return View(vm);
            }

            var s = new Semilla
            {
                Nombre = vm.Nombre,
                IdEspecie = vm.IdEspecie,
                IdUbicacion = vm.IdUbicacion,
                Cantidad = vm.Cantidad,
                FechaAlmacenamiento = vm.FechaAlmacenamiento
            };

            _semillaRepository.RegistrarSemilla(s);
            TempData["SuccessMessage"] = "Semilla registrada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // EDITAR SEMILLA
        // ===============================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var semilla = _semillaRepository.ObtenerPorId(id);
            if (semilla == null) return NotFound();

            var vm = new SemillaFormViewModel
            {
                IdSemilla = semilla.IdSemilla,
                Nombre = semilla.Nombre,
                IdEspecie = semilla.IdEspecie,
                IdUbicacion = semilla.IdUbicacion,
                Cantidad = semilla.Cantidad,
                FechaAlmacenamiento = semilla.FechaAlmacenamiento
            };
            // Cargar combos y seleccionar el valor de la semilla
            vm = CargarCombos(vm);
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(SemillaFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm = CargarCombos(vm);
                return View(vm);
            }

            var s = _semillaRepository.ObtenerPorId(vm.IdSemilla);
            if (s == null) return NotFound();

            s.Nombre = vm.Nombre;
            s.IdEspecie = vm.IdEspecie;
            s.IdUbicacion = vm.IdUbicacion;
            s.Cantidad = vm.Cantidad;
            s.FechaAlmacenamiento = vm.FechaAlmacenamiento;

            
            _semillaRepository.Actualizar(s);
            TempData["SuccessMessage"] = "Semilla modificada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // BORRAR SEMILLA
        // ===============================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var semilla = _semillaRepository.ObtenerPorId(id);
            if (semilla == null) return NotFound();
            return View(semilla); // Muestra Delete.cshtml
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            var semilla = _semillaRepository.ObtenerPorId(id);
            if (semilla == null) return NotFound();

            _semillaRepository.Eliminar(id);
            TempData["SuccessMessage"] = "Semilla eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // MÉTODO AUXILIAR CARGAR COMBOS
        // ===============================
        private SemillaFormViewModel CargarCombos(SemillaFormViewModel vm)
        {
            var especies = _semillaRepository.ObtenerTodasEspecies();
            vm.Especies = especies.Select(e => new SelectListItem
            {
                Value = e.IdEspecie.ToString(),
                Text = $"{e.NombreComun} ({e.NombreCientifico})",
                Selected = e.IdEspecie == vm.IdEspecie // selecciona la especie actual
            }).ToList();

            var ubicaciones = _semillaRepository.ObtenerTodasUbicaciones();
            vm.Ubicaciones = ubicaciones.Select(u => new SelectListItem
            {
                Value = u.IdUbicacion.ToString(),
                Text = u.Nombre,
                Selected = u.IdUbicacion == vm.IdUbicacion // selecciona la ubicación actual
            }).ToList();

            return vm;
        }
    }
}