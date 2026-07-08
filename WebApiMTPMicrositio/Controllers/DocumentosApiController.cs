using System;
using System.Threading.Tasks;
using System.Web.Http;
using MTPMicrositio.BusinessLogic;
using MTPMicrositio.Entities;

namespace WebApiMTPMicrositio.Controllers
{
    [RoutePrefix("api/v1/documentos")]
    public class DocumentosApiController : ApiController
    {
        private readonly DocumentoBLL _bll = new DocumentoBLL();

        [HttpPost]
        [Route("guardar")]
        public async Task< IHttpActionResult> Guardar(DocumentoDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                Guid idGenerado = await _bll.GuardarDocumentoAsync(dto);
                return Ok(new { Success = true, DocumentoId = idGenerado });
            }
            catch (Exception ex)
            {
                // Respuesta HTTP 500 Sanitizada para API externa
                return InternalServerError(new Exception("Error crítico en el servicio de persistencia: " + ex.Message));
            }
        }

        [HttpGet]
        [Route("obtener/{id}")]
        public async Task< IHttpActionResult> Obtener(Guid id)
        {
            try
            {
                var documento = await _bll.ObtenerPorIdAsync(id);
                if (documento == null) return NotFound();
                return Ok(documento);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("listar")]
        public async Task<IHttpActionResult> Listar(Guid? clienteId = null, Guid? proyectoId = null)
        {
            var res = await _bll.ListarDocumentosAsync(clienteId, proyectoId);
            return Ok(res);
        }
    }
}