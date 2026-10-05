using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Datos
{
    /// <summary>Acceso a datos de usuarios del sistema (login y control de acceso por rol).</summary>
    public class UsuarioDAO
    {
        /// <summary>Devuelve el usuario activo por nombre para que la capa de negocio verifique su contraseña.</summary>
        public Usuario BuscarActivo(string nombreUsuario)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT id_usuario, nombre_usuario, contrasena_hash, rol, id_persona, activo FROM usuario " +
                "WHERE nombre_usuario=@user AND activo=1", con);
                cmd.Parameters.AddWithValue("@user", nombreUsuario);
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;
                    return new Usuario
                    {
                        IdUsuario = DbHelper.Int(r, "id_usuario"),
                        NombreUsuario = DbHelper.Str(r, "nombre_usuario"),
                        ContrasenaHash = DbHelper.Str(r, "contrasena_hash"),
                        Rol = DbHelper.Str(r, "rol"),
                        IdPersona = DbHelper.IntNull(r, "id_persona"),
                        Activo = DbHelper.Bool(r, "activo")
                    };
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar las credenciales: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public int Alta(Usuario u)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "INSERT INTO usuario (nombre_usuario, contrasena_hash, rol, id_persona, activo) " +
                    "VALUES (@user,@hash,@rol,@idpersona,1); SELECT last_insert_rowid();", con);
                cmd.Parameters.AddWithValue("@user", u.NombreUsuario);
                cmd.Parameters.AddWithValue("@hash", u.ContrasenaHash);
                cmd.Parameters.AddWithValue("@rol", u.Rol);
                cmd.Parameters.AddWithValue("@idpersona", (object)u.IdPersona ?? DBNull.Value);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear el usuario: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public List<Usuario> Listar()
        {
            var lista = new List<Usuario>();
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT id_usuario, nombre_usuario, contrasena_hash, rol, id_persona, activo FROM usuario ORDER BY nombre_usuario", con);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new Usuario
                        {
                            IdUsuario = DbHelper.Int(r, "id_usuario"),
                            NombreUsuario = DbHelper.Str(r, "nombre_usuario"),
                            ContrasenaHash = DbHelper.Str(r, "contrasena_hash"),
                            Rol = DbHelper.Str(r, "rol"),
                            IdPersona = DbHelper.IntNull(r, "id_persona"),
                            Activo = DbHelper.Bool(r, "activo")
                        });
                    }
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los usuarios: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }
    }
}
