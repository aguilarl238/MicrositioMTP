using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTPMicrositio.DAL;
using MTPMicrositio.Entities;

namespace MTPMicrositio.BLL
{
    public class ComentarioBLL
    {
        private readonly ComentarioDAL _dal;

        public ComentarioBLL() { _dal = new ComentarioDAL(); }

        public async Task<List<ComentarioDto>> ListarComentariosAsync(Guid? pId, Guid? prId, Guid? eId)
        {
            return await _dal.ListarComentariosAsync(pId, prId, eId);
        }

        public async Task<Guid> RegistrarComentarioAsync(ComentarioDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Descripcion)) throw new ArgumentException("La descripción es requerida.");
            dto.Fecha = DateTime.Now;
            return await _dal.InsertarComentarioAsync(dto);
        }

        public async Task<bool> ModificarComentarioAsync(ComentarioDto dto)
        {
            return await _dal.ActualizarComentarioAsync(dto);
        }

        public async Task<List<CatalogoDto>> GetProyectosCatAsync() => await _dal.ObtenerCatalogoAsync("dbo.Proyectos", "ProyectoId", "Nombre");
        public async Task<List<CatalogoDto>> GetPrioridadesCatAsync() => await _dal.ObtenerCatalogoAsync("dbo.PrioridadComentario", "PrioridadId", "Nombre");
        public async Task<List<CatalogoDto>> GetEstatusCatAsync() => await _dal.ObtenerCatalogoAsync("dbo.Estatus", "EstatusId", "Nombre");

        public async Task<List<CatalogoDto>> GetTipoDocumentoCatAsync() => await _dal.ObtenerCatalogoAsync("dbo.TipoDocumento", "TipoDocumentoId", "Nombre");
    }
}
