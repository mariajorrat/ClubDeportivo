using System;

namespace ClubDeportivo.Entidades
{
    /// <summary>Socio del club: abona cuota mensual por adelantado y tiene carnet.</summary>
    public class Socio : Persona
    {
        public int IdSocio { get; set; }
        public string NroSocio { get; set; }
        public DateTime FechaAlta { get; set; }
        public bool AptoFisico { get; set; }
        public string Estado { get; set; } = "Activo";
        public bool CarnetEntregado { get; set; }

        /// <summary>Última fecha de vencimiento de cuota conocida (se completa desde CuotaDAO al listar).</summary>
        public DateTime? UltimoVencimiento { get; set; }

        /// <summary>true si, según UltimoVencimiento, el socio puede realizar actividades hoy.</summary>
        public bool CuotaAlDia => UltimoVencimiento.HasValue && UltimoVencimiento.Value.Date >= DateTime.Today;
    }
}
