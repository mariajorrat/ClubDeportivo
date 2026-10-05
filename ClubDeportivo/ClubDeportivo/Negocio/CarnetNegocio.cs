using System;
using System.Collections.Generic;
using ClubDeportivo.Datos;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Negocio
{
    /// <summary>Reglas de negocio de la emisión de carnets: se entrega al socio con apto físico y cuota al día.</summary>
    public class CarnetNegocio
    {
        private readonly CarnetDAO dao = new CarnetDAO();
        private readonly SocioDAO socioDao = new SocioDAO();

        public int EmitirCarnet(int idSocio)
        {
            var socio = socioDao.BuscarPorId(idSocio);
            if (socio == null)
                throw new ArgumentException("El socio no existe.");
            if (!socio.AptoFisico)
                throw new InvalidOperationException("El socio debe presentar el apto físico antes de emitir el carnet.");

            var ultimoVenc = socioDao.ObtenerUltimoVencimiento(idSocio);
            if (!ultimoVenc.HasValue || ultimoVenc.Value.Date < DateTime.Today)
                throw new InvalidOperationException("El socio debe tener la cuota al día para emitir el carnet.");

            var emision = DateTime.Today;
            var vencimiento = emision.AddYears(1);
            int id = dao.Emitir(idSocio, emision, vencimiento);
            socioDao.MarcarCarnetEntregado(idSocio);
            return id;
        }

        public List<Carnet> Listar() => dao.Listar();
    }
}
