using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arca.Data.Models
{
    public class Semilla
    {
        public int IdSemilla { get; set; }
        public string Nombre { get; set; }
        
        public int IdEspecie { get; set; }
        
        public int IdUbicacion { get; set; }
        
        public int Cantidad { get; set; }
        
        public DateTime FechaAlmacenamiento { get; set; }

        // Para mostrar info relacionada en la UI
        public Especie Especie { get; set; }
        public Ubicacion Ubicacion { get; set; }
    }
}
