using System;
using System.Collections.Generic;
using Arca.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace Arca.MVC.Models
{
    public class EspecieFormViewModel
    {
        // Datos de la semilla
        public int IdEspecie { get; set; }
        public string NombreCientifico { get; set; }
        public string NombreComun { get; set; }
        public string Familia { get; set; }
        public string Descripcion { get; set; }

        // Listas para dropdown //***---CREO QUE ESTO SE PUEDE ELIMINAR
        public List<SelectListItem> Especies { get; set; } = new();
        public List<SelectListItem> Ubicaciones { get; set; } = new();
    }
}
