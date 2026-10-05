using System;
using System.Data;
using Microsoft.Data.Sqlite;

namespace ClubDeportivo.Datos
{
    /// <summary>Consultas de solo lectura para el módulo de Reportes (frmReportes).</summary>
    public class ReporteDAO
    {
        private DataTable Ejecutar(string sql, params SqliteParameter[] parametros)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(sql, con);
                if (parametros != null) cmd.Parameters.AddRange(parametros);
                var dt = new DataTable();
                DbHelper.Fill(dt, cmd);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar el reporte: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public DataTable SociosPorEstado(string estado) => Ejecutar(
            "SELECT s.nro_socio AS 'N° Socio', p.apellido AS Apellido, p.nombre AS Nombre, " +
            "       s.fecha_alta AS 'Fecha alta', s.estado AS Estado " +
            "FROM socio s JOIN persona p ON p.id_persona = s.id_persona " +
            "WHERE s.estado = @estado ORDER BY p.apellido",
            new SqliteParameter("@estado", estado));

        public DataTable RecaudacionDelMes(int mes, int anio) => Ejecutar(
            "SELECT p.apellido AS Apellido, p.nombre AS Nombre, c.forma_pago AS 'Forma de pago', " +
            "       c.cuotas_tarjeta AS Cuotas, c.monto AS Monto, c.fecha_pago AS 'Fecha de pago' " +
            "FROM cuota c JOIN socio s ON s.id_socio = c.id_socio JOIN persona p ON p.id_persona = s.id_persona " +
            "WHERE strftime('%m', c.fecha_pago) = printf('%02d', @mes) AND strftime('%Y', c.fecha_pago) = CAST(@anio AS TEXT) " +
            "ORDER BY c.fecha_pago",
            new SqliteParameter("@mes", mes), new SqliteParameter("@anio", anio));

        public DataTable CarnetsEmitidos() => Ejecutar(
            "SELECT s.nro_socio AS 'N° Socio', p.apellido AS Apellido, p.nombre AS Nombre, " +
            "       c.fecha_emision AS Emisión, c.fecha_vencimiento AS Vencimiento " +
            "FROM carnet c JOIN socio s ON s.id_socio = c.id_socio JOIN persona p ON p.id_persona = s.id_persona " +
            "ORDER BY c.fecha_emision DESC");

        public DataTable ActividadesConCupo() => Ejecutar(
            "SELECT a.nombre AS Actividad, a.cupo_maximo AS 'Cupo máximo', " +
            "       (SELECT COUNT(*) FROM inscripcion i WHERE i.id_actividad = a.id_actividad) AS Inscriptos, " +
            "       (p.apellido || ', ' || p.nombre) AS Profesor " +
            "FROM actividad a LEFT JOIN profesor pr ON pr.id_profesor = a.id_profesor " +
            "LEFT JOIN persona p ON p.id_persona = pr.id_persona ORDER BY a.nombre");
    }
}
