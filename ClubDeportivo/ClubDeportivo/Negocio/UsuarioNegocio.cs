using System;
using System.Security.Cryptography;
using ClubDeportivo.Datos;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Negocio
{
    /// <summary>Reglas de negocio del login y control de acceso por rol.</summary>
    public class UsuarioNegocio
    {
        private readonly UsuarioDAO dao = new UsuarioDAO();

        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 600_000;

        /// <summary>Genera un hash PBKDF2 con salt aleatorio para almacenar contraseñas.</summary>
        public static string Hash(string textoPlano)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(textoPlano ?? string.Empty, salt, Iterations,
                HashAlgorithmName.SHA256, HashSize);
            return $"pbkdf2-sha256${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        private static bool VerificarHash(string textoPlano, string almacenado)
        {
            string[] partes = almacenado.Split('$');
            if (partes.Length != 4 || partes[0] != "pbkdf2-sha256" ||
                !int.TryParse(partes[1], out int iterations))
                return false;

            try
            {
                byte[] salt = Convert.FromBase64String(partes[2]);
                byte[] esperado = Convert.FromBase64String(partes[3]);
                byte[] actual = Rfc2898DeriveBytes.Pbkdf2(textoPlano ?? string.Empty, salt, iterations,
                    HashAlgorithmName.SHA256, esperado.Length);
                return CryptographicOperations.FixedTimeEquals(actual, esperado);
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>Valida usuario/contraseña contra la base. Devuelve el Usuario autenticado o null.</summary>
        public Usuario Login(string nombreUsuario, string contrasenaPlana)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(contrasenaPlana))
                return null;
            Usuario usuario = dao.BuscarActivo(nombreUsuario);
            return usuario is not null && VerificarHash(contrasenaPlana, usuario.ContrasenaHash)
                ? usuario
                : null;
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
