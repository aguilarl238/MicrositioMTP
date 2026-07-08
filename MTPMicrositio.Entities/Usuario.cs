using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPMicrositio.Entities
{
    public class Usuario
    {
        public Guid UsuarioId { get; set; }
        public Guid? ClienteId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

        public Guid RolId { get; set; }
        public string RolNombre { get; set; }
    }
}
