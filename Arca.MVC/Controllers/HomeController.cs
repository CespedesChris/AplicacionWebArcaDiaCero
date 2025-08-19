using Arca.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Arca.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult VistaBasica() // MUESTRA LA VISTA BASICA SI NO ES ADMIN
        {
            return View(); 
        }

        public IActionResult VistaAdministrador() // MUESTRA LA VISTA DEL ADMIN
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
