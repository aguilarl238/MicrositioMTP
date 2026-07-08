using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTPMicrositio.Entities;

namespace MTPMicrositio.DAL
{
    public  class RolDal
    {
        private readonly string _cn = ConfigurationManager.ConnectionStrings["WebApiMTPMicrositioContext"].ConnectionString;
        public async Task<List<RolDto>> ObtenerRolAsync()
        {
            var lista = new List<RolDto>();
            using (var conn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_Rol_Select", conn) { CommandType = CommandType.StoredProcedure })
            {

                await conn.OpenAsync().ConfigureAwait(false);
                using (var dr = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await dr.ReadAsync().ConfigureAwait(false))
                    {
                        lista.Add(new RolDto
                        {
                            RolId = dr.GetGuid(dr.GetOrdinal("RolId")),
                            Nombre = dr.GetString(dr.GetOrdinal("Nombre")),
                            Activo = dr.GetBoolean(dr.GetOrdinal("Activo"))
                        });
                    }
                }
            }
            return lista;
        }
    }
}
