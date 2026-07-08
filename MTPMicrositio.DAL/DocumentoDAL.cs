using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MTPMicrositio.Entities;

namespace MTPMicrositio.DataAccess
{
    public class DocumentoDAL
    {
        private readonly string _connectionString = ConfigurationManager.ConnectionStrings["WebApiMTPMicrositioContext"].ConnectionString;

        // Método de Inserción Asíncrono
        public async Task<Guid> InsertarAsync(DocumentoDTO documento)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Documento_Insert", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@ClienteId", (object)documento.ClienteId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TipoDocumentoId", (object)documento.TipoDocumentoId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Nombre", (object)documento.Nombre ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UrlDoc", (object)documento.UrlDoc ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@UsuarioId", (object)documento.UsuarioId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaVigencia", (object)documento.FechaVigencia ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@EstatusId", (object)documento.EstatusId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProyectoId", (object)documento.ProyectoId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ArchivoBase64", (object)documento.ArchivoBase64 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ExtensionArchivo", (object)documento.ExtensionArchivo ?? DBNull.Value);

                        SqlParameter outputParam = new SqlParameter("@DocumentoId", SqlDbType.UniqueIdentifier)
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
            catch (SqlException ex)
            {
                throw new Exception("Error asíncrono en la base de datos al insertar el documento: " + ex.Message, ex);
            }
        }

        // Método de Edición Asíncrono
        public async Task EditarAsync(DocumentoDTO documento)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Documentos_Editar", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DocumentoId", documento.DocumentoId);
                        cmd.Parameters.AddWithValue("@ClienteId", (object)documento.ClienteId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TipoDocumentoId", (object)documento.TipoDocumentoId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Nombre", (object)documento.Nombre ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Activo", documento.Activo);
                        cmd.Parameters.AddWithValue("@EstatusId", (object)documento.EstatusId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@FechaVigencia", (object)documento.FechaVigencia ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProyectoId", (object)documento.ProyectoId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ArchivoBase64", (object)documento.ArchivoBase64 ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ExtensionArchivo", (object)documento.ExtensionArchivo ?? DBNull.Value);

                        await conn.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error asíncrono en DAL al actualizar el documento.", ex);
            }
        }

        // Método de Consulta Asíncrono por ID
        public async Task<DocumentoDTO> ObtenerPorIdAsync(Guid documentoId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Documento_Select", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DocumentoId", documentoId);
                        cmd.Parameters.AddWithValue("@ClienteId", DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProyectoId", DBNull.Value);

                        await conn.OpenAsync();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new DocumentoDTO
                                {
                                    DocumentoId = (Guid)reader["DocumentoId"],
                                    ClienteId = reader["ClienteId"] as Guid?,
                                    TipoDocumentoId = reader["TipoDocumentoId"] as Guid?,
                                    Nombre = reader["Nombre"].ToString(),
                                    Activo = Convert.ToBoolean(reader["Activo"]),
                                    EstatusId = reader["EstatusId"] as Guid?,
                                    FechaVigencia = reader["FechaVigencia"] as DateTime?,
                                    ProyectoId = reader["ProyectoId"] as Guid?,
                                    ArchivoBase64 = reader["ArchivoBase64"] != DBNull.Value ? reader["ArchivoBase64"].ToString() : null,
                                    ExtensionArchivo = reader["ExtensionArchivo"] != DBNull.Value ? reader["ExtensionArchivo"].ToString() : null
                                };
                            }
                        }
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error asíncrono en DAL al consultar el documento.", ex);
            }
        }

        // Listado Asíncrono con Filtros
        public async Task<List<DocumentoDTO>> ListarConFiltrosAsync(Guid? clienteId, Guid? proyectoId)
        {
            var lista = new List<DocumentoDTO>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Documento_Select", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@DocumentoId", DBNull.Value);
                        cmd.Parameters.AddWithValue("@ClienteId", (object)clienteId ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@ProyectoId", (object)proyectoId ?? DBNull.Value);

                        await conn.OpenAsync();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                lista.Add(new DocumentoDTO
                                {
                                    DocumentoId = (Guid)reader["DocumentoId"],
                                    Nombre = reader["Nombre"].ToString(),
                                    ClienteId = (Guid)reader["ClienteId"],
                                    ProyectoId = (Guid)reader["ProyectoId"],
                                    TipoDocumentoId = (Guid)reader["TipoDocumentoId"],
                                    EstatusId = (Guid)reader["EstatusId"],
                                    ClienteNombre = reader["ClienteNombre"].ToString(),
                                    ProyectoNombre = reader["ProyectoNombre"].ToString(),
                                    TipoDocumentoNombre = reader["TipoDocumentoNombre"].ToString(),
                                    EstatusNombre = reader["EstatusNombre"].ToString(),
                                    Activo = Convert.ToBoolean(reader["Activo"]),
                                    FechaVigencia = reader["FechaVigencia"] as DateTime?,
                                    ArchivoBase64 = reader["ArchivoBase64"] != DBNull.Value ? reader["ArchivoBase64"].ToString() : null,
                                    ExtensionArchivo = reader["ExtensionArchivo"] != DBNull.Value ? reader["ExtensionArchivo"].ToString() : null,
                                    UrlDoc = reader["UrlDoc"] != DBNull.Value ? reader["UrlDoc"].ToString() : null
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error asíncrono en DAL al listar documentos.", ex);
            }
            return lista;
        }

        // Catálogos Asíncronos
        public async Task<List<CatalogoDto>> ObtenerCatalogoAsync(string tabla)
        {
            var lista = new List<CatalogoDto>();
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_Catalogo_Obtener", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Tabla", tabla);

                        await conn.OpenAsync();

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                lista.Add(new CatalogoDto
                                {
                                    Id = (Guid)reader["Id"],
                                    Descripcion = reader["Nombre"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error asíncrono en DAL al cargar catálogo {tabla}.", ex);
            }
            return lista;
        }
    }
}