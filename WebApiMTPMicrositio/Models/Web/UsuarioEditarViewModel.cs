using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MTPMicrositio.Models.Web
{
    public class UsuarioEditarViewModel
    {
        [Required]
        public Guid UsuarioId { get; set; }

        [Display(Name = "Cliente")]
        public Guid? ClienteId { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ser un email válido.")]
        [StringLength(150)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(200)]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "¿Activo?")]
        public bool Activo { get; set; }

        [Required(ErrorMessage = "El Rol es obligatorio.")]
        [Display(Name = "Rol")]
        public Guid RolId { get; set; }

        // Propiedad exclusiva de la Vista para pintar el Combo Box DropDown
        public IEnumerable<SelectListItem> ListadoRol { get; set; }
        public IEnumerable<SelectListItem> ListadoClientes { get; set; }
    }
}