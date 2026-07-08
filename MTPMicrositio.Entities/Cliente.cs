using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPMicrositio.Entities
{
    public class Cliente
    {
        public Guid ClienteId { get; set; }
        public string Nombre { get; set; }
        public Guid? EstatusId { get; set; }
        public string LogoUrl { get; set; }
        public Guid UsuarioCreacionID { get; set; }
        public bool LogoAutorizado { get; set; }
        public DateTime? FechaAutorizacion { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
