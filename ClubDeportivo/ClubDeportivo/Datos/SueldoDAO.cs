using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Datos
{
    /// <summary>Acceso a datos de las liquidaciones de sueldo de los profesores.</summary>
    public class SueldoDAO
    {
        /// <summary>Liquida (o actualiza) el sueldo del período.</summary>
        public void Liquidar(int idProfesor, int mes, int anio, decimal monto)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand("INSERT INTO sueldo(id_profesor,mes,anio,monto,fecha_pago) VALUES(@id,@mes,@anio,@monto,@fecha)", con);
                cmd.Parameters.AddWithValue("@id", idProfesor);
                cmd.Parameters.AddWithValue("@mes", mes);
                cmd.Parameters.AddWithValue("@anio", anio);
                cmd.Parameters.AddWithValue("@monto", monto);
                cmd.Parameters.AddWithValue("@fecha", DateTime.Today.ToString("yyyy-MM-dd"));
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al liquidar el sueldo: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public List<Sueldo> ListarPorProfesor(int idProfesor)
        {
            var lista = new List<Sueldo>();
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT id_sueldo, id_profesor, mes, anio, monto, fecha_pago FROM sueldo " +
                    "WHERE id_profesor=@id ORDER BY anio DESC, mes DESC", con);
                cmd.Parameters.AddWithValue("@id", idProfesor);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new Sueldo
                        {
                            IdSueldo = DbHelper.Int(r, "id_sueldo"),
                            IdProfesor = DbHelper.Int(r, "id_profesor"),
                            Mes = DbHelper.Int(r, "mes"),
                            Anio = DbHelper.Int(r, "anio"),
                            Monto = DbHelper.Decimal(r, "monto"),
                            FechaPago = DbHelper.FechaNull(r, "fecha_pago")
                        });
                    }
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los sueldos del profesor: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public DataTable ListarPorPeriodo(int mes, int anio)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT (p.apellido || ', ' || p.nombre) AS Profesor, pr.legajo AS Legajo, " +
                    "       s.monto AS Monto, s.fecha_pago AS FechaDePago FROM sueldo s " +
                    "JOIN profesor pr ON pr.id_profesor = s.id_profesor " +
                    "JOIN persona p ON p.id_persona = pr.id_persona " +
                    "WHERE s.mes=@mes AND s.anio=@anio ORDER BY p.apellido", con);
                cmd.Parameters.AddWithValue("@mes", mes);
                cmd.Parameters.AddWithValue("@anio", anio);
                var dt = new DataTable();
                DbHelper.Fill(dt, cmd);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los sueldos del período: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }
    }
}
