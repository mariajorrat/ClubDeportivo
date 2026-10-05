using System;
using System.Collections.Generic;
using ClubDeportivo.Datos;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Negocio
{
    /// <summary>Reglas de negocio del plantel de profesores (titulares y suplentes) y su asistencia.</summary>
    public class ProfesorNegocio
    {
        private readonly ProfesorDAO dao = new ProfesorDAO();
        private readonly AsistenciaProfesorDAO asistenciaDao = new AsistenciaProfesorDAO();

        public int AltaProfesor(Profesor p)
        {
            if (string.IsNullOrWhiteSpace(p.Nombre) || string.IsNullOrWhiteSpace(p.Apellido))
                throw new ArgumentException("Nombre y apellido son obligatorios.");
            if (string.IsNullOrWhiteSpace(p.Legajo))
                throw new ArgumentException("El legajo es obligatorio.");
            return dao.Alta(p);
        }

        public void ModificarProfesor(Profesor p) => dao.Modificar(p);

        public void BajaProfesor(int idProfesor) => dao.Baja(idProfesor);

        public List<Profesor> Listar() => dao.Listar();

        public List<Profesor> ListarSuplentes() => dao.ListarSuplentes();

        /// <summary>
        /// Registra la firma de asistencia del profesor al llegar al club.
        /// Un profesor no puede firmar dos veces el mismo día.
        /// </summary>
        public void RegistrarAsistencia(int idProfesor)
        {
            if (asistenciaDao.YaFirmoHoy(idProfesor, DateTime.Today))
                throw new InvalidOperationException("El profesor ya registró su asistencia hoy.");
            asistenciaDao.Registrar(idProfesor, DateTime.Today, DateTime.Now.TimeOfDay);
        }
    }
}
