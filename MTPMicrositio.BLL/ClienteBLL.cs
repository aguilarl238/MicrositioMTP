using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTPMicrositio.DAL;
using MTPMicrositio.Entities;

namespace MTPMicrositio.BLL
{
    public class ClienteBLL
    {
        private readonly ClienteDAL _clienteDAL;

        public ClienteBLL()
        {
            _clienteDAL = new ClienteDAL();
        }

        public async Task<Guid> RegistrarClienteAsync(Cliente cliente)
        {
            ValidarCliente(cliente);

            if (cliente.LogoAutorizado && cliente.FechaAutorizacion == null)
                cliente.FechaAutorizacion = DateTime.Now;

            //if (string.IsNullOrWhiteSpace(cliente.Estatus))
            //    cliente.Estatus = "Activo";

            return await _clienteDAL.InsertarAsync(cliente).ConfigureAwait(false);
        }

        public async Task<bool> ActualizarClienteAsync(Cliente cliente)
        {
            if (cliente.ClienteId == Guid.Empty)
                throw new ArgumentException("El ClienteId es obligatorio para actualizar.");

            ValidarCliente(cliente);

            if (cliente.LogoAutorizado && cliente.FechaAutorizacion == null)
                cliente.FechaAutorizacion = DateTime.Now;

            return await _clienteDAL.ActualizarAsync(cliente).ConfigureAwait(false);
        }

        public async Task<bool> EliminarClienteAsync(Guid clienteId)
        {
            if (clienteId == Guid.Empty)
                throw new ArgumentException("El ClienteId es obligatorio para eliminar.");

            return await _clienteDAL.EliminarAsync(clienteId).ConfigureAwait(false);
        }

        public async Task<List<Cliente>> ObtenerTodosAsync()
        {
            return await _clienteDAL.ObtenerTodosAsync().ConfigureAwait(false);
        }

        public async Task<Cliente> ObtenerPorIdAsync(Guid clienteId)
        {
            if (clienteId == Guid.Empty)
                throw new ArgumentException("El ClienteId es obligatorio.");

            return await _clienteDAL.ObtenerPorIdAsync(clienteId).ConfigureAwait(false);
        }

        // Validaciones de negocio reutilizables
        private void ValidarCliente(Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            if (string.IsNullOrWhiteSpace(cliente.Nombre))
                throw new ArgumentException("El nombre del cliente es obligatorio.");

            if (cliente.Nombre.Length > 200)
                throw new ArgumentException("El nombre no puede exceder 200 caracteres.");
        }
    }
}
