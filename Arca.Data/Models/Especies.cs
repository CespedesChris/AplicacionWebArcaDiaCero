using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arca.Data.Models
{
    public class Especie
    {
        public int IdEspecie { get; set; }
        public string NombreCientifico { get; set; }
        public string NombreComun { get; set; }
        public string Familia { get; set; }
        public string Descripcion { get; set; }
    }
}
