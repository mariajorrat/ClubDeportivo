using System;
using System.Security.Cryptography;
using System.Text;
using ClubDeportivo.Datos;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Negocio
{
    /// <summary>Reglas de negocio del login y control de acceso por rol.</summary>
    public class UsuarioNegocio
    {
        private readonly UsuarioDAO dao = new UsuarioDAO();

        /// <summary>Calcula el hash SHA-256 de un texto plano (nunca se guarda ni compara la contraseña en claro).</summary>
        public static string Hash(string textoPlano)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(textoPlano ?? string.Empty));
                var sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        /// <summary>Valida usuario/contraseña contra la base. Devuelve el Usuario autenticado o null.</summary>
        public Usuario Login(string nombreUsuario, string contrasenaPlana)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contrasenaPlana))
                return null;
            return dao.Autenticar(nombreUsuario, Hash(contrasenaPlana));
        }

        public int AltaUsuario(string nombreUsuario, string contrasenaPlana, string rol, int? idPersona)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contrasenaPlana))
                throw new ArgumentException("Usuario y contraseña son obligatorios.");
            return dao.Alta(new Usuario
            {
                NombreUsuario = nombreUsuario,
                ContrasenaHash = Hash(contrasenaPlana),
                Rol = rol,
                IdPersona = idPersona,
                Activo = true
            });
        }
    }
}
