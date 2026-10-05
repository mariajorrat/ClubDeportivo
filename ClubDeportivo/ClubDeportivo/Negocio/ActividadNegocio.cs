using System;
using System.Collections.Generic;
using ClubDeportivo.Datos;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Negocio
{
    /// <summary>Reglas de negocio de las actividades que ofrece el club.</summary>
    public class ActividadNegocio
    {
        private readonly ActividadDAO dao = new ActividadDAO();

        public int AltaActividad(Actividad a)
        {
            if (string.IsNullOrWhiteSpace(a.Nombre))
                throw new ArgumentException("El nombre de la actividad es obligatorio.");
            if (a.CostoNoSocio < 0)
                throw new ArgumentException("El costo para no socios no puede ser negativo.");
            if (a.CupoMaximo <= 0)
                throw new ArgumentException("El cupo máximo debe ser mayor a cero.");
            return dao.Alta(a);
        }

        public void ModificarActividad(Actividad a) => dao.Modificar(a);

        public void BajaActividad(int idActividad) => dao.Baja(idActividad);

        public List<Actividad> Listar() => dao.Listar();
    }
}
