using System;
using System.Collections.Generic;
using ClubDeportivo.Datos;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Negocio
{
    /// <summary>
    /// Reglas de negocio del cobro de cuotas: la cuota se abona por adelantado, en efectivo o
    /// tarjeta de crédito; con tarjeta existen promociones en 3 y 6 cuotas.
    /// </summary>
    public class CuotaNegocio
    {
        private readonly CuotaDAO dao = new CuotaDAO();

        public DateTime CobrarCuota(int idSocio, decimal monto, string formaPago, int? cuotasTarjeta)
        {
            if (monto <= 0)
                throw new ArgumentException("El monto de la cuota debe ser mayor a cero.");
            if (formaPago != "Efectivo" && formaPago != "Tarjeta")
                throw new ArgumentException("La forma de pago debe ser Efectivo o Tarjeta.");
            if (formaPago == "Tarjeta" && cuotasTarjeta.HasValue && cuotasTarjeta != 3 && cuotasTarjeta != 6)
                throw new ArgumentException("Con tarjeta, la promoción admitida es en 3 o 6 cuotas.");
            if (formaPago == "Efectivo")
                cuotasTarjeta = null;

            return dao.CobrarCuota(idSocio, monto, formaPago, cuotasTarjeta);
        }

        public List<Cuota> HistorialPorSocio(int idSocio) => dao.ListarPorSocio(idSocio);
    }
}
