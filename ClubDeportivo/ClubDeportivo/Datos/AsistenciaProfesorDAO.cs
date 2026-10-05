using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Datos
{
    /// <summary>Acceso a datos de la planilla de asistencia diaria de los profesores.</summary>
    public class AsistenciaProfesorDAO
    {
        public bool YaFirmoHoy(int idProfesor, DateTime fecha)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT COUNT(*) FROM asistencia_profesor WHERE id_profesor=@id AND fecha=@fecha", con);
                cmd.Parameters.AddWithValue("@id", idProfesor);
                cmd.Parameters.AddWithValue("@fecha", fecha.Date);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al verificar la asistencia: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public int Registrar(int idProfesor, DateTime fecha, TimeSpan horaEntrada)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "INSERT INTO asistencia_profesor (id_profesor, fecha, hora_entrada) VALUES (@id,@fecha,@hora); " +
                    "SELECT last_insert_rowid();", con);
                cmd.Parameters.AddWithValue("@id", idProfesor);
                cmd.Parameters.AddWithValue("@fecha", fecha.Date);
                cmd.Parameters.AddWithValue("@hora", horaEntrada);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar la asistencia: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public List<AsistenciaProfesor> ListarPorProfesor(int idProfesor)
        {
            var lista = new List<AsistenciaProfesor>();
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT id_asistencia, id_profesor, fecha, hora_entrada FROM asistencia_profesor " +
                    "WHERE id_profesor=@id ORDER BY fecha DESC", con);
                cmd.Parameters.AddWithValue("@id", idProfesor);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new AsistenciaProfesor
                        {
                            IdAsistencia = DbHelper.Int(r, "id_asistencia"),
                            IdProfesor = DbHelper.Int(r, "id_profesor"),
                            Fecha = DbHelper.Fecha(r, "fecha"),
                            HoraEntrada = DbHelper.Hora(r, "hora_entrada")
                        });
                    }
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar la asistencia del profesor: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public DataTable ListarPorFecha(DateTime fecha)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT (p.apellido || ', ' || p.nombre) AS Profesor, pr.legajo AS Legajo, " +
                    "       a.fecha AS Fecha, a.hora_entrada AS Hora FROM asistencia_profesor a " +
                    "JOIN profesor pr ON pr.id_profesor = a.id_profesor " +
                    "JOIN persona p ON p.id_persona = pr.id_persona " +
                    "WHERE a.fecha=@fecha ORDER BY a.hora_entrada", con);
                cmd.Parameters.AddWithValue("@fecha", fecha.Date);
                var dt = new DataTable();
                DbHelper.Fill(dt, cmd);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar la asistencia del día: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }
    }
}
