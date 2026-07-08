using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using MTPMicrositio.BLL;
using MTPMicrositio.Entities;
using MTPMicrositio.Models.Web;
using Newtonsoft.Json;

namespace WebApiMTPMicrositio.Controllers
{
    public class PortalController : Controller
    {
        private readonly ComentarioBLL _bll = new ComentarioBLL();

        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:59726/") //new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]) // ej. https://localhost:44300/
        };

        private static readonly string ApiKey = ConfigurationManager.AppSettings["ApiKey"];

        // GET: Portal
        public async Task< ActionResult> Index()
        {
            //Listar Proyectos
            Guid? filtrarClienteId = null;
            Guid filtrarUsuarioId = (Guid)Session["UsuarioId"];
            var filtroVM = new ProyectoFiltroViewModel
            {
                FiltrarUsuarioId = filtrarUsuarioId,
            };
            
            string endpoint = $"api/v1/proyectos/listar?clienteId={filtrarClienteId}&usuarioId={filtrarUsuarioId}";
            var response = await _httpClient.GetAsync(endpoint).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                filtroVM.Proyectos = JsonConvert.DeserializeObject<List<ProyectoViewModel>>(json);
            }

            foreach(var proyecto in filtroVM.Proyectos) 
            {
                Guid? filtrarProyectoId = proyecto.ProyectoId;
                Guid? filtrarPrioridadId = null;
                Guid? filtrarEstatusId = null;
                //Listar comentarios
                var vm = new ComentarioFiltroViewModel();

                var resp = await _httpClient.GetAsync($"api/v1/comentarios/buscar?proyectoId={filtrarProyectoId}&prioridadId={filtrarPrioridadId}&estatusId={filtrarEstatusId}");
                if (resp.IsSuccessStatusCode)
                {
                    var dtos = JsonConvert.DeserializeObject<List<ComentarioDto>>(await resp.Content.ReadAsStringAsync());
                    vm.Comentarios = dtos.Select(d => new ComentarioViewModel { ComentarioId = d.ComentarioId, Descripcion = d.Descripcion, Activo = d.Activo, Fecha = d.Fecha }).ToList();
                }
                proyecto.ListaComentarios = vm.Comentarios.OrderByDescending(x=>x.Fecha).Take(3);
            }

            return View(filtroVM);
        }

        public ActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        public async Task< ActionResult> Comentar(Guid id)
        {
            try
            {
                if (id == Guid.Empty) return RedirectToAction("Index", "Documentos");

                // 1. Consumir API para obtener metadatos del proyecto
                HttpResponseMessage projResponse = await _httpClient.GetAsync($"api/v1/proyectos/obtener?proyectoId={id}");
                if (!projResponse.IsSuccessStatusCode) return HttpNotFound("Proyecto no localizado.");
                var proyectoDto = JsonConvert.DeserializeObject<Proyecto>(await projResponse.Content.ReadAsStringAsync());

                // 2. Consumir API para obtener comentarios vinculados al proyecto
                HttpResponseMessage commentsResponse = await _httpClient.GetAsync($"api/v1/comentarios/buscar?proyectoId={id}&prioridadId={null}&estatusId={null}");
                var comentarios = commentsResponse.IsSuccessStatusCode
                    ? JsonConvert.DeserializeObject<List<ComentarioDto>>(await commentsResponse.Content.ReadAsStringAsync())
                    : new List<ComentarioDto>();

                // 3. Consumir API para listar documentos de este proyecto (para el DropDownList)
                HttpResponseMessage docsResponse = await _httpClient.GetAsync($"api/v1/documentos/listar?proyectoId={id}");
                var documentos = docsResponse.IsSuccessStatusCode
                    ? JsonConvert.DeserializeObject<List<DocumentoDTO>>(await docsResponse.Content.ReadAsStringAsync())
                    : new List<DocumentoDTO>();

                // Construit el ViewModel consolidado
                var model = new ProyectoComentariosViewModel
                {
                    ProyectoId = proyectoDto.ProyectoId,
                    ProyectoNombre = proyectoDto.Nombre,
                    CodigoProyecto = "PRJ-2026-QA",//proyectoDto.Tipo, // Ej: PRJ-2026-QA
                    HistorialComentarios = comentarios.OrderByDescending(c => c.Fecha).ToList(),
                    DocumentosAsociados = documentos.Select(d => new SelectListItem { Value = d.DocumentoId.ToString(), Text = d.Nombre }).ToList(),
                    ListaPrioridades = (await _bll.GetPrioridadesCatAsync()).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Descripcion }),
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al conectar con el servidor de servicios: " + ex.Message;
                return View(new ProyectoComentariosViewModel { HistorialComentarios = new List<ComentarioDto>() });
            }
            //return View();
        }


        //**Generar comentario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AgregarComentario(ProyectoComentariosViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Si el modelo es inválido, se debe rehidratar los listados de la API y regresar la misma vista
                    return RedirectToAction("Comentar", new { id = model.ProyectoId });
                }

                // Estructurar el DTO de inserción para la Web API ya generada
                var nuevoComentarioDto = new ComentarioDto
                {
                    ProyectoId = model.ProyectoId,
                    //DocumentoId = model.DocumentoVinculadoId.Value,
                    Descripcion = model.NuevoComentarioTexto,
                    Fecha = DateTime.Now,
                    UsuarioId = (Guid)Session["UsuarioId"],
                    PrioridadId=model.PrioridadId,
                    EstatusId=Guid.Parse("67CBEAFD-740E-49BD-A135-0942C33633FA")
                    // Nota: El backend de la Web API deducirá el Usuario actual mediante la sesión / Token de autenticación
                };

                string jsonContent = JsonConvert.SerializeObject(nuevoComentarioDto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Consumir la Web API existente
                HttpResponseMessage response = await _httpClient.PostAsync("api/v1/comentarios/guardar", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Comentario registrado";
                }
                else
                {
                    TempData["Error"] = "La Web API rechazó la inserción del comentario.";
                }

                return RedirectToAction("Comentar", new { id = model.ProyectoId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Excepción de comunicación: " + ex.Message;
                return RedirectToAction("Comentar", new { id = model.ProyectoId });
            }
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
    }
}