// DAL/UsuarioDAL.cs
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MTPMicrositio.Entities;

namespace MTPMicrositio.DAL
{
    public class UsuarioDAL
    {
        public async Task<List<Usuario>> ObtenerTodosAsync()
        {
            var lista = new List<Usuario>();

            using (SqlConnection cn = Conexion.ObtenerConexion())
            using (var cmd = new SqlCommand("sp_Usuario_ObtenerTodos", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                await cn.OpenAsync().ConfigureAwait(false);

                using (var dr = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await dr.ReadAsync().ConfigureAwait(false))
                        lista.Add(Mapear(dr));
                }
            }
            return lista;
        }

        public async Task<Usuario> ObtenerPorIdAsync(Guid usuarioId)
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            using (var cmd = new SqlCommand("sp_Usuario_ObtenerPorId", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = usuarioId;
                await cn.OpenAsync().ConfigureAwait(false);

                using (var dr = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (await dr.ReadAsync().ConfigureAwait(false))
                        return Mapear(dr);
                }
            }
            return null;
        }

        public async Task<Usuario> ObtenerPorEmailAsync(string email)
        {
            try
            {
                using (SqlConnection cn = Conexion.ObtenerConexion())
                using (var cmd = new SqlCommand("sp_Usuario_ObtenerPorEmail", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value = email;
                    await cn.OpenAsync().ConfigureAwait(false);

                    using (var dr = await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await dr.ReadAsync().ConfigureAwait(false))
                            return Mapear(dr);
                    }
                }
                return null;
            }
            catch (Exception ex) {
                throw new Exception("");
                return null;
            }
        } 

        public async Task<Guid> InsertarAsync(Usuario usuario)
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            using (var cmd = new SqlCommand("sp_Usuario_Insertar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                var idParam = new SqlParameter("@UsuarioId", SqlDbType.UniqueIdentifier)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(idParam);
                cmd.Parameters.Add("@ClienteId", SqlDbType.UniqueIdentifier).Value =
                    (object)usuario.ClienteId ?? DBNull.Value;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value =
                    (object)usuario.Email ?? DBNull.Value;
                cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 500).Value =
                    (object)usuario.PasswordHash ?? DBNull.Value;
                cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 200).Value =
                    (object)usuario.Nombre ?? DBNull.Value;
                cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = usuario.Activo;
                cmd.Parameters.Add("@RolId", SqlDbType.UniqueIdentifier).Value =
                    (object)usuario.RolId ?? DBNull.Value;
                await cn.OpenAsync().ConfigureAwait(false);
                await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                return (Guid)idParam.Value;
            }
        }

        public async Task<bool> ActualizarAsync(Usuario usuario)
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            using (var cmd = new SqlCommand("sp_Usuario_Actualizar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = usuario.UsuarioId;
                cmd.Parameters.Add("@ClienteId", SqlDbType.UniqueIdentifier).Value =
                    (object)usuario.ClienteId ?? DBNull.Value;
                cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 150).Value =
                    (object)usuario.Email ?? DBNull.Value;
                cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 200).Value =
                    (object)usuario.Nombre ?? DBNull.Value;
                cmd.Parameters.Add("@Activo", SqlDbType.Bit).Value = usuario.Activo;
                cmd.Parameters.Add("@RolId", SqlDbType.UniqueIdentifier).Value =
                   (object)usuario.RolId ?? DBNull.Value;

                await cn.OpenAsync().ConfigureAwait(false);
                int filas = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                return filas > 0;
            }
        }

        public async Task<bool> CambiarPasswordAsync(Guid usuarioId, string passwordHash)
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            using (var cmd = new SqlCommand("sp_Usuario_CambiarPassword", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = usuarioId;
                cmd.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 500).Value = passwordHash;

                await cn.OpenAsync().ConfigureAwait(false);
                int filas = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                return filas > 0;
            }
        }

        public async Task<bool> EliminarAsync(Guid usuarioId)
        {
            using (SqlConnection cn = Conexion.ObtenerConexion())
            using (var cmd = new SqlCommand("sp_Usuario_Eliminar", cn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@UsuarioId", SqlDbType.UniqueIdentifier).Value = usuarioId;

                await cn.OpenAsync().ConfigureAwait(false);
                int filas = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                return filas > 0;
            }
        }

        private static Usuario Mapear(IDataRecord dr)
        {
            return new Usuario
            {
                UsuarioId = dr.GetGuid(dr.GetOrdinal("UsuarioId")),
                ClienteId = dr["ClienteId"] == DBNull.Value
                    ? (Guid?)null
                    : (Guid)dr["ClienteId"],
                Email = dr["Email"] as string,
                PasswordHash = dr["PasswordHash"] as string,
                Nombre = dr["Nombre"] as string,
                Activo = dr["Activo"] != DBNull.Value && Convert.ToBoolean(dr["Activo"]),
                FechaCreacion = dr["FechaCreacion"] == DBNull.Value
                    ? default(DateTime)
                    : Convert.ToDateTime(dr["FechaCreacion"]),
                RolId=dr.GetGuid(dr.GetOrdinal("RolId")),
                RolNombre = dr["RolNombre"] as string
            };
        }
    }
}