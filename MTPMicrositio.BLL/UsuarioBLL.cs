// BLL/UsuarioBLL.cs
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MTPMicrositio.DAL;
using MTPMicrositio.Entities;

namespace MTPMicrositio.BLL
{
    public class UsuarioBLL
    {
        private readonly UsuarioDAL _usuarioDAL;

        public UsuarioBLL()
        {
            _usuarioDAL = new UsuarioDAL();
        }

        public Task<List<Usuario>> ObtenerTodosAsync()
        {
            return _usuarioDAL.ObtenerTodosAsync();
        }

        public Task<Usuario> ObtenerPorIdAsync(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty)
                throw new ArgumentException("El UsuarioId no es válido.", nameof(usuarioId));

            return _usuarioDAL.ObtenerPorIdAsync(usuarioId);
        }

        public async Task<Usuario> CrearAsync(Usuario usuario, string passwordPlano)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));
            if (string.IsNullOrWhiteSpace(usuario.Email))
                throw new ArgumentException("El email es obligatorio.");
            if (string.IsNullOrWhiteSpace(usuario.Nombre))
                throw new ArgumentException("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(passwordPlano))
                throw new ArgumentException("La contraseña es obligatoria.");

            // Validar email único
            var existente = await _usuarioDAL.ObtenerPorEmailAsync(usuario.Email).ConfigureAwait(false);
            if (existente != null)
                throw new InvalidOperationException("Ya existe un usuario con ese email.");

            usuario.PasswordHash = HashPassword(passwordPlano);
            usuario.Activo = true;

            Guid nuevoId = await _usuarioDAL.InsertarAsync(usuario).ConfigureAwait(false);
            return await _usuarioDAL.ObtenerPorIdAsync(nuevoId).ConfigureAwait(false);
        }

        public async Task<bool> ActualizarAsync(Usuario usuario)
        {
            if (usuario == null)
                throw new ArgumentNullException(nameof(usuario));
            if (usuario.UsuarioId == Guid.Empty)
                throw new ArgumentException("El UsuarioId es obligatorio.");
            if (string.IsNullOrWhiteSpace(usuario.Email))
                throw new ArgumentException("El email es obligatorio.");

            var existente = await _usuarioDAL.ObtenerPorIdAsync(usuario.UsuarioId).ConfigureAwait(false);
            if (existente == null)
                return false;

            return await _usuarioDAL.ActualizarAsync(usuario).ConfigureAwait(false);
        }

        public Task<bool> CambiarPasswordAsync(Guid usuarioId, string passwordPlano)
        {
            if (usuarioId == Guid.Empty)
                throw new ArgumentException("El UsuarioId no es válido.", nameof(usuarioId));
            if (string.IsNullOrWhiteSpace(passwordPlano))
                throw new ArgumentException("La contraseña es obligatoria.");

            string hash = HashPassword(passwordPlano);
            return _usuarioDAL.CambiarPasswordAsync(usuarioId, hash);
        }

        public async Task<Usuario> ValidarCredencialesAsync(string email, string passwordPlano)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(passwordPlano))
                return null;

            var usuario = await _usuarioDAL.ObtenerPorEmailAsync(email).ConfigureAwait(false);
            if (usuario == null || !usuario.Activo)
                return null;

            string hash = HashPassword(passwordPlano);
            return string.Equals(hash, usuario.PasswordHash, StringComparison.Ordinal) ? usuario : null;
        }

        public Task<bool> EliminarAsync(Guid usuarioId)
        {
            if (usuarioId == Guid.Empty)
                throw new ArgumentException("El UsuarioId no es válido.", nameof(usuarioId));

            return _usuarioDAL.EliminarAsync(usuarioId);
        }

        // Hash SHA-256 simple. En producción usa PBKDF2/BCrypt con salt.
        private static string HashPassword(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                var sb = new StringBuilder();
                foreach (var b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}