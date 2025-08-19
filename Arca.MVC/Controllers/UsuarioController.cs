using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Arca.Data.Models;

namespace Arca.Mvc.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly HttpClient _httpClient;
        public UsuarioController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7241/api/");
        }

        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            var usuarios = await _httpClient.GetFromJsonAsync<List<Usuario>>("usuario");
            return View(usuarios);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

            var response = await _httpClient.PostAsJsonAsync("usuario/registrar", usuario);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("", "Error al crear el usuario");
            return View(usuario);
        }
    }
}