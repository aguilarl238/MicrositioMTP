using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MTPMicrositio.Models.Web
{
    public class CambiarPasswordViewModel
    {
        [Required]
        public Guid UsuarioId { get; set; }

        [Display(Name = "Usuario")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mínimo 6 caracteres.")]
        [Display(Name = "Nueva contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarPassword { get; set; }
    }
}