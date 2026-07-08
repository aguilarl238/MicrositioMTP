using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTPMicrositio.DAL
{
    internal static class Conexion
    {
        // En producción, lee esto desde appsettings.json o variables de entorno
        //private const string CadenaConexion = ConfigurationManager.ConnectionStrings["WebApiMTPMicrositioContext"];
           // "Server=(localdb)\\MSSQLLocalDB;Database=MTP_MicrositioDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public static SqlConnection ObtenerConexion()
        {
            string cadena = ConfigurationManager
                .ConnectionStrings["WebApiMTPMicrositioContext"].ConnectionString;
            return new SqlConnection(cadena);
        }
    }
}
