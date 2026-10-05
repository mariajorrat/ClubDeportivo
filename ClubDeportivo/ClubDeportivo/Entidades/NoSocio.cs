using System;

namespace ClubDeportivo.Entidades
{
    /// <summary>Persona que no es socia y abona únicamente la actividad que realiza, según cartilla.</summary>
    public class NoSocio : Persona
    {
        public int IdNoSocio { get; set; }
        public string ActividadRealizada { get; set; }
        public DateTime FechaVisita { get; set; }
        public decimal MontoPagado { get; set; }
    }
}
