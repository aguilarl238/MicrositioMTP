using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MTPMicrositio.BLL;
using MTPMicrositio.Entities;
using Newtonsoft.Json;
using MTPMicrositio.Models.Web;

namespace WebApiMTPMicrositio.Controllers.Web
{
    public class ComentarioController : Controller
    {
        private readonly ComentarioBLL _bll = new ComentarioBLL();
        private static readonly HttpClient _client = new HttpClient { BaseAddress = new Uri("http://localhost:59726/") };

        private async Task PopulaCat(ComentarioViewModel m)
        {
            m.ListaProyectos = (await _bll.GetProyectosCatAsync()).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Descripcion });
            m.ListaPrioridades = (await _bll.GetPrioridadesCatAsync()).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Descripcion });
            m.ListaEstatus = (await _bll.GetEstatusCatAsync()).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Descripcion });
        }

        [HttpGet]
        public async Task<ActionResult> Index(Guid? filtrarProyectoId, Guid? filtrarPrioridadId, Guid? filtrarEstatusId)
        {
            var vm = new ComentarioFiltroViewModel
            {
                FiltrarProyectoId = filtrarProyectoId,
                FiltrarPrioridadId = filtrarPrioridadId,
                FiltrarEstatusId = filtrarEstatusId,
                ListaProyectos = (await _bll.GetProyectosCatAsync()).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Descripcion }),
                ListaPrioridades = (await _bll.GetPrioridadesCatAsync()).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Descripcion }),
                ListaEstatus = (await _bll.GetEstatusCatAsync()).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Descripcion })
            };

            var resp = await _client.GetAsync($"api/v1/comentarios/buscar?proyectoId={filtrarProyectoId}&prioridadId={filtrarPrioridadId}&estatusId={filtrarEstatusId}");
            if (resp.IsSuccessStatusCode)
            {
                var dtos = JsonConvert.DeserializeObject<List<ComentarioDto>>(await resp.Content.ReadAsStringAsync());
                vm.Comentarios = dtos.Select(d => new ComentarioViewModel { ComentarioId = d.ComentarioId, Descripcion = d.Descripcion, Activo = d.Activo, Fecha = d.Fecha }).ToList();
            }
            return View(vm);
        }

        [HttpGet]
        public async Task<ActionResult> Insertar()
        {
            var m = new ComentarioViewModel { Fecha = DateTime.Now, Activo = true };
            await PopulaCat(m);
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Insertar(ComentarioViewModel model)
        {
            model.Fecha= DateTime.Now;
            model.UsuarioId= (Guid)Session["UsuarioId"];

            if (!ModelState.IsValid) { await PopulaCat(model); return View(model); }

            var dto = new ComentarioDto { ProyectoId = model.ProyectoId, Descripcion = model.Descripcion, PrioridadId = model.PrioridadId, Activo = model.Activo, Fecha = model.Fecha, EstatusId = model.EstatusId,UsuarioId=model.UsuarioId };
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var resp = await _client.PostAsync("api/v1/comentarios/guardar", content);

            if (resp.IsSuccessStatusCode) return RedirectToAction("Index");

            await PopulaCat(model);
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Editar(Guid id)
        {
            var items = await _bll.ListarComentariosAsync(null, null, null);
            var target = items.FirstOrDefault(x => x.ComentarioId == id);
            if (target == null) return HttpNotFound();

            var m = new ComentarioViewModel { ComentarioId = target.ComentarioId, ProyectoId = target.ProyectoId, Descripcion = target.Descripcion, PrioridadId = target.PrioridadId, Activo = target.Activo, Fecha = target.Fecha, EstatusId = target.EstatusId };
            await PopulaCat(m);
            return View(m);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Editar(ComentarioViewModel model)
        {
            if (!ModelState.IsValid) { await PopulaCat(model); return View(model); }
            var dto = new ComentarioDto { ComentarioId = model.ComentarioId, ProyectoId = model.ProyectoId, Descripcion = model.Descripcion, PrioridadId = model.PrioridadId, Activo = model.Activo, Fecha = model.Fecha, EstatusId = model.EstatusId };
            await _bll.ModificarComentarioAsync(dto);
            return RedirectToAction("Index");
        }
    }
}