using System;
using System.Collections.Generic;
using Arca.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace Arca.MVC.Models
{
    public class SemillaFormViewModel
    {
        // Datos de la semilla
        public int IdSemilla { get; set; }
        public string Nombre { get; set; }
        public int IdEspecie { get; set; }
        public int IdUbicacion { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaAlmacenamiento { get; set; }

        // Listas para dropdown
        public List<SelectListItem> Especies { get; set; } = new();
        public List<SelectListItem> Ubicaciones { get; set; } = new();
    }
}
