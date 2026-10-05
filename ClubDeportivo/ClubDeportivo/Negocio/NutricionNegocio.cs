using System;
using System.Collections.Generic;
using ClubDeportivo.Datos;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Negocio
{
    /// <summary>
    /// Reglas de negocio del servicio de nutrición: funciona una vez por semana (miércoles),
    /// con turno asignado con anticipación.
    /// </summary>
    public class NutricionNegocio
    {
        public const DayOfWeek DiaDeAtencion = DayOfWeek.Wednesday;
        public const int TurnosDisponiblesPorDia = 12;

        private readonly ConsultaNutricionDAO dao = new ConsultaNutricionDAO();

        public int AsignarTurno(ConsultaNutricion c)
        {
            if (c.Fecha.DayOfWeek != DiaDeAtencion)
                throw new ArgumentException("El servicio de nutrición solo atiende los días miércoles.");
            if (c.Turno <= 0)
                throw new ArgumentException("El número de turno debe ser mayor a cero.");
            if (dao.ContarTurnosOcupados(c.Fecha) >= TurnosDisponiblesPorDia)
                throw new InvalidOperationException("No quedan turnos disponibles para esa fecha.");

            return dao.Asignar(c);
        }

        public void ModificarTurno(ConsultaNutricion c) => dao.Modificar(c);

        public void CancelarTurno(int idConsulta) => dao.Cancelar(idConsulta);

        public List<ConsultaNutricion> ListarPorSocio(int idSocio) => dao.ListarPorSocio(idSocio);

        /// <summary>Próximo miércoles a partir de (e incluyendo) la fecha indicada, útil para pre-cargar el formulario.</summary>
        public static DateTime ProximoDiaDeAtencion(DateTime desde)
        {
            var fecha = desde.Date;
            while (fecha.DayOfWeek != DiaDeAtencion) fecha = fecha.AddDays(1);
            return fecha;
        }
    }
}
