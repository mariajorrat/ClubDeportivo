using System;
using System.Collections.Generic;
using ClubDeportivo.Datos;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Negocio
{
    /// <summary>
    /// Reglas de negocio de los no socios: abonan únicamente el costo de la actividad
    /// que realizan en el momento, según cartilla (ActividadDAO.CostoNoSocio).
    /// </summary>
    public class NoSocioNegocio
    {
        private readonly NoSocioDAO dao = new NoSocioDAO();
        private readonly InscripcionDAO inscripcionDao = new InscripcionDAO();
        private readonly ActividadDAO actividadDao = new ActividadDAO();

        /// <summary>Registra (o reutiliza) la persona como no socio y genera la inscripción a la actividad del día.</summary>
        public int RegistrarVisita(NoSocio n, int idActividad)
        {
            if (string.IsNullOrWhiteSpace(n.Nombre) || string.IsNullOrWhiteSpace(n.Apellido))
                throw new ArgumentException("Nombre y apellido son obligatorios.");
            if (string.IsNullOrWhiteSpace(n.Dni))
                throw new ArgumentException("El DNI es obligatorio.");

            var actividad = actividadDao.Listar().Find(a => a.IdActividad == idActividad);
            if (actividad == null)
                throw new ArgumentException("Debe seleccionar una actividad válida.");

            int idPersona = dao.Alta(n);

            inscripcionDao.Alta(new Inscripcion
            {
                IdPersona = idPersona,
                IdActividad = idActividad,
                Fecha = DateTime.Today,
                Tipo = "NoSocio",
                AptoFisico = false,
                MontoPagado = actividad.CostoNoSocio
            });

            return idPersona;
        }

        public List<NoSocio> Listar() => dao.Listar();
    }
}
