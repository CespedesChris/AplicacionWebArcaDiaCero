using Microsoft.AspNetCore.Mvc;
using Arca.Data.Repositories;
namespace Arca.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UbicacionController : ControllerBase
    {
        private readonly UbicacionRepository _repo;
        public UbicacionController(IConfiguration cfg)
        {
            _repo = new UbicacionRepository(cfg.GetConnectionString("DefaultConnection"));
        }

        [HttpGet]
        public IActionResult Get() => Ok(_repo.ObtenerTodas());
    }
}
