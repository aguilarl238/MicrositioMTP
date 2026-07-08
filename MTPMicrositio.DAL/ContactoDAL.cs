using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPMicrositio.DAL
{
    public class ContactoDAL
    {
        private readonly string _connectionString;

        public ContactoDAL()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["WebApiMTPMicrositioContext"]?.ConnectionString;
        }

        public async Task<bool> InsertarContactoAsync(string nombre, string correo, string mensaje)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = @"INSERT INTO ContactoProspectos (NombreCompleto, CorreoCorporativo, Mensaje) 
                                 VALUES (@NombreCompleto, @CorreoCorporativo, @Mensaje);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@NombreCompleto", SqlDbType.VarChar, 150).Value = nombre;
                        cmd.Parameters.Add("@CorreoCorporativo", SqlDbType.VarChar, 100).Value = correo;
                        cmd.Parameters.Add("@Mensaje", SqlDbType.VarChar, 500).Value = mensaje;

                        await conn.OpenAsync();
                        int filasAfectadas = await cmd.ExecuteNonQueryAsync();
                        return filasAfectadas > 0;
                    }
                }
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }
    }
}
