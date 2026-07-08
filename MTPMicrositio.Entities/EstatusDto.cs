using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPMicrositio.Entities
{
    public class EstatusDto
    {
        public Guid EstatusId { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public string Seccion { get; set; }
    }
}
