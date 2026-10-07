using Microsoft.Data.Sqlite;
using System.Security.Cryptography;

namespace ClubDeportivo.Datos;

/// <summary>Opens the local SQLite database and creates its schema on first use.</summary>
public sealed class Conexion
{
    private SqliteConnection? conexion;
    private static readonly string DatabasePath =
        Environment.GetEnvironmentVariable("CLUBDEPORTIVO_DATABASE_PATH")
        ?? Path.Combine(AppContext.BaseDirectory, "clubdeportivo.db");

    public SqliteConnection Abrir()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(DatabasePath))!);
        conexion = new SqliteConnection($"Data Source={DatabasePath};Foreign Keys=True");
        conexion.Open();
        Inicializar(conexion);
        return conexion;
    }

    public void Cerrar() { conexion?.Dispose(); conexion = null; }

    private static void Inicializar(SqliteConnection con)
    {
        using var cmd = con.CreateCommand();
        cmd.CommandText = Schema;
        cmd.ExecuteNonQuery();
        CargarDatosIniciales(con);
    }

    private static void CargarDatosIniciales(SqliteConnection con)
    {
        using var transaction = con.BeginTransaction();
        using var cmd = con.CreateCommand();
        cmd.Transaction = transaction;
        cmd.CommandText = """
        CREATE TABLE IF NOT EXISTS app_metadata (clave TEXT PRIMARY KEY, valor TEXT NOT NULL);
        SELECT valor FROM app_metadata WHERE clave = 'datos_originales_v1';
        """;

        var loaded = cmd.ExecuteScalar();
        if (loaded is not null)
        {
            transaction.Commit();
            return;
        }

        cmd.CommandText = """
        INSERT OR IGNORE INTO persona (id_persona,nombre,apellido,dni,fecha_nacimiento,telefono,direccion,email) VALUES
        (1,'Juan','Pérez','30111222','1990-05-12','3624111222','San Martín 123','juan.perez@mail.com'),
        (2,'María','Gómez','32555666','1993-08-20','3624555666','Belgrano 456','maria.gomez@mail.com'),
        (3,'Carlos','López','28999888','1985-01-15','3624999888','Sarmiento 789','carlos.lopez@mail.com'),
        (4,'Lucía','Fernández','35222111','1996-11-02','3624222111','Rivadavia 234','lucia.fernandez@mail.com'),
        (5,'Diego','Martínez','27444333','1982-03-30','3624444333','Alberdi 654','diego.martinez@mail.com'),
        (6,'Ana','Ruiz','31777999','1991-07-08','3624777999','Moreno 321','ana.ruiz@mail.com'),
        (7,'Roberto','Sosa','25888777','1978-09-25','3624888777','Mitre 987','roberto.sosa@mail.com'),
        (8,'Valeria','Acosta','33666555','1994-02-14','3624666555','Necochea 159','valeria.acosta@mail.com');

        INSERT OR IGNORE INTO socio (id_socio,id_persona,nro_socio,fecha_alta,apto_fisico,estado,carnet_entregado) VALUES
        (1,1,'S-0001','2026-01-10',1,'Activo',1),(2,2,'S-0002','2026-02-05',1,'Activo',1),
        (3,3,'S-0003','2025-11-20',1,'Inactivo',0),(4,4,'S-0004','2026-06-01',0,'Activo',0);
        INSERT OR IGNORE INTO profesor (id_profesor,id_persona,legajo,especialidad,tipo,horarios_asignados) VALUES
        (1,5,'P-100','Musculación y Aparatos','Titular','Lun a Vie 08:00-14:00'),
        (2,6,'P-101','Funcional','Suplente','Sáb 09:00-13:00');
        INSERT OR IGNORE INTO no_socio (id_no_socio,id_persona) VALUES (1,7),(2,8);
        INSERT OR IGNORE INTO actividad (id_actividad,nombre,tipo,costo_no_socio,horario,cupo_maximo,id_profesor) VALUES
        (1,'Musculación','Sala de máquinas',3500,'Lun a Sáb 08:00-22:00',40,1),
        (2,'Funcional','Clase grupal',2800,'Lun/Mié/Vie 18:00-19:00',20,1),
        (3,'Zumba','Clase grupal',2500,'Mar/Jue 19:00-20:00',25,2),
        (4,'Natación adultos','Pileta',4000,'Lun a Vie 07:00-09:00',15,NULL);
        INSERT OR IGNORE INTO cuota (id_cuota,id_socio,mes,anio,monto,fecha_vencimiento,fecha_pago,forma_pago,cuotas_tarjeta) VALUES
        (1,1,9,2026,15000,'2026-10-15','2026-09-14','Efectivo',NULL),
        (2,2,9,2026,15000,'2026-10-02','2026-09-01','Tarjeta',3),
        (3,3,8,2026,15000,'2026-09-20','2026-08-20','Efectivo',NULL),
        (4,4,9,2026,15000,'2026-10-05','2026-09-25','Tarjeta',6);
        INSERT OR IGNORE INTO carnet (id_carnet,id_socio,fecha_emision,fecha_vencimiento) VALUES
        (1,1,'2026-01-10','2027-01-10'),(2,2,'2026-02-05','2027-02-05');
        INSERT OR IGNORE INTO rutina (id_rutina,id_profesor,id_socio,descripcion,fecha) VALUES
        (1,1,1,'Rutina de fuerza tren superior - 4 series x 12 rep. Press banca, remo, dominadas asistidas.','2026-09-15'),
        (2,1,2,'Rutina full body principiante - circuito de 5 estaciones, 3 vueltas.','2026-09-10');
        INSERT OR IGNORE INTO asistencia_profesor (id_asistencia,id_profesor,fecha,hora_entrada) VALUES
        (1,1,'2026-09-26','07:55:00'),(2,2,'2026-09-26','08:58:00');
        INSERT OR IGNORE INTO sueldo (id_sueldo,id_profesor,mes,anio,monto,fecha_pago) VALUES
        (1,1,8,2026,450000,'2026-08-31'),(2,2,8,2026,180000,'2026-08-31');
        INSERT OR IGNORE INTO consulta_nutricion (id_consulta,id_socio,fecha,hora,turno,observaciones,carga_actividad_permitida) VALUES
        (1,1,'2026-09-30','10:00:00',1,'Primer control. Sin patologías previas.','Actividad moderada, 3 veces por semana'),
        (2,2,'2026-09-30','10:30:00',2,'Seguimiento nutricional.','Actividad intensa, 5 veces por semana');
        """;
        cmd.ExecuteNonQuery();

        InsertUser(cmd, "admin", "admin123", "Administrador", null);
        InsertUser(cmd, "recepcion", "recep123", "Recepcionista", null);
        InsertUser(cmd, "profesor1", "prof123", "Profesor", 5);
        InsertUser(cmd, "nutricionista1", "nutri123", "Nutricionista", null);

        cmd.CommandText = "INSERT INTO app_metadata (clave, valor) VALUES ('datos_originales_v1', 'cargados')";
        cmd.ExecuteNonQuery();
        transaction.Commit();
    }

    private static void InsertUser(SqliteCommand cmd, string username, string password, string role, int? personId)
    {
        cmd.CommandText = """
        UPDATE usuario SET contrasena_hash = @hash, rol = @role, id_persona = @personId, activo = 1
        WHERE nombre_usuario = @username;
        INSERT INTO usuario (nombre_usuario, contrasena_hash, rol, id_persona, activo)
        SELECT @username, @hash, @role, @personId, 1
        WHERE changes() = 0;
        """;
        cmd.Parameters.Clear();
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@hash", HashPassword(password));
        cmd.Parameters.AddWithValue("@role", role);
        cmd.Parameters.AddWithValue("@personId", (object?)personId ?? DBNull.Value);
        cmd.ExecuteNonQuery();
    }

    private static string HashPassword(string password)
    {
        const int iterations = 600_000;
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, 32);
        return $"pbkdf2-sha256${iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
    }

    private const string Schema = """
    CREATE TABLE IF NOT EXISTS persona (id_persona INTEGER PRIMARY KEY AUTOINCREMENT, nombre TEXT NOT NULL, apellido TEXT NOT NULL, dni TEXT NOT NULL UNIQUE, fecha_nacimiento TEXT NOT NULL, telefono TEXT, direccion TEXT, email TEXT);
    CREATE TABLE IF NOT EXISTS socio (id_socio INTEGER PRIMARY KEY AUTOINCREMENT, id_persona INTEGER NOT NULL REFERENCES persona(id_persona), nro_socio TEXT NOT NULL UNIQUE, fecha_alta TEXT NOT NULL, apto_fisico INTEGER NOT NULL DEFAULT 0, estado TEXT NOT NULL DEFAULT 'Activo', carnet_entregado INTEGER NOT NULL DEFAULT 0);
    CREATE TABLE IF NOT EXISTS profesor (id_profesor INTEGER PRIMARY KEY AUTOINCREMENT, id_persona INTEGER NOT NULL REFERENCES persona(id_persona), legajo TEXT NOT NULL UNIQUE, especialidad TEXT, tipo TEXT NOT NULL DEFAULT 'Titular', horarios_asignados TEXT);
    CREATE TABLE IF NOT EXISTS no_socio (id_no_socio INTEGER PRIMARY KEY AUTOINCREMENT, id_persona INTEGER NOT NULL REFERENCES persona(id_persona));
    CREATE TABLE IF NOT EXISTS usuario (id_usuario INTEGER PRIMARY KEY AUTOINCREMENT, nombre_usuario TEXT NOT NULL UNIQUE, contrasena_hash TEXT NOT NULL, rol TEXT NOT NULL, id_persona INTEGER, activo INTEGER NOT NULL DEFAULT 1);
    CREATE TABLE IF NOT EXISTS actividad (id_actividad INTEGER PRIMARY KEY AUTOINCREMENT, nombre TEXT NOT NULL, tipo TEXT, costo_no_socio REAL NOT NULL DEFAULT 0, horario TEXT, cupo_maximo INTEGER NOT NULL DEFAULT 1, id_profesor INTEGER REFERENCES profesor(id_profesor));
    CREATE TABLE IF NOT EXISTS inscripcion (id_inscripcion INTEGER PRIMARY KEY AUTOINCREMENT, id_persona INTEGER NOT NULL, id_actividad INTEGER NOT NULL, fecha TEXT NOT NULL, tipo TEXT, apto_fisico INTEGER NOT NULL DEFAULT 0, monto_pagado REAL NOT NULL DEFAULT 0);
    CREATE TABLE IF NOT EXISTS asistencia_profesor (id_asistencia INTEGER PRIMARY KEY AUTOINCREMENT, id_profesor INTEGER NOT NULL, fecha TEXT NOT NULL, hora_entrada TEXT NOT NULL);
    CREATE TABLE IF NOT EXISTS carnet (id_carnet INTEGER PRIMARY KEY AUTOINCREMENT, id_socio INTEGER NOT NULL, fecha_emision TEXT NOT NULL, fecha_vencimiento TEXT NOT NULL);
    CREATE TABLE IF NOT EXISTS cuota (id_cuota INTEGER PRIMARY KEY AUTOINCREMENT, id_socio INTEGER NOT NULL, mes INTEGER NOT NULL, anio INTEGER NOT NULL, monto REAL NOT NULL, fecha_vencimiento TEXT NOT NULL, fecha_pago TEXT, forma_pago TEXT, cuotas_tarjeta INTEGER);
    CREATE TABLE IF NOT EXISTS consulta_nutricion (id_consulta INTEGER PRIMARY KEY AUTOINCREMENT, id_socio INTEGER NOT NULL, fecha TEXT NOT NULL, hora TEXT NOT NULL, turno INTEGER NOT NULL DEFAULT 1, observaciones TEXT, carga_actividad_permitida TEXT);
    CREATE TABLE IF NOT EXISTS rutina (id_rutina INTEGER PRIMARY KEY AUTOINCREMENT, id_profesor INTEGER NOT NULL, id_socio INTEGER NOT NULL, descripcion TEXT, fecha TEXT NOT NULL);
    CREATE TABLE IF NOT EXISTS sueldo (id_sueldo INTEGER PRIMARY KEY AUTOINCREMENT, id_profesor INTEGER NOT NULL, mes INTEGER NOT NULL, anio INTEGER NOT NULL, monto REAL NOT NULL, fecha_pago TEXT);
    """;
}
