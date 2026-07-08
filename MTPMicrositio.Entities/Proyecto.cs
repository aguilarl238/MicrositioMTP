using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Web.Mvc;

namespace MTPMicrositio.Entities
{
    public class Proyecto
    {
        public Guid ProyectoId { get; set; }

        
        public Guid? ClienteId { get; set; }

       
        public Guid? UsuarioId { get; set; }

        
        public string Nombre { get; set; }

        
        public string Tipo { get; set; }

        
        public Guid? EstatusId { get; set; }

        
        public bool Activo { get; set; }

       
        public DateTime? FechaInicio { get; set; }

        //Listas de selección para los DropdownLists en las vistas
        public IEnumerable<SelectListItem> ListaClientes { get; set; }
        public IEnumerable<SelectListItem> ListaUsuarios { get; set; }
        public IEnumerable<SelectListItem> ListaEstatus { get; set; }
    }
}