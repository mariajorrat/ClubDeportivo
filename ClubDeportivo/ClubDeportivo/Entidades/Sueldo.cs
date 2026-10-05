using System;

namespace ClubDeportivo.Entidades
{
    /// <summary>Liquidación mensual del sueldo de un profesor (último día hábil del mes).</summary>
    public class Sueldo
    {
        public int IdSueldo { get; set; }
        public int IdProfesor { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
        public decimal Monto { get; set; }
        public DateTime? FechaPago { get; set; }
    }
}
