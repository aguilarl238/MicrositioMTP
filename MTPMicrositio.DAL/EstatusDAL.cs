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
    public class EstatusDal
    {
        private readonly string _cn = ConfigurationManager.ConnectionStrings["WebApiMTPMicrositioContext"].ConnectionString;

        public async Task<List<EstatusDto>> ObtenerPorSeccionAsync(string seccion)
        {
            var lista = new List<EstatusDto>();
            using (var conn = new SqlConnection(_cn))
            using (var cmd = new SqlCommand("dbo.sp_Estatus_SeleccionarPorSeccion", conn) { CommandType = CommandType.StoredProcedure })
            {
                cmd.Parameters.AddWithValue("@Seccion", seccion);
                await conn.OpenAsync().ConfigureAwait(false);
                using (var dr = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await dr.ReadAsync().ConfigureAwait(false))
                    {
                        lista.Add(new EstatusDto
                        {
                            EstatusId = dr.GetGuid(dr.GetOrdinal("EstatusId")),
                            Nombre = dr.GetString(dr.GetOrdinal("Nombre")),
                            Activo = dr.GetBoolean(dr.GetOrdinal("Activo")),
                            Seccion = dr.GetString(dr.GetOrdinal("Seccion"))
                        });
                    }
                }
            }
            return lista;
        }
    }
}
