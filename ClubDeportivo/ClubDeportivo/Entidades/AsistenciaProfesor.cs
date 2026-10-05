using System;

namespace ClubDeportivo.Entidades
{
    /// <summary>Registro de firma de asistencia del profesor al llegar al club.</summary>
    public class AsistenciaProfesor
    {
        public int IdAsistencia { get; set; }
        public int IdProfesor { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraEntrada { get; set; }
    }
}
