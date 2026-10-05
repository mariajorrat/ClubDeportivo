using System;

namespace ClubDeportivo.Entidades
{
    /// <summary>Carnet entregado al socio, habilita el ingreso al club.</summary>
    public class Carnet
    {
        public int IdCarnet { get; set; }
        public int IdSocio { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
    }
}
