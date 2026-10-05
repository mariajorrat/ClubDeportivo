using System;
using System.Collections.Generic;
using System.Data;
using ClubDeportivo.Datos;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Negocio
{
    /// <summary>
    /// Reglas de negocio del socio: alta, baja, modificación y la regla central del club:
    /// "vencido el período de pago, el socio automáticamente no puede realizar actividades".
    /// </summary>
    public class SocioNegocio
    {
        private readonly SocioDAO dao = new SocioDAO();
        private readonly CuotaDAO cuotaDao = new CuotaDAO();

        public int AltaSocio(Socio s)
        {
            if (string.IsNullOrWhiteSpace(s.Nombre) || string.IsNullOrWhiteSpace(s.Apellido))
                throw new ArgumentException("Nombre y apellido son obligatorios.");
            if (string.IsNullOrWhiteSpace(s.Dni))
                throw new ArgumentException("El DNI es obligatorio.");
            if (string.IsNullOrWhiteSpace(s.NroSocio))
                throw new ArgumentException("El número de socio es obligatorio.");
            if (dao.BuscarPorDni(s.Dni) != null)
                throw new ArgumentException("Ya existe una persona registrada con ese DNI.");
            if (dao.BuscarPorNroSocio(s.NroSocio) != null)
                throw new ArgumentException("Ya existe un socio con ese número.");

            return dao.Alta(s);
        }

        public void ModificarSocio(Socio s) => dao.Modificar(s);

        public void BajaSocio(int idSocio) => dao.Baja(idSocio);

        public List<Socio> Listar()
        {
            var lista = dao.Listar();
            foreach (var s in lista)
                s.UltimoVencimiento = dao.ObtenerUltimoVencimiento(s.IdSocio);
            return lista;
        }

        public Socio Buscar(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio)) return null;
            Socio s = dao.BuscarPorNroSocio(criterio) ?? dao.BuscarPorDni(criterio);
            if (s != null) s.UltimoVencimiento = dao.ObtenerUltimoVencimiento(s.IdSocio);
            return s;
        }

        /// <summary>
        /// Regla de negocio central: el socio puede realizar actividades solo si su última cuota
        /// abonada tiene vencimiento igual o posterior a hoy.
        /// </summary>
        public bool PuedeRealizarActividad(int idSocio)
        {
            DateTime? ultimoVencimiento = dao.ObtenerUltimoVencimiento(idSocio);
            return ultimoVencimiento.HasValue && ultimoVencimiento.Value.Date >= DateTime.Today;
        }

        /// <summary>Listado diario de socios a los que hoy (o la fecha indicada) les vence la cuota.</summary>
        public DataTable ListarVencimientosDelDia(DateTime fecha) => cuotaDao.ListarVencimientosDia(fecha);
    }
}
