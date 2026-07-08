using System.ComponentModel.DataAnnotations;

namespace MTPMicrositio.Models.Web
{
    public class ClienteViewModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres.")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El estatus es obligatorio.")]
        [Display(Name = "Estatus")]
        public string Estatus { get; set; }

        [Url(ErrorMessage = "Debe ser una URL válida.")]
        [StringLength(500)]
        [Display(Name = "URL del Logo")]
        public string LogoUrl { get; set; }

        [Display(Name = "¿Logo autorizado?")]
        public bool LogoAutorizado { get; set; }
    }
}