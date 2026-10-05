-- ClubDeportivo - SQLite schema and development data.
-- The application creates this schema automatically in clubdeportivo.db.
PRAGMA foreign_keys = ON;

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

-- Password is SHA-256("admin123").
INSERT OR IGNORE INTO usuario (nombre_usuario, contrasena_hash, rol, activo)
VALUES ('admin', '240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9', 'Administrador', 1);
INSERT OR IGNORE INTO persona (nombre, apellido, dni, fecha_nacimiento, telefono, direccion, email)
VALUES ('Administrador', 'Sistema', '00000000', '1990-01-01', '', '', 'admin@club.local');
INSERT OR IGNORE INTO profesor (id_persona, legajo, especialidad, tipo)
SELECT id_persona, 'ADM-001', 'Administración', 'Titular' FROM persona WHERE dni = '00000000'
AND NOT EXISTS (SELECT 1 FROM profesor WHERE legajo = 'ADM-001');
