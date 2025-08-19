using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arca.Data.Models
{
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
}

