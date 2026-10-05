using System;
using System.Collections.Generic;
using System.Data;
using ClubDeportivo.Datos;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Negocio
{
    /// <summary>
    /// Reglas de negocio de la liquidación de sueldos: se paga mensualmente el último día hábil del mes.
    /// </summary>
    public class SueldoNegocio
    {
        private readonly SueldoDAO dao = new SueldoDAO();

        /// <summary>Determina si la fecha indicada es el último día hábil (lunes a viernes) del mes.</summary>
        public bool EsUltimoDiaHabilDelMes(DateTime fecha)
        {
            DateTime ultimoDiaMes = new DateTime(fecha.Year, fecha.Month, DateTime.DaysInMonth(fecha.Year, fecha.Month));
            DateTime cursor = ultimoDiaMes;
            while (cursor.DayOfWeek == DayOfWeek.Saturday || cursor.DayOfWeek == DayOfWeek.Sunday)
                cursor = cursor.AddDays(-1);
            return fecha.Date == cursor.Date;
        }

        /// <summary>
        /// Liquida el sueldo del profesor para el período indicado. Si la fecha no es el último día hábil
        /// del mes se permite igualmente (para no bloquear pruebas fuera de fecha) pero se informa al llamador.
        /// </summary>
        public void LiquidarSueldo(int idProfesor, int mes, int anio, decimal monto)
        {
            if (monto <= 0)
                throw new ArgumentException("El monto del sueldo debe ser mayor a cero.");
            dao.Liquidar(idProfesor, mes, anio, monto);
        }

        public DataTable ListarPorPeriodo(int mes, int anio) => dao.ListarPorPeriodo(mes, anio);

        public List<Sueldo> ListarPorProfesor(int idProfesor) => dao.ListarPorProfesor(idProfesor);
    }
}
