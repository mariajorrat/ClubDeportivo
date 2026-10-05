using System;
using System.Collections.Generic;
using ClubDeportivo.Datos;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Negocio
{
    /// <summary>Reglas de negocio de las rutinas que el profesor confecciona para sus alumnos.</summary>
    public class RutinaNegocio
    {
        private readonly RutinaDAO dao = new RutinaDAO();

        public int AltaRutina(Rutina r)
        {
            if (string.IsNullOrWhiteSpace(r.Descripcion))
                throw new ArgumentException("La descripción de la rutina es obligatoria.");
            return dao.Alta(r);
        }

        public void ModificarRutina(Rutina r) => dao.Modificar(r);

        public void BajaRutina(int idRutina) => dao.Baja(idRutina);

        public List<Rutina> ListarPorSocio(int idSocio) => dao.ListarPorSocio(idSocio);

        public List<Rutina> ListarPorProfesor(int idProfesor) => dao.ListarPorProfesor(idProfesor);
    }
}
