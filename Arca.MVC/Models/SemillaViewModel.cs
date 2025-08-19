using System;
using System.Collections.Generic;
using Arca.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace Arca.MVC.Models;
public class SemillaViewModel
{
    public int IdSemilla { get; set; }
    public string Nombre { get; set; }
    public int IdEspecie { get; set; }
    public string NombreEspecie { get; set; }
    public int IdUbicacion { get; set; }
    public string NombreUbicacion { get; set; }
    public int Cantidad { get; set; }
    public DateTime FechaAlmacenamiento { get; set; }
}