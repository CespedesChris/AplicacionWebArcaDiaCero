using System;
using System.Collections.Generic;
using Arca.Data.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Arca.MVC.Models
{
    public class UsuariosFormViewModel
    {
        // Datos del Usuario
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int IdRol { get; set; }
        // Listas para dropdown
        public List<SelectListItem> Roles { get; set; } = new();


    }
}