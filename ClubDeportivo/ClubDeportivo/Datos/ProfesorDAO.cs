using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Datos
{
    /// <summary>Acceso a datos del plantel de profesores (titulares y suplentes).</summary>
    public class ProfesorDAO
    {
        private const string SELECT_BASE =
            "SELECT pr.id_profesor, p.id_persona, p.nombre, p.apellido, p.dni, p.fecha_nacimiento, " +
            "       p.telefono, p.direccion, p.email, pr.legajo, pr.especialidad, pr.tipo, pr.horarios_asignados " +
            "FROM profesor pr JOIN persona p ON p.id_persona = pr.id_persona ";

        private static Profesor Mapear(IDataRecord r) => new Profesor
        {
            IdProfesor = DbHelper.Int(r, "id_profesor"),
            Id = DbHelper.Int(r, "id_persona"),
            Nombre = DbHelper.Str(r, "nombre"),
            Apellido = DbHelper.Str(r, "apellido"),
            Dni = DbHelper.Str(r, "dni"),
            FechaNacimiento = DbHelper.Fecha(r, "fecha_nacimiento"),
            Telefono = DbHelper.Str(r, "telefono"),
            Direccion = DbHelper.Str(r, "direccion"),
            Email = DbHelper.Str(r, "email"),
            Legajo = DbHelper.Str(r, "legajo"),
            Especialidad = DbHelper.Str(r, "especialidad"),
            Tipo = DbHelper.Str(r, "tipo"),
            HorariosAsignados = DbHelper.Str(r, "horarios_asignados")
        };

        public int Alta(Profesor p)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var tx = con.BeginTransaction();
                try
                {
                    var cmdP = new SqliteCommand(
                        "INSERT INTO persona (nombre, apellido, dni, fecha_nacimiento, telefono, direccion, email) " +
                        "VALUES (@nombre,@apellido,@dni,@fecnac,@telefono,@direccion,@email); SELECT last_insert_rowid();", con, tx);
                    cmdP.Parameters.AddWithValue("@nombre", p.Nombre);
                    cmdP.Parameters.AddWithValue("@apellido", p.Apellido);
                    cmdP.Parameters.AddWithValue("@dni", p.Dni);
                    cmdP.Parameters.AddWithValue("@fecnac", p.FechaNacimiento);
                    cmdP.Parameters.AddWithValue("@telefono", (object)p.Telefono ?? "");
                    cmdP.Parameters.AddWithValue("@direccion", (object)p.Direccion ?? "");
                    cmdP.Parameters.AddWithValue("@email", (object)p.Email ?? "");
                    int idPersona = Convert.ToInt32(cmdP.ExecuteScalar());

                    var cmdPr = new SqliteCommand(
                        "INSERT INTO profesor (id_persona, legajo, especialidad, tipo, horarios_asignados) " +
                        "VALUES (@idp,@legajo,@especialidad,@tipo,@horarios); SELECT last_insert_rowid();", con, tx);
                    cmdPr.Parameters.AddWithValue("@idp", idPersona);
                    cmdPr.Parameters.AddWithValue("@legajo", p.Legajo);
                    cmdPr.Parameters.AddWithValue("@especialidad", (object)p.Especialidad ?? "");
                    cmdPr.Parameters.AddWithValue("@tipo", p.Tipo);
                    cmdPr.Parameters.AddWithValue("@horarios", (object)p.HorariosAsignados ?? "");
                    int idProfesor = Convert.ToInt32(cmdPr.ExecuteScalar());
                    tx.Commit();
                    return idProfesor;
                }
                catch { tx.Rollback(); throw; }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al dar de alta al profesor: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public void Modificar(Profesor p)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var tx = con.BeginTransaction();
                try
                {
                    var cmdP = new SqliteCommand(
                        "UPDATE persona SET nombre=@nombre, apellido=@apellido, dni=@dni, fecha_nacimiento=@fecnac, " +
                        "telefono=@telefono, direccion=@direccion, email=@email WHERE id_persona=@idpersona", con, tx);
                    cmdP.Parameters.AddWithValue("@nombre", p.Nombre);
                    cmdP.Parameters.AddWithValue("@apellido", p.Apellido);
                    cmdP.Parameters.AddWithValue("@dni", p.Dni);
                    cmdP.Parameters.AddWithValue("@fecnac", p.FechaNacimiento);
                    cmdP.Parameters.AddWithValue("@telefono", (object)p.Telefono ?? "");
                    cmdP.Parameters.AddWithValue("@direccion", (object)p.Direccion ?? "");
                    cmdP.Parameters.AddWithValue("@email", (object)p.Email ?? "");
                    cmdP.Parameters.AddWithValue("@idpersona", p.Id);
                    cmdP.ExecuteNonQuery();

                    var cmdPr = new SqliteCommand(
                        "UPDATE profesor SET legajo=@legajo, especialidad=@especialidad, tipo=@tipo, " +
                        "horarios_asignados=@horarios WHERE id_profesor=@idprofesor", con, tx);
                    cmdPr.Parameters.AddWithValue("@legajo", p.Legajo);
                    cmdPr.Parameters.AddWithValue("@especialidad", (object)p.Especialidad ?? "");
                    cmdPr.Parameters.AddWithValue("@tipo", p.Tipo);
                    cmdPr.Parameters.AddWithValue("@horarios", (object)p.HorariosAsignados ?? "");
                    cmdPr.Parameters.AddWithValue("@idprofesor", p.IdProfesor);
                    cmdPr.ExecuteNonQuery();
                    tx.Commit();
                }
                catch { tx.Rollback(); throw; }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el profesor: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public void Baja(int idProfesor)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand("DELETE FROM profesor WHERE id_profesor=@id", con);
                cmd.Parameters.AddWithValue("@id", idProfesor);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al dar de baja al profesor (verifique que no tenga actividades, rutinas o sueldos asociados): " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public Profesor BuscarPorId(int idProfesor)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(SELECT_BASE + "WHERE pr.id_profesor=@id", con);
                cmd.Parameters.AddWithValue("@id", idProfesor);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? Mapear(r) : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar el profesor: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public List<Profesor> Listar()
        {
            var lista = new List<Profesor>();
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(SELECT_BASE + "ORDER BY p.apellido, p.nombre", con);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) lista.Add(Mapear(r));
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los profesores: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public List<Profesor> ListarSuplentes()
        {
            var lista = new List<Profesor>();
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(SELECT_BASE + "WHERE pr.tipo='Suplente' ORDER BY p.apellido, p.nombre", con);
                using (var r = cmd.ExecuteReader())
                    while (r.Read()) lista.Add(Mapear(r));
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los suplentes: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }
    }
}
