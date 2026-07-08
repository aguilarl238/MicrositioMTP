using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApiMTPMicrositio.Models.Web
{
    public class ContactoViewModel
    {
        [Required(ErrorMessage = "El nombre completo es requerido.")]
        [StringLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
        [Display(Name = "Nombre Completo")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El correo corporativo es requerido.")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder los 100 caracteres.")]
        [Display(Name = "Correo Corporativo")]
        public string CorreoCorporativo { get; set; }

        [Required(ErrorMessage = "El mensaje o requerimiento es requerido.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Mensaje / Requerimiento")]
        public string Mensaje { get; set; }
    }
}