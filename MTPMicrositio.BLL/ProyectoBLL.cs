using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MTPMicrositio.Entities;
using MTPMicrositio.DAL;
using System.Web.Mvc;
namespace MTPMicrositio.BLL
{
    public class ProyectoBLL
    {
        private readonly ProyectoDAL _dal;

        public ProyectoBLL()
        {
            _dal = new ProyectoDAL();
        }

        public async Task<List<Proyecto>> ListarProyectosAsync(Guid? clienteId, Guid? usuarioId)
        {
            return await _dal.ObtenerProyectosAsync(clienteId, usuarioId);
        }

        public async Task<Proyecto> ObtenerPorIdAsync(Guid id)
        {
            return await _dal.ObtenerProyectoPorIdAsync(id);
        }

        public async Task<bool> GuardarProyectoAsync(Proyecto  model)
        {
            if (string.IsNullOrWhiteSpace(model.Nombre)) throw new ArgumentException("El nombre del proyecto es inválido.");
            return await _dal.InsertarProyectoAsync(model);
        }

        public async Task<bool> ModificarProyectoAsync(Proyecto model)
        {
            return await _dal.ActualizarProyectoAsync(model);
        }

        public async Task<IEnumerable<SelectListItem>> GetSelectClientesAsync()
        {
            var datos = await _dal.ObtenerCatClientesAsync();
            var list = new List<SelectListItem>();
            foreach (var item in datos) list.Add(new SelectListItem { Value = item.Key.ToString(), Text = item.Value });
            return list;
        }

        public async Task<IEnumerable<SelectListItem>> GetSelectUsuariosAsync()
        {
            var datos = await _dal.ObtenerCatUsuariosAsync();
            var list = new List<SelectListItem>();
            foreach (var item in datos) list.Add(new SelectListItem { Value = item.Key.ToString(), Text = item.Value });
            return list;
        }
    }
}