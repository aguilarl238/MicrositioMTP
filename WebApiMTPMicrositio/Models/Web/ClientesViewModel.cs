using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace MTPMicrositio.Models.Web
{
    public class ClientesViewModel
    {
        public Guid ClienteId { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres.")]
        [Display(Name = "Nombre de la Empresa")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe asignar un estatus válido de la lista.")]
        [Display(Name = "Estatus Comercial")]
        public Guid? EstatusId { get; set; }

        [Display(Name = "Cuenta Activa")]
        public bool Activo { get; set; } = true;

        [Url(ErrorMessage = "Ingrese una URL válida para el logotipo.")]
        [StringLength(500, ErrorMessage = "La URL no puede exceder los 500 caracteres.")]
        [Display(Name = "URL del Logotipo")]
        public string LogoUrl { get; set; }

        public Guid? UsuarioCreacionID { get; set; }

        [Display(Name = "Autorización de marca registrada")]
        //[AssertThatLogoAuthorized(ErrorMessage = "Debe certificar que cuenta con autorización escrita.")]
        public bool LogoAutorizado { get; set; }

        // Propiedad exclusiva de la Vista para pintar el Combo Box DropDown
        public IEnumerable<SelectListItem> ListadoEstatus { get; set; }

        // Propiedad clave para recibir los IDs seleccionados desde la Vista/Modal
        public List<Guid> UsuariosSeleccionadosIds { get; set; } = new List<Guid>();
    }
}