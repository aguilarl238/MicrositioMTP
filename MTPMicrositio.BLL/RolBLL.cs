using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTPMicrositio.DAL;
using MTPMicrositio.Entities;

namespace MTPMicrositio.BLL
{
    public class RolBLL
    {
        private readonly RolDal _dal = new RolDal();

        public async Task<List<RolDto>> ObtenerRolAsync() => await _dal.ObtenerRolAsync();
    }
}
