using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Datos
{
    /// <summary>Acceso a datos de las actividades que ofrece el club.</summary>
    public class ActividadDAO
    {
        private const string SELECT_BASE =
            "SELECT a.id_actividad, a.nombre, a.tipo, a.costo_no_socio, a.horario, a.cupo_maximo, a.id_profesor, " +
            "       (p.apellido || ', ' || p.nombre) AS nombre_profesor " +
            "FROM actividad a " +
            "LEFT JOIN profesor pr ON pr.id_profesor = a.id_profesor " +
            "LEFT JOIN persona p ON p.id_persona = pr.id_persona ";

        private static Actividad Mapear(IDataRecord r) => new Actividad
        {
            IdActividad = DbHelper.Int(r, "id_actividad"),
            Nombre = DbHelper.Str(r, "nombre"),
            Tipo = DbHelper.Str(r, "tipo"),
            CostoNoSocio = DbHelper.Decimal(r, "costo_no_socio"),
            Horario = DbHelper.Str(r, "horario"),
            CupoMaximo = DbHelper.Int(r, "cupo_maximo"),
            IdProfesor = DbHelper.IntNull(r, "id_profesor"),
            NombreProfesor = DbHelper.Str(r, "nombre_profesor")
        };

        public int Alta(Actividad a)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "INSERT INTO actividad (nombre, tipo, costo_no_socio, horario, cupo_maximo, id_profesor) " +
                    "VALUES (@nombre,@tipo,@costo,@horario,@cupo,@idprof); SELECT last_insert_rowid();", con);
                cmd.Parameters.AddWithValue("@nombre", a.Nombre);
                cmd.Parameters.AddWithValue("@tipo", (object)a.Tipo ?? "");
                cmd.Parameters.AddWithValue("@costo", a.CostoNoSocio);
                cmd.Parameters.AddWithValue("@horario", (object)a.Horario ?? "");
                cmd.Parameters.AddWithValue("@cupo", a.CupoMaximo);
                cmd.Parameters.AddWithValue("@idprof", (object)a.IdProfesor ?? DBNull.Value);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Error al dar de alta la actividad: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public void Modificar(Actividad a)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "UPDATE actividad SET nombre=@nombre, tipo=@tipo, costo_no_socio=@costo, horario=@horario, " +
                    "cupo_maximo=@cupo, id_profesor=@idprof WHERE id_actividad=@id", con);
                cmd.Parameters.AddWithValue("@nombre", a.Nombre);
                cmd.Parameters.AddWithValue("@tipo", (object)a.Tipo ?? "");
                cmd.Parameters.AddWithValue("@costo", a.CostoNoSocio);
                cmd.Parameters.AddWithValue("@horario", (object)a.Horario ?? "");
                cmd.Parameters.AddWithValue("@cupo", a.CupoMaximo);
                cmd.Parameters.AddWithValue("@idprof", (object)a.IdProfesor ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@id", a.IdActividad);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar la actividad: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public void Baja(int idActividad)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand("DELETE FROM actividad WHERE id_actividad=@id", con);
                cmd.Parameters.AddWithValue("@id", idActividad);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar la actividad (verifique que no tenga inscripciones asociadas): " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public List<Actividad> Listar()
        {
            var lista = new List<Actividad>();
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(SELECT_BASE + "ORDER BY a.nombre", con);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) lista.Add(Mapear(r));
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar las actividades: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }
    }
}
