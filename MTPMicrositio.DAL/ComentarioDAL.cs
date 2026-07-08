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
    public class ComentarioDAL
    {
        private readonly string _connectionString;

        public ComentarioDAL()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["WebApiMTPMicrositioContext"]?.ConnectionString;
        }

        public async Task<List<ComentarioDto>> ListarComentariosAsync(Guid? proyectoId, Guid? prioridadId, Guid? estatusId)
        {
            var lista = new List<ComentarioDto>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_Comentario_Select", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ProyectoId", SqlDbType.UniqueIdentifier).Value = (object)proyectoId ?? DBNull.Value;
                    cmd.Parameters.Add("@PrioridadId", SqlDbType.UniqueIdentifier).Value = (object)prioridadId ?? DBNull.Value;
                    cmd.Parameters.Add("@EstatusId", SqlDbType.UniqueIdentifier).Value = (object)estatusId ?? DBNull.Value;

                    await conn.OpenAsync();
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                    {
                        while (await rdr.ReadAsync())
                        {
                            lista.Add(new ComentarioDto
                            {
                                ComentarioId = rdr.GetGuid(0),
                                ProyectoId = rdr.IsDBNull(1) ? (Guid?)null : rdr.GetGuid(1),
                                UsuarioId = rdr.IsDBNull(2) ? (Guid?)null : rdr.GetGuid(2),
                                Descripcion = rdr.IsDBNull(3) ? null : rdr.GetString(3),
                                PrioridadId = rdr.IsDBNull(4) ? (Guid?)null : rdr.GetGuid(4),
                                Activo = rdr.IsDBNull(5) ? false : rdr.GetBoolean(5),
                                Fecha = rdr.IsDBNull(6) ? (DateTime?)null : rdr.GetDateTime(6),
                                EstatusId = rdr.IsDBNull(7) ? (Guid?)null : rdr.GetGuid(7)
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public async Task<Guid> InsertarComentarioAsync(ComentarioDto dto)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("dbo.sp_Comentario_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@ProyectoId", SqlDbType.UniqueIdentifier).Value = (object)dto.ProyectoId ?? DBNull.Value;
                        cmd.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = (object)dto.UsuarioId ?? DBNull.Value;
                        cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, -1).Value = (object)dto.Descripcion ?? DBNull.Value;
                        cmd.Parameters.Add("@PrioridadId", SqlDbType.UniqueIdentifier).Value = (object)dto.PrioridadId ?? DBNull.Value;
                        cmd.Parameters.Add("@EstatusId", SqlDbType.UniqueIdentifier).Value = (object)dto.EstatusId ?? DBNull.Value;

                        // CONFIGURACIÓN DEL PARAMETRO OUTPUT REQUERIDO
                        SqlParameter outputParam = new SqlParameter("@ComentarioId", SqlDbType.UniqueIdentifier)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputParam);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();

                        return (Guid)outputParam.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> ActualizarComentarioAsync(ComentarioDto dto)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dbo.sp_Comentario_Update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@ComentarioId", SqlDbType.UniqueIdentifier).Value = dto.ComentarioId;
                    cmd.Parameters.Add("@ProyectoId", SqlDbType.UniqueIdentifier).Value = (object)dto.ProyectoId ?? DBNull.Value;
                    cmd.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = (object)dto.UsuarioId ?? DBNull.Value;
                    cmd.Parameters.Add("@Descripcion", SqlDbType.NVarChar, -1).Value = (object)dto.Descripcion ?? DBNull.Value;
                    cmd.Parameters.Add("@PrioridadId", SqlDbType.UniqueIdentifier).Value = (object)dto.PrioridadId ?? DBNull.Value;
                    cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = dto.Activo;
                    cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = (object)dto.Fecha ?? DBNull.Value;
                    cmd.Parameters.Add("@EstatusId", SqlDbType.UniqueIdentifier).Value = (object)dto.EstatusId ?? DBNull.Value;

                    await conn.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync() > 0;
                }
            }
        }

        // Simulación de carga de diccionarios para catálogos
        public async Task<List<CatalogoDto>> ObtenerCatalogoAsync(string tabla, string idCol, string descCol)
        {
            var list = new List<CatalogoDto>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                string query = $"SELECT {idCol}, {descCol} FROM {tabla} WHERE Activo = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    await conn.OpenAsync();
                    using (SqlDataReader rdr = await cmd.ExecuteReaderAsync())
                        while (await rdr.ReadAsync())
                            list.Add(new CatalogoDto { Id = rdr.GetGuid(0), Descripcion = rdr.GetString(1) });
                }
            }
            return list;
        }
    }
}
