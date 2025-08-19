using System;
using System.Collections.Generic;
using Arca.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace Arca.MVC.Models
{
    public class UbicacionFormViewModel
    {
        // Datos de la Ubicación
        public int IdUbicacion { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Condiciones { get; set; }
        
    }
}
