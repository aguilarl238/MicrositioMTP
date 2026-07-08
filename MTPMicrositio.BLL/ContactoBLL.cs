using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTPMicrositio.DAL;

namespace MTPMicrositio.BLL
{
    public class ContactoBLL
    {
        private readonly ContactoDAL _contactoDal;

        public ContactoBLL()
        {
            _contactoDal = new ContactoDAL();
        }

        public async Task<bool> RegistrarContactoAsync(string nombre, string correo, string mensaje)
        {
            if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(mensaje))
            {
                throw new ArgumentException("Todos los campos del formulario son obligatorios.");
            }

            // Aquí se propaga la llamada asíncrona hacia la base de datos
            return await _contactoDal.InsertarContactoAsync(nombre, correo, mensaje);
        }
    }
}
