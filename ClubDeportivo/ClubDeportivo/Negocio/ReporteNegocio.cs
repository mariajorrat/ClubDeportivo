using System.Data;
using ClubDeportivo.Datos;

namespace ClubDeportivo.Negocio
{
    /// <summary>Capa de negocio del módulo de reportes (delega en ReporteDAO).</summary>
    public class ReporteNegocio
    {
        private readonly ReporteDAO dao = new ReporteDAO();

        public DataTable SociosActivos() => dao.SociosPorEstado("Activo");
        public DataTable SociosInactivos() => dao.SociosPorEstado("Inactivo");
        public DataTable RecaudacionDelMes(int mes, int anio) => dao.RecaudacionDelMes(mes, anio);
        public DataTable CarnetsEmitidos() => dao.CarnetsEmitidos();
        public DataTable ActividadesConCupo() => dao.ActividadesConCupo();
    }
}
