using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MTPMicrositio.Entities;
using MTPMicrositio.BLL;
using MTPMicrositio.Models.Web;

namespace WebApiMTPMicrositio.Controllers
{
    [RoutePrefix("api/v1/proyectos")]
    public class ProyectosApiController : ApiController
    {
        private readonly ProyectoBLL _bll;

        public ProyectosApiController()
        {
            _bll = new ProyectoBLL();
        }

        [HttpGet]
        [Route("listar")]
        public async Task<IHttpActionResult> Listar(Guid? clienteId = null, Guid? usuarioId = null)
        {
            var res = await _bll.ListarProyectosAsync(clienteId, usuarioId);
            return Ok(res);
        }

        [HttpGet]
        [Route("obtener")]
        public async Task<IHttpActionResult> Obtener(Guid proyectoId)
        {
            var res = await _bll.ObtenerPorIdAsync(proyectoId);
            return Ok(res);
        }

        [HttpPost]
        [Route("crear")]
        public async Task<IHttpActionResult> Crear([FromBody] Proyecto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            bool ok = await _bll.GuardarProyectoAsync(model);
            if (ok) return Ok(new { Exito = true });
            return InternalServerError();
        }

        [HttpPut]
        [Route("actualizar")]
        public async Task<IHttpActionResult> Actualizar([FromBody] Proyecto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            bool ok = await _bll.ModificarProyectoAsync(model);
            if (ok) return Ok(new { Exito = true });
            return InternalServerError();
        }
    }
}
