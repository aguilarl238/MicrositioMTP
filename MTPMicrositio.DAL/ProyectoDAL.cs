using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MTPMicrositio.Entities;

namespace MTPMicrositio.DAL
{
    public class ProyectoDAL
    {
        private readonly string _connectionString;

        public ProyectoDAL()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["WebApiMTPMicrositioContext"]?.ConnectionString;
        }

        public async Task<List<Proyecto>> ObtenerProyectosAsync(Guid? clienteId, Guid? usuarioId)
        {
            var lista = new List<Proyecto>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.Usp_Proyectos_Seleccionar", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ClienteId", SqlDbType.UniqueIdentifier).Value = (object)clienteId ?? DBNull.Value;
                    cmd.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = (object)usuarioId ?? DBNull.Value;

                    await conn.OpenAsync();
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            lista.Add(MapearProyecto(rdr));
                        }
                    }
                }
            }
            return lista;
        }

        public async Task<Proyecto> ObtenerProyectoPorIdAsync(Guid proyectoId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.Usp_Proyectos_ObtenerPorId", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProyectoId", SqlDbType.UniqueIdentifier).Value = proyectoId;

                    await conn.OpenAsync();
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        if (await rdr.ReadAsync())
                        {
                            return MapearProyecto(rdr);
                        }
                    }
                }
            }
            return null;
        }

        public async Task<bool> InsertarProyectoAsync(Proyecto model)
        {
            int filasAfectadas = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.Usp_Proyectos_Insertar", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        AsignarParametrosComunes(cmd, model);

                        await conn.OpenAsync();
                        filasAfectadas = await cmd.ExecuteNonQueryAsync();
                        //return filasAfectadas > 0;
                    }
                }
                return filasAfectadas > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public async Task<bool> ActualizarProyectoAsync(Proyecto model)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.Usp_Proyectos_Actualizar", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProyectoId", SqlDbType.UniqueIdentifier).Value = model.ProyectoId;
                    AsignarParametrosComunes(cmd, model);

                    await conn.OpenAsync();
                    int filasAfectadas = await cmd.ExecuteNonQueryAsync();
                    return filasAfectadas > 0;
                }
            }
        }

        #region Métodos Auxiliares de Limpieza de Código (Refactorización)

        private void AsignarParametrosComunes(SqlCommand cmd, Proyecto model)
        {
            cmd.Parameters.Add("@ClienteId", SqlDbType.UniqueIdentifier).Value = (object)model.ClienteId ?? DBNull.Value;
            cmd.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = (object)model.UsuarioId ?? DBNull.Value;
            cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 200).Value = (object)model.Nombre ?? DBNull.Value;
            cmd.Parameters.Add("@Tipo", SqlDbType.NVarChar, 50).Value = (object)model.Tipo ?? DBNull.Value;
            cmd.Parameters.Add("@EstatusId", SqlDbType.UniqueIdentifier).Value = (object)model.EstatusId ?? DBNull.Value;
            cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = model.Activo;
            cmd.Parameters.Add("@FechaInicio", SqlDbType.DateTime).Value = (object)model.FechaInicio ?? DBNull.Value;
        }

        private Proyecto MapearProyecto(SqlDataReader rdr)
        {
            return new Proyecto
            {
                ProyectoId = rdr.GetGuid(rdr.GetOrdinal("ProyectoId")),
                ClienteId = rdr.IsDBNull(rdr.GetOrdinal("ClienteId")) ? (Guid?)null : rdr.GetGuid(rdr.GetOrdinal("ClienteId")),
                UsuarioId = rdr.IsDBNull(rdr.GetOrdinal("UsuarioId")) ? (Guid?)null : rdr.GetGuid(rdr.GetOrdinal("UsuarioId")),
                Nombre = rdr.IsDBNull(rdr.GetOrdinal("Nombre")) ? null : rdr.GetString(rdr.GetOrdinal("Nombre")),
                Tipo = rdr.IsDBNull(rdr.GetOrdinal("Tipo")) ? null : rdr.GetString(rdr.GetOrdinal("Tipo")),
                EstatusId = rdr.IsDBNull(rdr.GetOrdinal("EstatusId")) ? (Guid?)null : rdr.GetGuid(rdr.GetOrdinal("EstatusId")),
                Activo = rdr.IsDBNull(rdr.GetOrdinal("Activo")) ? false : rdr.GetBoolean(rdr.GetOrdinal("Activo")),
                FechaInicio = rdr.IsDBNull(rdr.GetOrdinal("FechaInicio")) ? (DateTime?)null : rdr.GetDateTime(rdr.GetOrdinal("FechaInicio"))
            };
        }

        #endregion

        #region Catálogos
        public async Task<Dictionary<Guid, string>> ObtenerCatClientesAsync()
        {
            var dic = new Dictionary<Guid, string>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT ClienteId, Nombre FROM dbo.Clientes WHERE Activo = 1", conn))
                {
                    await conn.OpenAsync();
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        while (await rdr.ReadAsync()) dic.Add(rdr.GetGuid(0), rdr.GetString(1));
                }
            }
            return dic;
        }

        public async Task<Dictionary<Guid, string>> ObtenerCatUsuariosAsync()
        {
            var dic = new Dictionary<Guid, string>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT UsuarioId, Nombre FROM dbo.Usuarios WHERE Activo = 1", conn))
                {
                    await conn.OpenAsync();
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        while (await rdr.ReadAsync()) dic.Add(rdr.GetGuid(0), rdr.GetString(1));
                }
            }
            return dic;
        }
        #endregion
    }
}