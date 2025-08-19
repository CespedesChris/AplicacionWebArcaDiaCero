using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arca.Data.Models
{
    public class Ubicacion
    {
        public int IdUbicacion { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string Condiciones { get; set; }
    }
}
