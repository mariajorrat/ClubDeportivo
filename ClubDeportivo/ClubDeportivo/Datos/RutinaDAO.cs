using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Datos
{
    /// <summary>Acceso a datos de las rutinas que los profesores confeccionan para sus alumnos.</summary>
    public class RutinaDAO
    {
        private static Rutina Mapear(IDataRecord r) => new Rutina
        {
            IdRutina = DbHelper.Int(r, "id_rutina"),
            IdProfesor = DbHelper.Int(r, "id_profesor"),
            IdSocio = DbHelper.Int(r, "id_socio"),
            Descripcion = DbHelper.Str(r, "descripcion"),
            Fecha = DbHelper.Fecha(r, "fecha")
        };

        public int Alta(Rutina rt)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "INSERT INTO rutina (id_profesor, id_socio, descripcion, fecha) VALUES (@idp,@ids,@desc,@fecha); " +
                    "SELECT last_insert_rowid();", con);
                cmd.Parameters.AddWithValue("@idp", rt.IdProfesor);
                cmd.Parameters.AddWithValue("@ids", rt.IdSocio);
                cmd.Parameters.AddWithValue("@desc", rt.Descripcion);
                cmd.Parameters.AddWithValue("@fecha", rt.Fecha);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar la rutina: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public void Modificar(Rutina rt)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "UPDATE rutina SET descripcion=@desc, fecha=@fecha WHERE id_rutina=@id", con);
                cmd.Parameters.AddWithValue("@desc", rt.Descripcion);
                cmd.Parameters.AddWithValue("@fecha", rt.Fecha);
                cmd.Parameters.AddWithValue("@id", rt.IdRutina);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar la rutina: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public void Baja(int idRutina)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand("DELETE FROM rutina WHERE id_rutina=@id", con);
                cmd.Parameters.AddWithValue("@id", idRutina);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la rutina: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public List<Rutina> ListarPorSocio(int idSocio)
        {
            var lista = new List<Rutina>();
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT id_rutina, id_profesor, id_socio, descripcion, fecha FROM rutina " +
                    "WHERE id_socio=@id ORDER BY fecha DESC", con);
                cmd.Parameters.AddWithValue("@id", idSocio);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) lista.Add(Mapear(r));
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar las rutinas del socio: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public List<Rutina> ListarPorProfesor(int idProfesor)
        {
            var lista = new List<Rutina>();
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT id_rutina, id_profesor, id_socio, descripcion, fecha FROM rutina " +
                    "WHERE id_profesor=@id ORDER BY fecha DESC", con);
                cmd.Parameters.AddWithValue("@id", idProfesor);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) lista.Add(Mapear(r));
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar las rutinas del profesor: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }
    }
}
