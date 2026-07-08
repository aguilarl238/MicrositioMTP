using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPMicrositio.Entities
{
    public class ComentarioDto
    {
        public Guid ComentarioId { get; set; }
        public Guid? ProyectoId { get; set; }
        public Guid? UsuarioId { get; set; }
        public string Descripcion { get; set; }
        public Guid? PrioridadId { get; set; }
        public bool Activo { get; set; }
        public DateTime? Fecha { get; set; }
        public Guid? EstatusId { get; set; }
    }
}
