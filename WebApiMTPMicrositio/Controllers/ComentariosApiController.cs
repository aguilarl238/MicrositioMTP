using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MTPMicrositio.BLL;
using MTPMicrositio.Entities;

namespace WebApiMTPMicrositio.Controllers
{
    [RoutePrefix("api/v1/comentarios")]
    public class ComentariosApiController : ApiController
    {
        private readonly ComentarioBLL _bll = new ComentarioBLL();

        [HttpGet]
        [Route("buscar")]
        public async Task<IHttpActionResult> Buscar(Guid? proyectoId = null, Guid? prioridadId = null, Guid? estatusId = null)
        {
            var res = await _bll.ListarComentariosAsync(proyectoId, prioridadId, estatusId);
            return Ok(res);
        }

        [HttpPost]
        [Route("guardar")]
        public async Task<IHttpActionResult> Guardar([FromBody] ComentarioDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            Guid nuevoId = await _bll.RegistrarComentarioAsync(dto);
            return Ok(new { Exito = true, ComentarioId = nuevoId });
        }
    }
}
