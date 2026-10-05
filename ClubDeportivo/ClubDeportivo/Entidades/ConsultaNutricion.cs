using System;

namespace ClubDeportivo.Entidades
{
    /// <summary>Turno de nutrición (el servicio atiende una vez por semana, con turno asignado).</summary>
    public class ConsultaNutricion
    {
        public int IdConsulta { get; set; }
        public int IdSocio { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public int Turno { get; set; }
        public string Observaciones { get; set; }
        public string CargaActividadPermitida { get; set; }
    }
}
