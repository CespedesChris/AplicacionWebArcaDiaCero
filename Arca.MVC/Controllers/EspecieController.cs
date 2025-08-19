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
    public class EspecieController : Controller
    {
        private readonly EspecieRepository _especieRepository; //----ANTES
        public EspecieController(IConfiguration configuration) // ------ ANTES
        {
           var connectionString = configuration.GetConnectionString("DefaultConnection");
           _especieRepository = new EspecieRepository(connectionString);


        }
        // ===========================================================================================
        // -------------------------*   VER DETALLE DE LA ESPECIE SELECCIONADA 
        // ===========================================================================================
        [HttpGet]
        public IActionResult VerDetalle(int id)
        {
            var especie = _especieRepository.ObtenerPorId(id);
            if (especie == null) return NotFound();

            // Creamos un ViewModel para separar la lógica de la vista
            var vm = new EspecieFormViewModel
            {
                IdEspecie = especie.IdEspecie,
                NombreCientifico = especie.NombreCientifico,
                NombreComun = especie.NombreComun,
                Familia = especie.Familia,
                Descripcion = especie.Descripcion
            };

            return View(vm);
        }

        // ===========================================================================================
        // -------------------------*   FIN VER DETALLE DE LA ESPECIE SELECCIONADA 
        // ===========================================================================================




        // ===============================
        // LISTAR ESPECIES
        // ===============================
        public IActionResult Index()
        {
            var especies = _especieRepository.ObtenerTodas();
            return View(especies);
        }

        // ===============================
        // CREAR SEMILLA
        // ===============================
        [HttpGet]
        public IActionResult Create() // CREO QUE NO SE NECESITA CARGAR COMBOS PORQUE ES INGRESAR ESPECIE NO SE NECESITA COMBOS
        {
            //var vm = new EspecieFormViewModel
            // {
            //     FechaAlmacenamiento = DateTime.Today
            // };

            //  vm = CargarCombos(vm); // carga combos para crear

            return View();
        }

        [HttpPost]
        public IActionResult Create(EspecieFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                //vm = CargarCombos(vm);
                return View(vm);
            }

            var e = new Especie
            {
                NombreCientifico = vm.NombreCientifico,
                NombreComun = vm.NombreComun,
                Familia = vm.Familia,
                Descripcion = vm.Descripcion
            };


            _especieRepository.RegistrarEspecie(e);

            TempData["SuccessMessage"] = "Especie registrada correctamente.";
            

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // EDITAR ESPECIE
        // ===============================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var especie = _especieRepository.ObtenerPorId(id);
            if (especie == null) return NotFound();

            var vm = new EspecieFormViewModel
            {
                IdEspecie = especie.IdEspecie,
                NombreCientifico = especie.NombreCientifico,
                NombreComun = especie.NombreComun,
                Familia = especie.Familia,
                Descripcion = especie.Descripcion,
            };
            // Cargar combos y seleccionar el valor de la semilla // CREO QUE ESTO NO VA PORQUE NO HAY COMBOS
            //vm = CargarCombos(vm);
            
            return View(vm);
        }

        [HttpPost]
        public IActionResult Edit(EspecieFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // vm = CargarCombos(vm);
                return View(vm);
            }

            var e = _especieRepository.ObtenerPorId(vm.IdEspecie);
            if (e == null) return NotFound();

            e.NombreCientifico = vm.NombreCientifico;
            e.NombreComun = vm.NombreComun;
            e.Familia = vm.Familia;
            e.Descripcion = vm.Descripcion;


            _especieRepository.Actualizar(e);
            TempData["SuccessMessage"] = "Especie Editada Correctamente.";
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // BORRAR ESPECIE
        // ===============================
        [HttpGet]
        public IActionResult Delete(int id)
        {
            var especie = _especieRepository.ObtenerPorId(id);
            if (especie == null) return NotFound();
            return View(especie); // Muestra Delete.cshtml
        }

        [HttpPost, ActionName("DeleteConfirmed")]
        public IActionResult DeleteConfirmed(int id)
        {
            var especie = _especieRepository.ObtenerPorId(id);
            if (especie == null) return NotFound();
            bool eliminado = _especieRepository.Eliminar(id);
            if (!eliminado)
            {
                TempData["ErrorMessage"] = "No se puede eliminar la especie porque tiene semillas asociadas.";
                return RedirectToAction(nameof(Index));
            }
            TempData["SuccessMessage"] = "Especie eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
