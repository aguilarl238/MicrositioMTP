// Controllers/UsuariosController.cs
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using MTPMicrositio.BLL;
using MTPMicrositio.Entities;

namespace WebApiMTPMicrositio.Controllers
{
    [RoutePrefix("api/usuarios")]
    public class UsuariosController : ApiController
    {
        private readonly UsuarioBLL _usuarioBLL;

        public UsuariosController()
        {
            _usuarioBLL = new UsuarioBLL();
        }

        // GET api/usuarios
        [HttpGet, Route("")]
        public async Task<IHttpActionResult> GetAll()
        {
            List<Usuario> usuarios = await _usuarioBLL.ObtenerTodosAsync();
            return Ok(usuarios);
        }

        // GET api/usuarios/All
        [HttpGet, Route("All")]
        public async Task<IHttpActionResult> All()
        {
            // Simulación de obtención desde BLL
            var usuarios = new[] {
                new { UsuarioId = System.Guid.NewGuid(), NombreCompleto = "Ing. Alejandro Torres" },
                new { UsuarioId = System.Guid.NewGuid(), NombreCompleto = "Lic. Beatriz Mendoza" },
                new { UsuarioId = System.Guid.NewGuid(), NombreCompleto = "Mtro. Carlos Slim" }
            };

            return Ok(usuarios);
        }

        // GET api/usuarios/{id}
        [HttpGet, Route("{id:guid}")]
        public async Task<IHttpActionResult> GetById(Guid id)
        {
            Usuario usuario = await _usuarioBLL.ObtenerPorIdAsync(id);
            if (usuario == null)
                return NotFound();

            return Ok(usuario);
        }

        // POST api/usuarios
        [HttpPost, Route("")]
        public async Task<IHttpActionResult> Crear([FromBody] UsuarioCrearRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuario = new Usuario
                {
                    ClienteId = request.ClienteId,
                    Email = request.Email,
                    Nombre = request.Nombre,
                    RolId = request.RolId
                };

                Usuario creado = await _usuarioBLL.CrearAsync(usuario, request.Password);
                return CreatedAtRoute("DefaultApi", new { id = creado.UsuarioId }, creado);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Content(System.Net.HttpStatusCode.Conflict, ex.Message);
            }
        }

        // PUT api/usuarios/{id}
        [HttpPut, Route("{id:guid}")]
        public async Task<IHttpActionResult> Actualizar(Guid id, [FromBody] Usuario usuario)
        {
            if (usuario == null)
                return BadRequest("Datos del usuario requeridos.");

            usuario.UsuarioId = id;

            try
            {
                bool actualizado = await _usuarioBLL.ActualizarAsync(usuario);
                if (!actualizado)
                    return NotFound();

                return Ok(usuario);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // PATCH api/usuarios/{id}/password
        [HttpPatch, Route("{id:guid}/password")]
        public async Task<IHttpActionResult> CambiarPassword(Guid id, [FromBody] CambiarPasswordRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("La contraseña es requerida.");

            bool ok = await _usuarioBLL.CambiarPasswordAsync(id, request.Password);
            if (!ok)
                return NotFound();

            return Ok(new { UsuarioId = id, Mensaje = "Contraseña actualizada." });
        }

        // DELETE api/usuarios/{id}
        [HttpDelete, Route("{id:guid}")]
        public async Task<IHttpActionResult> Eliminar(Guid id)
        {
            bool eliminado = await _usuarioBLL.EliminarAsync(id);
            if (!eliminado)
                return NotFound();

            return StatusCode(System.Net.HttpStatusCode.NoContent);
        }

        // POST api/usuarios/login
        [HttpPost, Route("login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email)
                || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email y contraseña son requeridos.");

            Usuario usuario = await _usuarioBLL.ValidarCredencialesAsync(request.Email, request.Password);
            if (usuario == null)
                return Unauthorized();

            // No exponer el PasswordHash en la respuesta
            return Ok(new
            {
                usuario.UsuarioId,
                usuario.ClienteId,
                usuario.Email,
                usuario.Nombre,
                usuario.Activo
            });
        }
    }

    // DTOs de request (no exponen el PasswordHash)
    public class UsuarioCrearRequest
    {
        public Guid? ClienteId { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }

        public Guid RolId { get; set; }
    }

    public class CambiarPasswordRequest
    {
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}