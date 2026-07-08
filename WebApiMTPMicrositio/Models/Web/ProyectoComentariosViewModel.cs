using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MTPMicrositio.Entities;

namespace MTPMicrositio.Models.Web
{
    public class ProyectoComentariosViewModel
    {
        // Contexto del Proyecto Actual
        public Guid ProyectoId { get; set; }
        public string ProyectoNombre { get; set; }
        public string CodigoProyecto { get; set; }

        // Listados para renderizar la interfaz
        public List<ComentarioDto> HistorialComentarios { get; set; }
        public IEnumerable<SelectListItem> DocumentosAsociados { get; set; }
        public IEnumerable<SelectListItem> ListaPrioridades { get; set; }


        // Propiedades de captura para el Nuevo Comentario (Binding)
        [Required(ErrorMessage = "El cuerpo del comentario o requerimiento es obligatorio.")]
        [DataType(DataType.MultilineText)]
        public string NuevoComentarioTexto { get; set; }

        //[Required(ErrorMessage = "Debe vincular el comentario a un componente o documento del proyecto.")]
        public Guid? DocumentoVinculadoId { get; set; }

        public Guid? PrioridadId { get; set; }
    }
}