using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MTPMicrositio.Entities;
using MTPMicrositio.Models.Web;
using Newtonsoft.Json;

namespace MTPMicrositio.Web.Controllers
{
    public class ClientesController : Controller
    {
        // Reutilizable: una sola instancia de HttpClient
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:59726/") //new Uri(ConfigurationManager.AppSettings["ApiBaseUrl"]) // ej. https://localhost:44300/
        };

        private static readonly string ApiKey = "AASDASDASDASDASD";// ConfigurationManager.AppSettings["ApiKey"];

        private readonly string _apiUrl = "http://localhost:59726/api/";

        private HttpClient GetClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return client;
        }

        public async Task<ActionResult> Index()
        {
            //using (var client = GetClient())
            //{
            //    var res = await client.GetAsync(_apiUrl);
            //    var json = await res.Content.ReadAsStringAsync();
            //    var list = JsonConvert.DeserializeObject<List<ClienteViewModel>>(json);
            //    return View(list);
            //}
            var lista = new List<ClientesViewModel>();
            using (var request = new HttpRequestMessage(HttpMethod.Get, "api/clientes"))
            {
                request.Headers.Add("X-Api-Key", ApiKey);
                //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);

                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    lista = JsonConvert.DeserializeObject<List<ClientesViewModel>>(json);
                }
                else
                {
                    ViewBag.Error = $"No se pudo cargar el listado ({(int)response.StatusCode}).";
                }
            }
            return View(lista);
        }

        public ActionResult Create() => View(new ClientesViewModel());

        // GET: Clientes/Crear
        // GET: Clientes/Create
        public async Task<ActionResult> Crear()
        {
            var model = new ClientesViewModel();
            model.ListadoEstatus = await CargarComboEstatusAsync();
            return View(model);
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Crear(ClientesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListadoEstatus = await CargarComboEstatusAsync(); // Recargar si falla validación
                return View(model);
            }
            model.UsuarioCreacionID =(Guid) Session["UsuarioId"];
            using (var client = GetClient())
            {
                string json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PostAsync($"{_apiUrl}clientes", content).ConfigureAwait(false);
                if (res.IsSuccessStatusCode) return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error del servidor central al guardar.");
            model.ListadoEstatus = await CargarComboEstatusAsync();
            return View(model);
        }

        // GET: Clientes/Edit/GUID
        public async Task<ActionResult> Editar(Guid id)
        {
            using (var client = GetClient())
            {
                var res = await client.GetAsync($"{_apiUrl}clientes/{id}").ConfigureAwait(false);
                if (!res.IsSuccessStatusCode) return HttpNotFound();

                string json = await res.Content.ReadAsStringAsync().ConfigureAwait(false);
                var model = JsonConvert.DeserializeObject<ClientesViewModel>(json);

                var usuarios = new[] {
                new { UsuarioId = System.Guid.NewGuid(), NombreCompleto = "Ing. Alejandro Torres" },
                new { UsuarioId = System.Guid.NewGuid(), NombreCompleto = "Lic. Beatriz Mendoza" },
                new { UsuarioId = System.Guid.NewGuid(), NombreCompleto = "Mtro. Carlos Slim" }
            };
                model.UsuariosSeleccionadosIds = usuarios.Select(u => u.UsuarioId).ToList();


                // Carga dinámica del Combo con el elemento seleccionado por ID
                model.ListadoEstatus = await CargarComboEstatusAsync();
                return View(model);
            }
        }

        // POST: Clientes/Edit/GUID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(Guid id, ClientesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListadoEstatus = await CargarComboEstatusAsync();
                return View(model);
            }
            
            model.UsuarioCreacionID = (Guid)Session["UsuarioId"];

            using (var client = GetClient())
            {
                string json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var res = await client.PutAsync($"{_apiUrl}clientes/{id}", content).ConfigureAwait(false);
                if (res.IsSuccessStatusCode) return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Error al procesar la actualización.");
            model.ListadoEstatus = await CargarComboEstatusAsync();
            return View(model);
        }

        // POST: Clientes/Delete/GUID
        [HttpPost]
        [ValidateAntiForgeryToken] // Previene ataques CSRF desde formularios maliciosos
        public async Task<ActionResult> Delete(Guid id)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();

                // Petición HTTP DELETE hacia la API central
                var response = await client.DeleteAsync($"{_apiUrl}clientes/{id}").ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Registro eliminado con éxito." });
                }

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return Json(new { success = false, message = "El cliente ya no existe en el sistema." });
                }
            }

            return Json(new { success = false, message = "Ocurrió un error en el servidor central al intentar eliminar." });
        }
        // Clase espejo local interna para deserializar la respuesta del API de Estatus
        private class EstatusDtoSimulado { public Guid EstatusId { get; set; } public string Nombre { get; set; } }

        private async Task<IEnumerable<SelectListItem>> CargarComboEstatusAsync()
        {
            using (var client = GetClient())
            {
                var response = await client.GetAsync($"{_apiUrl}estatus/seccion/Cliente").ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var listaEstatus = JsonConvert.DeserializeObject<List<EstatusDtoSimulado>>(json);

                    var items = new List<SelectListItem> { new SelectListItem { Text = "-- Seleccione un Estatus --", Value = "" } };
                    foreach (var est in listaEstatus)
                    {
                        items.Add(new SelectListItem { Text = est.Nombre, Value = est.EstatusId.ToString() });
                    }
                    return items;
                }
            }
            return new List<SelectListItem>();
        }
    }
}