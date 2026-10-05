using System;

namespace ClubDeportivo.Entidades
{
    /// <summary>Inscripción de una persona (socio o no socio) a una actividad puntual.</summary>
    public class Inscripcion
    {
        public int IdInscripcion { get; set; }
        public int IdPersona { get; set; }
        public int IdActividad { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public bool AptoFisico { get; set; }
        public decimal MontoPagado { get; set; }
    }
}
