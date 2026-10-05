# Guion para video — Proyecto Integrador

**Duración sugerida:** 10 a 25 minutos.

**Reparto sugerido**

| # | Bloque |
| 1 | Introducción, enunciado y arquitectura |
| 2 | Base de datos y capa de Datos |
| 3 | Demo en vivo del sistema |
| 4 | Diagramas UML, pruebas y conclusiones |

**Antes de grabar**
- Levantar MySQL y correr `proyecto_club.sql` (deja cargados los usuarios de prueba).
- Abrir `ClubDeportivo.sln` en Visual Studio y compilar una vez (F5) antes de grabar, para que no se vean errores de restauración de paquetes en cámara.
- Crear previamente un usuario de demostración local con una contraseña temporal que no se publique ni se reutilice.
- Decidir quién comparte pantalla en el bloque 3 (demo).

---

## Bloque 1 — Introducción, enunciado y arquitectura (Integrante 1)

**(Cámara/presentación, sin pantalla del sistema todavía)**

> "Hola, somos el grupo 13, y este es nuestro proyecto integrador: un sistema de administración para un club deportivo, desarrollado en C# con Windows Forms y base de datos MySQL.
> El club tiene socios y no socios. Los socios pagan una cuota mensual por adelantado, en efectivo o con tarjeta en 3 o 6 cuotas; los no socios pagan solo la actividad puntual que hacen ese día. Si a un socio se le vence la cuota, el sistema automáticamente le bloquea el acceso a las actividades hasta que vuelva a pagar. Además el club tiene profesores titulares y suplentes, que firman asistencia, arman rutinas y cobran un sueldo que se liquida el último día hábil del mes, y un servicio de nutrición que da turnos una vez por semana.
> Para resolver esto organizamos el sistema en **cuatro capas**: Presentación, con los 14 formularios de Windows Forms; Negocio, con las reglas del club que acabo de nombrar; Datos, con una clase Conexión y un DAO por cada tabla; y Entidades, las clases que representan cada dato. Esto nos permitió, por ejemplo, tener la regla de 'cómo se calcula el nuevo vencimiento de la cuota' en un solo lugar, sin repetirla en cada pantalla."

---

## Bloque 2 — Base de datos y capa de Datos (Integrante 2)

**(Compartir pantalla: MySQL Workbench o el archivo `proyecto_club.sql`, y el Explorador de Soluciones con la carpeta `Datos/`)**

> "La base se llama **Proyecto** y tiene 13 tablas: persona, socio, no_socio, profesor, actividad, cuota, carnet, rutina, y demás. Persona guarda los datos comunes, y socio, no_socio y profesor se relacionan con ella por clave foránea, para no repetir nombre, DNI o teléfono tres veces.
> Para las operaciones más importantes —dar de alta un socio, cobrar una cuota y listar los vencimientos del día— usamos **stored procedures**. Por ejemplo, `sp_CobrarCuota` calcula la nueva fecha de vencimiento: si el socio nunca pagó, el plazo arranca hoy; si ya tenía una cuota paga, el nuevo plazo arranca al día siguiente del vencimiento anterior, tal como pedía el enunciado.
> Del lado de C#, toda la conexión vive en la carpeta **Datos**, como pedía la consigna. Acá está la clase `Conexion`, con sus métodos `Abrir()` y `Cerrar()` y manejo de errores con try-catch-finally. Y acá un DAO, por ejemplo `CuotaDAO`: fíjense que llama al stored procedure con `CommandType.StoredProcedure`, y todas las consultas usan parámetros —nunca concatenamos texto del usuario— para evitar inyección SQL."

---

## Bloque 3 — Demo en vivo del sistema (Integrante 3)

**(Compartir pantalla: ejecutar `ClubDeportivo.exe` o F5 desde Visual Studio)**

> "Vamos a mostrar el sistema andando. Arranca con el login: entro como recepcionista..."

**Recorrido sugerido (ir narrando mientras se hace click):**
1. **Login** → entrar con `recepcion / recep123`. Mostrar que el menú cambia según el rol.
2. **Alta de socio** (`Gestión de socios`) → cargar un socio nuevo con un DNI y N° de socio que no existan, guardar, y mostrar que aparece en la grilla.
3. **Cobro de cuota** (`Cobro de cuota`) → buscar ese socio, cobrar la cuota en efectivo, mostrar el mensaje con la nueva fecha de vencimiento.
4. **Listado de vencimientos** (`Listado diario de vencimientos`) → elegir una fecha y mostrar el listado (requerimiento explícito del enunciado).
5. **Emisión de carnet** (`Emisión de carnet`) → mostrar que si el socio no tiene apto físico, el botón "Emitir carnet" queda deshabilitado; con un socio que sí lo tiene, emitirlo.
6. *(Opcional, si da el tiempo)* cerrar sesión y volver a entrar como `profesor1 / prof123` para mostrar que el menú ahora es distinto (solo asistencia y rutinas).

> "Como ven, cada pantalla valida los datos antes de guardar y avisa con un mensaje claro si algo está mal, por ejemplo si el DNI ya existe o si falta un campo obligatorio."

---

## Bloque 4 — Diagramas UML, pruebas y conclusiones (Integrante 4)

**(En pantalla: `DiagramasUML/diagrama_clases.png` y `diagrama_casos_uso.png`, y la sección 6 del documento Word/PDF)**

> "Para el análisis hicimos el diagrama de clases, donde se ve que Persona es una clase abstracta de la que heredan Socio, No Socio y Profesor, y sus relaciones con Cuota, Carnet, Rutina, Actividad, etc. También el diagrama de casos de uso, con los cinco actores: Administrador, Recepcionista, Profesor, Nutricionista y Socio.
> Armamos un juego de catorce pruebas —por ejemplo, cobrar una cuota con tarjeta en 6 cuotas, o tratar de emitir un carnet sin apto físico— y todas dieron el resultado esperado, están detalladas en la sección 6 del documento.
> Como conclusión, este trabajo nos permitió aplicar el ciclo completo de un proyecto de software: desde el análisis del enunciado hasta la puesta a punto, pasando por una arquitectura en capas que, por ejemplo, nos dejó implementar la regla del vencimiento de la cuota en un solo lugar sin tocar ningún formulario. Como mejora a futuro, pensamos en agregar notificaciones automáticas antes del vencimiento y un registro de auditoría de cambios.
> Muchas gracias por ver nuestro proyecto."

---
