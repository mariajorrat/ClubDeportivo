using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Datos
{
    /// <summary>Acceso a datos de los turnos de nutrición (ficha médica / carga de actividad permitida).</summary>
    public class ConsultaNutricionDAO
    {
        private static ConsultaNutricion Mapear(IDataRecord r) => new ConsultaNutricion
        {
            IdConsulta = DbHelper.Int(r, "id_consulta"),
            IdSocio = DbHelper.Int(r, "id_socio"),
            Fecha = DbHelper.Fecha(r, "fecha"),
            Hora = DbHelper.Hora(r, "hora"),
            Turno = DbHelper.Int(r, "turno"),
            Observaciones = DbHelper.Str(r, "observaciones"),
            CargaActividadPermitida = DbHelper.Str(r, "carga_actividad_permitida")
        };

        public int Asignar(ConsultaNutricion c)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "INSERT INTO consulta_nutricion (id_socio, fecha, hora, turno, observaciones, carga_actividad_permitida) " +
                    "VALUES (@id,@fecha,@hora,@turno,@obs,@carga); SELECT last_insert_rowid();", con);
                cmd.Parameters.AddWithValue("@id", c.IdSocio);
                cmd.Parameters.AddWithValue("@fecha", c.Fecha.Date);
                cmd.Parameters.AddWithValue("@hora", c.Hora);
                cmd.Parameters.AddWithValue("@turno", c.Turno);
                cmd.Parameters.AddWithValue("@obs", (object)c.Observaciones ?? "");
                cmd.Parameters.AddWithValue("@carga", (object)c.CargaActividadPermitida ?? "");
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Error al asignar el turno de nutrición: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public void Modificar(ConsultaNutricion c)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "UPDATE consulta_nutricion SET fecha=@fecha, hora=@hora, turno=@turno, observaciones=@obs, " +
                    "carga_actividad_permitida=@carga WHERE id_consulta=@id", con);
                cmd.Parameters.AddWithValue("@fecha", c.Fecha.Date);
                cmd.Parameters.AddWithValue("@hora", c.Hora);
                cmd.Parameters.AddWithValue("@turno", c.Turno);
                cmd.Parameters.AddWithValue("@obs", (object)c.Observaciones ?? "");
                cmd.Parameters.AddWithValue("@carga", (object)c.CargaActividadPermitida ?? "");
                cmd.Parameters.AddWithValue("@id", c.IdConsulta);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el turno de nutrición: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public void Cancelar(int idConsulta)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand("DELETE FROM consulta_nutricion WHERE id_consulta=@id", con);
                cmd.Parameters.AddWithValue("@id", idConsulta);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al cancelar el turno: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public List<ConsultaNutricion> ListarPorSocio(int idSocio)
        {
            var lista = new List<ConsultaNutricion>();
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT id_consulta, id_socio, fecha, hora, turno, observaciones, carga_actividad_permitida " +
                    "FROM consulta_nutricion WHERE id_socio=@id ORDER BY fecha DESC", con);
                cmd.Parameters.AddWithValue("@id", idSocio);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) lista.Add(Mapear(r));
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los turnos del socio: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public int ContarTurnosOcupados(DateTime fecha)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand("SELECT COUNT(*) FROM consulta_nutricion WHERE fecha=@fecha", con);
                cmd.Parameters.AddWithValue("@fecha", fecha.Date);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Error al contar los turnos: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }
    }
}
