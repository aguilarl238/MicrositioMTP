using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MTPMicrositio.Models.Web
{
    public class ProyectoViewModel
    {
        public Guid ProyectoId { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio.")]
        [Display(Name = "Cliente")]
        public Guid? ClienteId { get; set; }

        [Required(ErrorMessage = "El usuario asignado es obligatorio.")]
        [Display(Name = "Usuario Asignado")]
        public Guid? UsuarioId { get; set; }

        [Required(ErrorMessage = "El nombre del proyecto es obligatorio.")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder los 200 caracteres.")]
        [Display(Name = "Nombre del Proyecto")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El tipo de proyecto es obligatorio.")]
        [StringLength(50, ErrorMessage = "El tipo no puede exceder los 50 caracteres.")]
        [Display(Name = "Tipo de Proyecto")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "El estatus es obligatorio.")]
        [Display(Name = "Estatus")]
        public Guid? EstatusId { get; set; }

        [Display(Name = "¿Proyecto Activo?")]
        public bool Activo { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es obligatoria.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Fecha de Inicio")]
        public DateTime? FechaInicio { get; set; }

        // Listas de selección para los DropdownLists en las vistas
        public IEnumerable<SelectListItem> ListaClientes { get; set; }
        public IEnumerable<SelectListItem> ListaUsuarios { get; set; }
        public IEnumerable<SelectListItem> ListaEstatus { get; set; }
        public IEnumerable<ComentarioViewModel> ListaComentarios { get; set; }
    }

    public class ProyectoFiltroViewModel
    {
        public Guid? FiltrarClienteId { get; set; }
        public Guid? FiltrarUsuarioId { get; set; }
        public List<ProyectoViewModel> Proyectos { get; set; }
        public IEnumerable<SelectListItem> ListaClientes { get; set; }
        public IEnumerable<SelectListItem> ListaUsuarios { get; set; }
        
    }
}