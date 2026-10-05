using System;

namespace ClubDeportivo.Entidades
{
    /// <summary>
    /// Clase base abstracta con los datos personales comunes a Socio, NoSocio y Profesor.
    /// No se instancia directamente: cada especialización la hereda y agrega sus propios atributos.
    /// </summary>
    public abstract class Persona
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }

        /// <summary>Apellido, Nombre — formato usado en listados y grillas.</summary>
        public string NombreCompleto => $"{Apellido}, {Nombre}";

        public int Edad
        {
            get
            {
                var hoy = DateTime.Today;
                int edad = hoy.Year - FechaNacimiento.Year;
                if (FechaNacimiento.Date > hoy.AddYears(-edad)) edad--;
                return edad;
            }
        }
    }
}
