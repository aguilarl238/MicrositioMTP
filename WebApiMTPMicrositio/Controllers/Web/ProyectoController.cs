using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MTPMicrositio.BLL;
using MTPMicrositio.Models.Web;
using Newtonsoft.Json;

namespace MTPMicrositio.Controllers
{
    public class ProyectoController : Controller
    {
        private readonly HttpClient _client;
        private readonly ProyectoBLL _bll;
        private readonly string _apiUrl = "http://localhost:59726/api/";
        //static ProyectoController()
        //{
        //    _client = new HttpClient { BaseAddress = new Uri("http://localhost:59726/") };
        //    _client.DefaultRequestHeaders.Accept.Clear();
        //    _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //}

        public ProyectoController()
        {
            _bll = new ProyectoBLL(); // Usado para rellenar catálogos locales rápidamente
            _client = new HttpClient { BaseAddress = new Uri("http://localhost:59726/") };
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Pantalla Principal: Index con Filtros por ClienteID y UsuarioID
        [HttpGet]
        public async Task<ActionResult> Index(Guid? filtrarClienteId, Guid? filtrarUsuarioId)
        {
            var filtroVM = new ProyectoFiltroViewModel
            {
                FiltrarClienteId = filtrarClienteId,
                FiltrarUsuarioId = filtrarUsuarioId,
                ListaClientes = await _bll.GetSelectClientesAsync(),
                ListaUsuarios = await _bll.GetSelectUsuariosAsync()
            };

            string endpoint = $"api/v1/proyectos/listar?clienteId={filtrarClienteId}&usuarioId={filtrarUsuarioId}";
            var response = await _client.GetAsync(endpoint).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                filtroVM.Proyectos = JsonConvert.DeserializeObject<List<ProyectoViewModel>>(json);
            }
            return View(filtroVM);
        }

        [HttpGet]
        public async Task<ActionResult> Insertar()
        {
            var model = new ProyectoViewModel
            {
                ListaClientes = await _bll.GetSelectClientesAsync(),
                ListaUsuarios = await _bll.GetSelectUsuariosAsync(),
                ListaEstatus = await CargarComboEstatusAsync(),
                FechaInicio = DateTime.Now
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Insertar(ProyectoViewModel model)
        {
             if (!ModelState.IsValid)
            {
                model.ListaClientes = await _bll.GetSelectClientesAsync();
                model.ListaUsuarios = await _bll.GetSelectUsuariosAsync();
                return View(model);
            }

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("api/v1/proyectos/crear", content).ConfigureAwait(false); ;
            if (response.IsSuccessStatusCode) return RedirectToAction("Index");

            ModelState.AddModelError("", "Error del servicio al insertar el proyecto.");
            model.ListaClientes = await _bll.GetSelectClientesAsync();
            model.ListaUsuarios = await _bll.GetSelectUsuariosAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Editar(Guid id)
        {
            var model = await _bll.ObtenerPorIdAsync(id);
            if (model == null) return HttpNotFound();

            string json = JsonConvert.SerializeObject(model); //await res.Content.ReadAsStringAsync().ConfigureAwait(false);
            var model1 = JsonConvert.DeserializeObject<ProyectoViewModel>(json);

            model1.ListaClientes = await _bll.GetSelectClientesAsync();
            model1.ListaUsuarios = await _bll.GetSelectUsuariosAsync();
            model1.ListaEstatus = await CargarComboEstatusAsync();
            return View(model1);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(ProyectoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.ListaClientes = await _bll.GetSelectClientesAsync();
                model.ListaUsuarios = await _bll.GetSelectUsuariosAsync();
                model.ListaEstatus = await CargarComboEstatusAsync();
                return View(model);
            }

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync("api/v1/proyectos/actualizar", content).ConfigureAwait(false); ;
            if (response.IsSuccessStatusCode) return RedirectToAction("Index");

            ModelState.AddModelError("", "Error del servicio al actualizar el proyecto.");
            model.ListaClientes = await _bll.GetSelectClientesAsync();
            model.ListaUsuarios = await _bll.GetSelectUsuariosAsync();
            model.ListaEstatus = await CargarComboEstatusAsync();
            return View(model);
        }
        private class EstatusDtoSimulado { public Guid EstatusId { get; set; } public string Nombre { get; set; } }
        private async Task<IEnumerable<SelectListItem>> CargarComboEstatusAsync()
        {
            using (var client = _client)
            {
                var response = await client.GetAsync($"{_apiUrl}estatus/seccion/Proyecto").ConfigureAwait(false);
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