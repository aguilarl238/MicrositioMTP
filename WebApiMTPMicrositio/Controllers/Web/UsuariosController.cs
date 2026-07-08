using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using MTPMicrositio.Entities;
using MTPMicrositio.Models.Web;
using Newtonsoft.Json;

namespace MTPMicrositio.Controllers.Web
{
    public class UsuariosController : Controller
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:59726/") //new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]) // ej. https://localhost:44300/
        };

        private static readonly string ApiKey = ConfigurationManager.AppSettings["ApiKey"];


        private HttpClient GetClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }
        // GET: Usuarios/Crear
        [HttpGet]
        public async Task <ActionResult> Crear()
        {
            UsuarioViewModel viewModel = new UsuarioViewModel();
            viewModel.ListadoRol = await CargarComboRolAsync("Rol");// Cargar combo de roles
            viewModel.ListadoClientes= await CargarComboRolAsync("Cli");// Cargar combo de Clientes
            return View(viewModel);
        }

        // POST: Usuarios/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(UsuarioViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Payload que espera el API (UsuarioCrearRequest)
            var payload = new
            {
                ClienteId = model.ClienteId,
                Email = model.Email,
                Nombre = model.Nombre,
                Password = model.Password,
                RolId = model.RolId
            };
            var json = JsonConvert.SerializeObject(payload);

            using (var request = new HttpRequestMessage(HttpMethod.Post, "api/usuarios"))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Add("X-Api-Key", ApiKey);
                //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "Usuario creado correctamente.";
                    return RedirectToAction("Crear");
                }

                string error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty,
                    $"Error al crear el usuario ({(int)response.StatusCode}): {error}");
            }

            return View(model);
        }

        // GET: Usuarios  (listado con buscador)
        [HttpGet]
        public async Task<ActionResult> Index(string buscar = null, bool? activo = null)
        {
            var lista = new List<Usuario>();

            using (var request = new HttpRequestMessage(HttpMethod.Get, "api/usuarios"))
            {
                request.Headers.Add("X-Api-Key", ApiKey);
                //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    lista = JsonConvert.DeserializeObject<List<Usuario>>(json);
                }
                else
                {
                    ViewBag.Error = $"No se pudo cargar el listado ({(int)response.StatusCode}).";
                }
            }

            // Filtro por texto (nombre o email)
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                string termino = buscar.Trim().ToLowerInvariant();
                lista = lista.Where(u =>
                    (!string.IsNullOrEmpty(u.Nombre) && u.Nombre.ToLowerInvariant().Contains(termino)) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLowerInvariant().Contains(termino)))
                    .ToList();
            }

            // Filtro por estatus
            if (activo.HasValue)
                lista = lista.Where(u => u.Activo == activo.Value).ToList();

            ViewBag.Buscar = buscar;
            ViewBag.Activo = activo;
            return View(lista);
        }

        // GET: Usuarios/Editar/{id}
        [HttpGet]
        public async Task<ActionResult> Editar(Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/usuarios/{id}"))
            {
                request.Headers.Add("X-Api-Key", ApiKey);
                //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return HttpNotFound();

                string json = await response.Content.ReadAsStringAsync();
                Usuario usuario = JsonConvert.DeserializeObject<Usuario>(json);

                var model = new UsuarioEditarViewModel
                {
                    UsuarioId = usuario.UsuarioId,
                    ClienteId = usuario.ClienteId,
                    Email = usuario.Email,
                    Nombre = usuario.Nombre,
                    Activo = usuario.Activo,
                    RolId = usuario.RolId,
                };
                model.ListadoRol = await CargarComboRolAsync("Rol"); // Cargar combo de roles
                model.ListadoClientes = await CargarComboRolAsync("Cli");// Cargar combo de Clientes
                return View(model);
            }
        }

        // POST: Usuarios/Editar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(UsuarioEditarViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Payload que espera el PUT api/usuarios/{id} (entidad Usuario)
            var payload = new
            {
                UsuarioId = model.UsuarioId,
                ClienteId = model.ClienteId,
                Email = model.Email,
                Nombre = model.Nombre,
                Activo = model.Activo, RolId = model.RolId,
            };

            var json = JsonConvert.SerializeObject(payload);

            using (var request = new HttpRequestMessage(HttpMethod.Put, $"api/usuarios/{model.UsuarioId}"))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Add("X-Api-Key", ApiKey);
                //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "Usuario actualizado correctamente.";
                    return RedirectToAction("Index");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return HttpNotFound();

                string error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty,
                    $"Error al actualizar ({(int)response.StatusCode}): {error}");
            }

            return View(model);
        }

        // POST: Usuarios/Eliminar/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Eliminar(Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Delete, $"api/usuarios/{id}"))
            {
                request.Headers.Add("X-Api-Key", ApiKey);
                //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                    TempData["Mensaje"] = "Usuario eliminado correctamente.";
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    TempData["Error"] = "El usuario no existe o ya fue eliminado.";
                else
                    TempData["Error"] = $"No se pudo eliminar el usuario ({(int)response.StatusCode}).";
            }

            return RedirectToAction("Index");
        }

        // GET: Usuarios/CambiarPassword/{id}
        [HttpGet]
        public async Task<ActionResult> CambiarPassword(Guid id)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/usuarios/{id}"))
            {
                request.Headers.Add("X-Api-Key", ApiKey);
                //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return HttpNotFound();

                string json = await response.Content.ReadAsStringAsync();
                Usuario usuario = JsonConvert.DeserializeObject<Usuario>(json);

                var model = new CambiarPasswordViewModel
                {
                    UsuarioId = usuario.UsuarioId,
                    Nombre = usuario.Nombre
                };

                return View(model);
            }
        }

        // POST: Usuarios/CambiarPassword/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CambiarPassword(CambiarPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Payload que espera PATCH api/usuarios/{id}/password (CambiarPasswordRequest)
            var payload = new { Password = model.Password };
            var json = JsonConvert.SerializeObject(payload);

            using (var request = new HttpRequestMessage(
                new HttpMethod("PATCH"), $"api/usuarios/{model.UsuarioId}/password"))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                request.Headers.Add("X-Api-Key", ApiKey);
                //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "Contraseña actualizada correctamente.";
                    return RedirectToAction("Index");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return HttpNotFound();

                string error = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty,
                    $"Error al cambiar la contraseña ({(int)response.StatusCode}): {error}");
            }

            return View(model);
        }

        // GET: Usuarios/Login
        [HttpGet]
        public ActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        // POST: Usuarios/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var payload = new { Email = model.Email, Password = model.Password };
            var json = JsonConvert.SerializeObject(payload);

            using (var request = new HttpRequestMessage(HttpMethod.Post, "api/usuarios/login"))
            {
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
                //request.Headers.Add("X-Api-Key", ApiKey);
                //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync();
                    Usuario usuario = JsonConvert.DeserializeObject<Usuario>(body);

                    // Cookie de autenticación de Forms
                    FormsAuthentication.SetAuthCookie(usuario.Email, false);
                    Session["UsuarioId"] = usuario.UsuarioId;
                    Session["NombreUsuario"] = usuario.Nombre;

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Portal");
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
                else
                    ModelState.AddModelError(string.Empty,
                        $"Error al iniciar sesión ({(int)response.StatusCode}).");
            }
            //return RedirectToAction("Login", "Portal");
            return View(model);
        }


        private class RolDtoSimulado { public Guid RolId { get; set; } public string Nombre { get; set; } }

        private async Task<IEnumerable<SelectListItem>> CargarComboRolAsync(string tipo)
        {
            string uri=String.Empty;
            switch (tipo)
            {
                case "Rol":
                    uri = "api/Rol/Obtener";
                    break;
                case "Cli":
                    uri = "api/Clientes";
                break;        
            }
            using (var request = new HttpRequestMessage(HttpMethod.Get, uri))
            {
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {


                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    //Si es cliente, obtener la lista
                    if (tipo=="Cli")
                    {
                        var listacli = JsonConvert.DeserializeObject<List<Cliente>>(json);
                        //de la lista obtenida, tomar los dos campos id, nombre

                        var j = from p in listacli select new { RolId = p.ClienteId, Nombre = p.Nombre };

                        //pasar la list a json
                        json = JsonConvert.SerializeObject(j);
                    }

                    var listaEstatus = JsonConvert.DeserializeObject<List<RolDtoSimulado>>(json);

                    var items = new List<SelectListItem>();// { new SelectListItem { Text = "-- Seleccione un Estatus --", Value = "" } };
                    foreach (var est in listaEstatus)
                    {
                        items.Add(new SelectListItem { Text = est.Nombre, Value = est.RolId.ToString() });
                    }
                    return items;
                }
            }
            return new List<SelectListItem>();
        }
        // GET: Usuarios/Logout
        [HttpGet]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "Usuarios");
        }
    }
}