using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MTPMicrositio.Models.Web
{
    public class ComentarioViewModel
    {
        public Guid ComentarioId { get; set; }

        [Required(ErrorMessage = "El proyecto es requerido.")]
        [Display(Name = "Proyecto")]
        public Guid? ProyectoId { get; set; }

        public Guid? UsuarioId { get; set; }

        [Required(ErrorMessage = "La descripción o comentario técnico es requerido.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Descripción del Comentario")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La prioridad es requerida.")]
        [Display(Name = "Prioridad")]
        public Guid? PrioridadId { get; set; }

        [Display(Name = "¿Vigente / Activo?")]
        public bool Activo { get; set; }

        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Fecha de Registro")]
        public DateTime? Fecha { get; set; }

        [Required(ErrorMessage = "El estatus es requerido.")]
        [Display(Name = "Estatus")]
        public Guid? EstatusId { get; set; }

        // Elementos de UI independientes para poblar DropDownLists
        public IEnumerable<SelectListItem> ListaProyectos { get; set; }
        public IEnumerable<SelectListItem> ListaPrioridades { get; set; }
        public IEnumerable<SelectListItem> ListaEstatus { get; set; }
    }

    public class ComentarioFiltroViewModel
    {
        public Guid? FiltrarProyectoId { get; set; }
        public Guid? FiltrarPrioridadId { get; set; }
        public Guid? FiltrarEstatusId { get; set; }
        public List<ComentarioViewModel> Comentarios { get; set; }
        public IEnumerable<SelectListItem> ListaProyectos { get; set; }
        public IEnumerable<SelectListItem> ListaPrioridades { get; set; }
        public IEnumerable<SelectListItem> ListaEstatus { get; set; }
    }
}