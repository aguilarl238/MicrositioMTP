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
    [RoutePrefix("api/estatus")]
    public class EstatusApiController : ApiController
    {
        private readonly EstatusBll _bll = new EstatusBll();

        [HttpGet]
        [Route("seccion/{seccion}")]
        public async Task<IHttpActionResult> GetBySeccion(string seccion)
        {
            var res = await _bll.ObtenerPorSeccionAsync(seccion);
            return Ok(res);
        }
    }
}