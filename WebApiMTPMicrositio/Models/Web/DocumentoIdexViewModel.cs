using System;
using System.Collections.Generic;
using System.Web.Mvc;
using MTPMicrositio.Entities;

namespace MTPMicrositio.Models.Web
{
    public class DocumentoIndexViewModel
    {
        public Guid? FiltroClienteId { get; set; }
        public Guid? FiltroProyectoId { get; set; }

        public List<DocumentoDTO> Documentos { get; set; }

        public IEnumerable<SelectListItem> Clientes { get; set; }
        public IEnumerable<SelectListItem> Proyectos { get; set; }
    }
}