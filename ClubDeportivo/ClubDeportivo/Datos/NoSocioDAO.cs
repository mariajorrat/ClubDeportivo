using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Datos
{
    /// <summary>Acceso a datos de personas que no son socias (pagan solo la actividad que realizan).</summary>
    public class NoSocioDAO
    {
        /// <summary>Busca una persona ya registrada como no socio por DNI; devuelve su id_persona o null.</summary>
        public int? BuscarIdPersonaPorDni(string dni)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand("SELECT id_persona FROM persona WHERE dni=@dni", con);
                cmd.Parameters.AddWithValue("@dni", dni);
                object res = cmd.ExecuteScalar();
                return res == null ? (int?)null : Convert.ToInt32(res);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al buscar la persona: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        /// <summary>Crea la persona (si no existía) y su registro de no socio. Devuelve id_persona.</summary>
        public int Alta(NoSocio n)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                int idPersona;

                var cmdBuscar = new SqliteCommand("SELECT id_persona FROM persona WHERE dni=@dni", con);
                cmdBuscar.Parameters.AddWithValue("@dni", n.Dni);
                object existente = cmdBuscar.ExecuteScalar();

                if (existente != null)
                {
                    idPersona = Convert.ToInt32(existente);
                }
                else
                {
                    var cmdIns = new SqliteCommand(
                        "INSERT INTO persona (nombre, apellido, dni, fecha_nacimiento, telefono, direccion, email) " +
                        "VALUES (@nombre,@apellido,@dni,@fecnac,@telefono,@direccion,@email); SELECT last_insert_rowid();", con);
                    cmdIns.Parameters.AddWithValue("@nombre", n.Nombre);
                    cmdIns.Parameters.AddWithValue("@apellido", n.Apellido);
                    cmdIns.Parameters.AddWithValue("@dni", n.Dni);
                    cmdIns.Parameters.AddWithValue("@fecnac", n.FechaNacimiento);
                    cmdIns.Parameters.AddWithValue("@telefono", (object)n.Telefono ?? "");
                    cmdIns.Parameters.AddWithValue("@direccion", (object)n.Direccion ?? "");
                    cmdIns.Parameters.AddWithValue("@email", (object)n.Email ?? "");
                    idPersona = Convert.ToInt32(cmdIns.ExecuteScalar());

                    var cmdNoSocio = new SqliteCommand("INSERT INTO no_socio (id_persona) VALUES (@idp)", con);
                    cmdNoSocio.Parameters.AddWithValue("@idp", idPersona);
                    cmdNoSocio.ExecuteNonQuery();
                }
                return idPersona;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar el no socio: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public List<NoSocio> Listar()
        {
            var lista = new List<NoSocio>();
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT ns.id_no_socio, p.id_persona, p.nombre, p.apellido, p.dni, p.fecha_nacimiento, " +
                    "       p.telefono, p.direccion, p.email, " +
                    "       MAX(i.fecha) AS ultima_visita, " +
                    "       (SELECT a.nombre FROM inscripcion i2 JOIN actividad a ON a.id_actividad = i2.id_actividad " +
                    "        WHERE i2.id_persona = p.id_persona ORDER BY i2.fecha DESC LIMIT 1) AS actividad, " +
                    "       (SELECT i3.monto_pagado FROM inscripcion i3 WHERE i3.id_persona = p.id_persona " +
                    "        ORDER BY i3.fecha DESC LIMIT 1) AS monto " +
                    "FROM no_socio ns " +
                    "JOIN persona p ON p.id_persona = ns.id_persona " +
                    "LEFT JOIN inscripcion i ON i.id_persona = p.id_persona " +
                    "GROUP BY ns.id_no_socio, p.id_persona, p.nombre, p.apellido, p.dni, p.fecha_nacimiento, " +
                    "         p.telefono, p.direccion, p.email " +
                    "ORDER BY p.apellido, p.nombre", con);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new NoSocio
                        {
                            IdNoSocio = DbHelper.Int(r, "id_no_socio"),
                            Id = DbHelper.Int(r, "id_persona"),
                            Nombre = DbHelper.Str(r, "nombre"),
                            Apellido = DbHelper.Str(r, "apellido"),
                            Dni = DbHelper.Str(r, "dni"),
                            FechaNacimiento = DbHelper.Fecha(r, "fecha_nacimiento"),
                            Telefono = DbHelper.Str(r, "telefono"),
                            Direccion = DbHelper.Str(r, "direccion"),
                            Email = DbHelper.Str(r, "email"),
                            FechaVisita = DbHelper.FechaNull(r, "ultima_visita") ?? DateTime.MinValue,
                            ActividadRealizada = DbHelper.Str(r, "actividad"),
                            MontoPagado = DbHelper.Decimal(r, "monto")
                        });
                    }
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los no socios: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }
    }
}
