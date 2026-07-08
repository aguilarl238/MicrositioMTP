using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPMicrositio.Entities
{
    public class DocumentoDTO
    {
        public Guid DocumentoId { get; set; }
        public Guid? ClienteId { get; set; }
        public Guid? TipoDocumentoId { get; set; }
        public string Nombre { get; set; }
        public bool Activo { get; set; }
        public Guid? EstatusId { get; set; }
        public DateTime? FechaVigencia { get; set; }
        public Guid? ProyectoId { get; set; }
        public string ArchivoBase64 { get; set; }
        public string ExtensionArchivo { get; set; }
        public string UrlDoc { get; set; }
        public Guid? UsuarioId { get; set; }

        // Propiedades de navegación de solo lectura para listados
        public string ClienteNombre { get; set; }
        public string TipoDocumentoNombre { get; set; }
        public string EstatusNombre { get; set; }
        public string ProyectoNombre { get; set; }
    }
}
