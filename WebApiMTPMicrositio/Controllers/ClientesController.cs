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
    [RoutePrefix("api/clientes")]
    public class ClientesController : ApiController
    {
        private readonly ClienteBLL _clienteBLL;

        public ClientesController()
        {
            _clienteBLL = new ClienteBLL();
        }

        // GET api/clientes
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            List<Cliente> clientes = await _clienteBLL.ObtenerTodosAsync();
            return Ok(clientes);
        }

        // GET api/clientes/{id}
        [HttpGet, Route("{id:guid}")]
        public async Task<IHttpActionResult> GetById(Guid id)
        {
            Cliente cliente = await _clienteBLL.ObtenerPorIdAsync(id);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        // POST api/clientes
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Crear([FromBody] Cliente cliente)
        {
            try
            {
                Guid nuevoId = await _clienteBLL.RegistrarClienteAsync(cliente);
                return Created($"api/clientes/{nuevoId}", new { ClienteId = nuevoId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PUT api/clientes/{id}
        [HttpPut, Route("{id:guid}")]
        public async Task<IHttpActionResult> Actualizar(Guid id, [FromBody] Cliente cliente)
        {
            try
            {
                cliente.ClienteId = id; // aseguramos el id de la ruta
                bool actualizado = await _clienteBLL.ActualizarClienteAsync(cliente);

                if (!actualizado)
                    return NotFound();

                return Ok(new { mensaje = "Cliente actualizado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/clientes/{id}
        [HttpDelete, Route("{id:guid}")]
        public async Task<IHttpActionResult> Eliminar(Guid id)
        {
            try
            {
                bool eliminado = await _clienteBLL.EliminarClienteAsync(id);

                if (!eliminado)
                    return NotFound();

                return Ok(new { mensaje = "Cliente eliminado correctamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
