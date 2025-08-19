using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Arca.MVC.Models
{
    public class ReporteFiltroViewModel
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public int? EspecieId { get; set; }
        public List<SelectListItem> Especies { get; set; } = new();

        public int? UbicacionId { get; set; }
        public List<SelectListItem> Ubicaciones { get; set; } = new();


        // ---- NUEVOS (para programar):
        public string Formato { get; set; }          // pdf | xlsx | csv
        public string Frecuencia { get; set; }       // unico | diaria | semanal | mensual
        public DateTime? ProximoEnvio { get; set; }  // fecha/hora deseada
        public string Destinatarios { get; set; }    // correos separados por coma
    }
}