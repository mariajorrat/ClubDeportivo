using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Datos
{
    /// <summary>Acceso a datos de la tabla socio (y persona asociada). Todas las consultas parametrizadas.</summary>
    public class SocioDAO
    {
        private const string SELECT_BASE =
            "SELECT s.id_socio, p.id_persona, p.nombre, p.apellido, p.dni, p.fecha_nacimiento, " +
            "       p.telefono, p.direccion, p.email, s.nro_socio, s.fecha_alta, s.apto_fisico, " +
            "       s.estado, s.carnet_entregado " +
            "FROM socio s JOIN persona p ON p.id_persona = s.id_persona ";

        private static Socio Mapear(IDataRecord r) => new Socio
        {
            IdSocio = DbHelper.Int(r, "id_socio"),
            Id = DbHelper.Int(r, "id_persona"),
            Nombre = DbHelper.Str(r, "nombre"),
            Apellido = DbHelper.Str(r, "apellido"),
            Dni = DbHelper.Str(r, "dni"),
            FechaNacimiento = DbHelper.Fecha(r, "fecha_nacimiento"),
            Telefono = DbHelper.Str(r, "telefono"),
            Direccion = DbHelper.Str(r, "direccion"),
            Email = DbHelper.Str(r, "email"),
            NroSocio = DbHelper.Str(r, "nro_socio"),
            FechaAlta = DbHelper.Fecha(r, "fecha_alta"),
            AptoFisico = DbHelper.Bool(r, "apto_fisico"),
            Estado = DbHelper.Str(r, "estado"),
            CarnetEntregado = DbHelper.Bool(r, "carnet_entregado")
        };

        /// <summary>Alta de socio (crea persona + socio) mediante stored procedure. Devuelve el id_socio generado.</summary>
        public int Alta(Socio s)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                using var tx = con.BeginTransaction();
                var persona = new SqliteCommand("INSERT INTO persona(nombre,apellido,dni,fecha_nacimiento,telefono,direccion,email) VALUES(@nombre,@apellido,@dni,@nac,@tel,@dir,@email); SELECT last_insert_rowid();", con, tx);
                persona.Parameters.AddWithValue("@nombre", s.Nombre); persona.Parameters.AddWithValue("@apellido", s.Apellido);
                persona.Parameters.AddWithValue("@dni", s.Dni); persona.Parameters.AddWithValue("@nac", s.FechaNacimiento.ToString("yyyy-MM-dd"));
                persona.Parameters.AddWithValue("@tel", s.Telefono ?? ""); persona.Parameters.AddWithValue("@dir", s.Direccion ?? ""); persona.Parameters.AddWithValue("@email", s.Email ?? "");
                var idPersona = Convert.ToInt32(persona.ExecuteScalar());
                var socio = new SqliteCommand("INSERT INTO socio(id_persona,nro_socio,fecha_alta,apto_fisico) VALUES(@id,@nro,@alta,@apto); SELECT last_insert_rowid();", con, tx);
                socio.Parameters.AddWithValue("@id", idPersona); socio.Parameters.AddWithValue("@nro", s.NroSocio);
                socio.Parameters.AddWithValue("@alta", s.FechaAlta == default ? DateTime.Today.ToString("yyyy-MM-dd") : s.FechaAlta.ToString("yyyy-MM-dd"));
                socio.Parameters.AddWithValue("@apto", s.AptoFisico ? 1 : 0);
                var id = Convert.ToInt32(socio.ExecuteScalar()); tx.Commit(); return id;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al dar de alta al socio: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public void Modificar(Socio s)
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
                    cmdP.Parameters.AddWithValue("@nombre", s.Nombre);
                    cmdP.Parameters.AddWithValue("@apellido", s.Apellido);
                    cmdP.Parameters.AddWithValue("@dni", s.Dni);
                    cmdP.Parameters.AddWithValue("@fecnac", s.FechaNacimiento);
                    cmdP.Parameters.AddWithValue("@telefono", (object)s.Telefono ?? "");
                    cmdP.Parameters.AddWithValue("@direccion", (object)s.Direccion ?? "");
                    cmdP.Parameters.AddWithValue("@email", (object)s.Email ?? "");
                    cmdP.Parameters.AddWithValue("@idpersona", s.Id);
                    cmdP.ExecuteNonQuery();

                    var cmdS = new SqliteCommand(
                        "UPDATE socio SET nro_socio=@nro, apto_fisico=@apto, estado=@estado, carnet_entregado=@carnet " +
                        "WHERE id_socio=@idsocio", con, tx);
                    cmdS.Parameters.AddWithValue("@nro", s.NroSocio);
                    cmdS.Parameters.AddWithValue("@apto", s.AptoFisico);
                    cmdS.Parameters.AddWithValue("@estado", s.Estado);
                    cmdS.Parameters.AddWithValue("@carnet", s.CarnetEntregado);
                    cmdS.Parameters.AddWithValue("@idsocio", s.IdSocio);
                    cmdS.ExecuteNonQuery();
                    tx.Commit();
                }
                catch { tx.Rollback(); throw; }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al modificar el socio: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        /// <summary>Baja lógica: el socio pasa a estado Inactivo (no se elimina el historial).</summary>
        public void Baja(int idSocio)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand("UPDATE socio SET estado='Inactivo' WHERE id_socio=@id", con);
                cmd.Parameters.AddWithValue("@id", idSocio);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al dar de baja al socio: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public void MarcarCarnetEntregado(int idSocio)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand("UPDATE socio SET carnet_entregado=1 WHERE id_socio=@id", con);
                cmd.Parameters.AddWithValue("@id", idSocio);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al actualizar la entrega del carnet: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public Socio BuscarPorId(int idSocio)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(SELECT_BASE + "WHERE s.id_socio=@id", con);
                cmd.Parameters.AddWithValue("@id", idSocio);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? Mapear(r) : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar el socio: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public Socio BuscarPorNroSocio(string nroSocio)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(SELECT_BASE + "WHERE s.nro_socio=@nro", con);
                cmd.Parameters.AddWithValue("@nro", nroSocio);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? Mapear(r) : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar el socio: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public Socio BuscarPorDni(string dni)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(SELECT_BASE + "WHERE p.dni=@dni", con);
                cmd.Parameters.AddWithValue("@dni", dni);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? Mapear(r) : null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar el socio: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public List<Socio> Listar()
        {
            var lista = new List<Socio>();
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
                throw new Exception("Error al listar los socios: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        /// <summary>Fecha de vencimiento de la última cuota abonada por el socio (null si nunca pagó).</summary>
        public DateTime? ObtenerUltimoVencimiento(int idSocio)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT MAX(fecha_vencimiento) FROM cuota WHERE id_socio=@id AND fecha_pago IS NOT NULL", con);
                cmd.Parameters.AddWithValue("@id", idSocio);
                object res = cmd.ExecuteScalar();
                return res == null || res == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(res);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al consultar el vencimiento del socio: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }
    }
}
