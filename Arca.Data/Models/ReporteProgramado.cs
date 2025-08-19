using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arca.Data.Models;

public class ReporteProgramado
{
    public int Id { get; set; }
    public string NombreReporte { get; set; }
    public string Formato { get; set; }
    public string Frecuencia { get; set; }
    public string Destinatarios { get; set; } // correos separados por coma
    public DateTime? ProximoEnvio { get; set; }
    public string Parametros { get; set; }  // JSON con los filtros
    public DateTime FechaCreacion { get; set; }







}

