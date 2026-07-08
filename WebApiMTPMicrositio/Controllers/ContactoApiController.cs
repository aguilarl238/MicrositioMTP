using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MTPMicrositio.BLL;
using WebApiMTPMicrositio.Models.Web;

namespace WebApiMTPMicrositio.Controllers
{
    [RoutePrefix("api/v1/contacto")]
    public class ContactoApiController : ApiController
    {
        private readonly ContactoBLL _contactoBll;

        public ContactoApiController()
        {
            _contactoBll = new ContactoBLL();
        }

        [HttpPost]
        [Route("enviar")]
        public async Task<IHttpActionResult> InsertarContactoAsync([FromBody] ContactoViewModel model)
        {
            if (model == null)
                return BadRequest("El cuerpo de la solicitud no puede estar vacío.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Procesamiento no bloqueante de la solicitud
                bool resultado = await _contactoBll.RegistrarContactoAsync(model.NombreCompleto, model.CorreoCorporativo, model.Mensaje);

                if (resultado)
                {
                    return Ok(new { Exito = true, Message = "Información registrada de manera asíncrona correctamente." });
                }

                return InternalServerError(new Exception("Error al insertar el registro en los almacenes de datos."));
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(argEx.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
