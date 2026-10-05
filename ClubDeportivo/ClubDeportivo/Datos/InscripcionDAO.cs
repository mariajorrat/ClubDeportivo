using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Datos
{
    /// <summary>Acceso a datos de inscripciones puntuales a actividades (usado principalmente por no socios).</summary>
    public class InscripcionDAO
    {
        public int Alta(Inscripcion i)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "INSERT INTO inscripcion (id_persona, id_actividad, fecha, tipo, apto_fisico, monto_pagado) " +
                    "VALUES (@idp,@ida,@fecha,@tipo,@apto,@monto); SELECT last_insert_rowid();", con);
                cmd.Parameters.AddWithValue("@idp", i.IdPersona);
                cmd.Parameters.AddWithValue("@ida", i.IdActividad);
                cmd.Parameters.AddWithValue("@fecha", i.Fecha);
                cmd.Parameters.AddWithValue("@tipo", i.Tipo);
                cmd.Parameters.AddWithValue("@apto", i.AptoFisico);
                cmd.Parameters.AddWithValue("@monto", i.MontoPagado);
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Error al registrar la inscripción: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public List<Inscripcion> ListarPorPersona(int idPersona)
        {
            var lista = new List<Inscripcion>();
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT id_inscripcion, id_persona, id_actividad, fecha, tipo, apto_fisico, monto_pagado " +
                    "FROM inscripcion WHERE id_persona=@id ORDER BY fecha DESC", con);
                cmd.Parameters.AddWithValue("@id", idPersona);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new Inscripcion
                        {
                            IdInscripcion = DbHelper.Int(r, "id_inscripcion"),
                            IdPersona = DbHelper.Int(r, "id_persona"),
                            IdActividad = DbHelper.Int(r, "id_actividad"),
                            Fecha = DbHelper.Fecha(r, "fecha"),
                            Tipo = DbHelper.Str(r, "tipo"),
                            AptoFisico = DbHelper.Bool(r, "apto_fisico"),
                            MontoPagado = DbHelper.Decimal(r, "monto_pagado")
                        });
                    }
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar las inscripciones: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }
    }
}
