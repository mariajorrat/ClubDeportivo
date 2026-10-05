using System;

namespace ClubDeportivo.Entidades
{
    /// <summary>Cuota mensual de un socio. Se abona por adelantado, en efectivo o tarjeta (3 o 6 cuotas).</summary>
    public class Cuota
    {
        public int IdCuota { get; set; }
        public int IdSocio { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public DateTime? FechaPago { get; set; }
        public string FormaPago { get; set; }
        public int? CuotasTarjeta { get; set; }
    }
}
