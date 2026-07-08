using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MTPMicrositio.BLL;

namespace WebApiMTPMicrositio.Controllers
{
    [RoutePrefix("api/Rol")]
    public class RolApiController : ApiController
    {
        private readonly RolBLL _bll = new RolBLL();

        [HttpGet]
        [Route("Obtener")]
        public async Task<IHttpActionResult> Obener()
        {
            var res = await _bll.ObtenerRolAsync();
            return Ok(res);
        }
    }
}
