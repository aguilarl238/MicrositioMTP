using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace MTPMicrositio.Models.Web
{
    public class DocumentoViewModel
    {
        public Guid DocumentoId { get; set; }

        [Required(ErrorMessage = "El cliente es mandatorio.")]
        public Guid? ClienteId { get; set; }

        [Required(ErrorMessage = "Establezca el tipo de documento.")]
        public Guid? TipoDocumentoId { get; set; }

        [Required(ErrorMessage = "El nombre técnico del documento es requerido.")]
        [StringLength(200, ErrorMessage = "No puede superar los 200 caracteres.")]
        public string Nombre { get; set; }

        public bool Activo { get; set; } = true;

        [Required(ErrorMessage = "Seleccione el estatus actual de control.")]
        public Guid? EstatusId { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? FechaVigencia { get; set; }

        [Required(ErrorMessage = "El documento debe asociarse a un proyecto técnico.")]
        public Guid? ProyectoId { get; set; }

        [Display(Name = "Adjuntar Documento Digital (PDF, Word, Imagen)")]
        public HttpPostedFileBase ArchivoCargado { get; set; }

        public string UrlDoc {  get; set; }

        public string ArchivoBase64 { get; set; }
        public string ExtensionArchivo { get; set; }

        public Guid? UsuarioId { get; set; }

        // Listas de Selección (Combos)
        public IEnumerable<SelectListItem> Clientes { get; set; }
        public IEnumerable<SelectListItem> TiposDocumento { get; set; }
        public IEnumerable<SelectListItem> Estatus { get; set; }
        public IEnumerable<SelectListItem> Proyectos { get; set; }
    }
}