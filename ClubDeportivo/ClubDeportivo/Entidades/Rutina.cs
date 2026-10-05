using System;

namespace ClubDeportivo.Entidades
{
    /// <summary>Rutina de entrenamiento que un profesor confecciona para un socio.</summary>
    public class Rutina
    {
        public int IdRutina { get; set; }
        public int IdProfesor { get; set; }
        public int IdSocio { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
    }
}
