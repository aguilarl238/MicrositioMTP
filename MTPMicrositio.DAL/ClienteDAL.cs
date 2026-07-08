using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTPMicrositio.Entities;
namespace MTPMicrositio.DAL
{
    public class ClienteDAL
    {
        // ---------------------------------------------------------
        // INSERT (async) - retorna el ClienteId generado
        // ---------------------------------------------------------
        public async Task<Guid> InsertarAsync(Cliente cliente)
        {
            try
            {
                using (SqlConnection conn = Conexion.ObtenerConexion())
                using (SqlCommand cmd = new SqlCommand("sp_Cliente_Insert", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 200).Value =
                        (object)cliente.Nombre ?? DBNull.Value;
                    cmd.Parameters.Add("@EstatusId", SqlDbType.UniqueIdentifier).Value =
                        (object)cliente.EstatusId ?? DBNull.Value;
                    cmd.Parameters.Add("@LogoUrl", SqlDbType.NVarChar, 500).Value =
                        (object)cliente.LogoUrl ?? DBNull.Value;
                    cmd.Parameters.Add("@LogoAutorizado", SqlDbType.Bit).Value =
                        cliente.LogoAutorizado;
                    cmd.Parameters.Add("@UsuarioCreacion", SqlDbType.UniqueIdentifier).Value =
                        (object)cliente.UsuarioCreacionID ?? DBNull.Value;
                    SqlParameter paramOutput = new SqlParameter("@ClienteId", SqlDbType.UniqueIdentifier)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(paramOutput);

                    await conn.OpenAsync().ConfigureAwait(false);

                    object resultado = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);

                    return resultado != null && resultado != DBNull.Value
                        ? (Guid)paramOutput.Value
                        : Guid.Empty;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // ---------------------------------------------------------
        // UPDATE (async) - retorna true si afectó al menos 1 fila
        // ---------------------------------------------------------
        public async Task<bool> ActualizarAsync(Cliente cliente)
        {
            try
            {
                using (SqlConnection conn = Conexion.ObtenerConexion())
                using (SqlCommand cmd = new SqlCommand("sp_Cliente_Update", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@ClienteId", SqlDbType.UniqueIdentifier).Value =
                        cliente.ClienteId;
                    cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 200).Value =
                        (object)cliente.Nombre ?? DBNull.Value;
                    cmd.Parameters.Add("@EstatusId", SqlDbType.UniqueIdentifier).Value =
                            (object)cliente.EstatusId ?? DBNull.Value;
                    cmd.Parameters.Add("@LogoUrl", SqlDbType.NVarChar, 500).Value =
                        (object)cliente.LogoUrl ?? DBNull.Value;
                    cmd.Parameters.Add("@LogoAutorizado", SqlDbType.Bit).Value =
                        cliente.LogoAutorizado;
                    cmd.Parameters.Add("@UsuarioActualizacion", SqlDbType.UniqueIdentifier).Value =
                        (object)cliente.UsuarioCreacionID ?? DBNull.Value;

                    await conn.OpenAsync().ConfigureAwait(false);

                    object resultado = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                    int filas = resultado != null ? Convert.ToInt32(resultado) : 0;

                    return filas > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        // ---------------------------------------------------------
        // DELETE (async) - retorna true si afectó al menos 1 fila
        // ---------------------------------------------------------
        public async Task<bool> EliminarAsync(Guid clienteId)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand("sp_Cliente_Delete", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ClienteId", SqlDbType.UniqueIdentifier).Value =
                    clienteId;

                await conn.OpenAsync().ConfigureAwait(false);

                object resultado = await cmd.ExecuteNonQueryAsync().ConfigureAwait(false);
                int filas = resultado != null ? Convert.ToInt32(resultado) : 0;

                return filas > 0;
            }
        }

        // ---------------------------------------------------------
        // GET ALL (async) - retorna lista de clientes
        // ---------------------------------------------------------
        public async Task<List<Cliente>> ObtenerTodosAsync()
        {
            var lista = new List<Cliente>();
            try
            {
                using (SqlConnection conn = Conexion.ObtenerConexion())
                using (SqlCommand cmd = new SqlCommand("sp_Cliente_Select", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    await conn.OpenAsync().ConfigureAwait(false);

                    using (SqlDataReader reader =
                        await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            lista.Add(MapearCliente(reader));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error" +
                    ex.Message, ex);
            }
            return lista;
        
        }

        // ---------------------------------------------------------
        // GET BY ID (async) - retorna un cliente o null
        // ---------------------------------------------------------
        public async Task<Cliente> ObtenerPorIdAsync(Guid clienteId)
        {
            using (SqlConnection conn = Conexion.ObtenerConexion())
            using (SqlCommand cmd = new SqlCommand("sp_Clientes_GetById", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@ClienteId", SqlDbType.UniqueIdentifier).Value =
                    clienteId;

                await conn.OpenAsync().ConfigureAwait(false);

                using (SqlDataReader reader =
                    await cmd.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    if (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        return MapearCliente(reader);
                    }
                }
            }

            return null;
        }

        // ---------------------------------------------------------
        // Helper: mapea una fila del reader a un objeto Cliente
        // ---------------------------------------------------------
        private Cliente MapearCliente(SqlDataReader reader)
        {
            return new Cliente
            {
                ClienteId = reader.GetGuid(reader.GetOrdinal("ClienteId")),
                Nombre = reader["Nombre"] as string,
                EstatusId =(Guid)reader["EstatusId"],
                LogoUrl = reader["LogoUrl"] as string,
                LogoAutorizado = reader["LogoAutorizado"] != DBNull.Value &&
                                 Convert.ToBoolean(reader["LogoAutorizado"]),                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                 
                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"])
            };
        }
    }
}
