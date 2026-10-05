using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using ClubDeportivo.Entidades;

namespace ClubDeportivo.Datos
{
    /// <summary>Acceso a datos de los carnets entregados a los socios.</summary>
    public class CarnetDAO
    {
        public int Emitir(int idSocio, DateTime fechaEmision, DateTime fechaVencimiento)
        {
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "INSERT INTO carnet (id_socio, fecha_emision, fecha_vencimiento) VALUES (@id,@emision,@venc); SELECT last_insert_rowid();", con);
                cmd.Parameters.AddWithValue("@id", idSocio);
                cmd.Parameters.AddWithValue("@emision", fechaEmision.ToString("yyyy-MM-dd"));
                cmd.Parameters.AddWithValue("@venc", fechaVencimiento.ToString("yyyy-MM-dd"));
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception("Error al emitir el carnet: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }

        public List<Carnet> Listar()
        {
            var lista = new List<Carnet>();
            var cn = new Conexion();
            try
            {
                var con = cn.Abrir();
                var cmd = new SqliteCommand(
                    "SELECT c.id_carnet, c.id_socio, c.fecha_emision, c.fecha_vencimiento, s.nro_socio, " +
                    "       (p.apellido || ', ' || p.nombre) AS titular " +
                    "FROM carnet c JOIN socio s ON s.id_socio = c.id_socio JOIN persona p ON p.id_persona = s.id_persona " +
                    "ORDER BY c.fecha_emision DESC", con);
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        lista.Add(new Carnet
                        {
                            IdCarnet = DbHelper.Int(r, "id_carnet"),
                            IdSocio = DbHelper.Int(r, "id_socio"),
                            FechaEmision = DbHelper.Fecha(r, "fecha_emision"),
                            FechaVencimiento = DbHelper.Fecha(r, "fecha_vencimiento")
                        });
                    }
                }
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al listar los carnets: " + ex.Message, ex);
            }
            finally
            {
                cn.Cerrar();
            }
        }
    }
}
