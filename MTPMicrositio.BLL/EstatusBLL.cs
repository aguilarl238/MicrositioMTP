using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTPMicrositio.DAL;
using MTPMicrositio.Entities;

namespace MTPMicrositio.BLL
{
    public class EstatusBll
    {
        private readonly EstatusDal _dal = new EstatusDal();
        public async Task<List<EstatusDto>> ObtenerPorSeccionAsync(string seccion) => await _dal.ObtenerPorSeccionAsync(seccion);
    }
}
