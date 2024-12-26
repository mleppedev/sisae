# Esquema de la Base de Datos

## Diagrama de Relaciones entre Entidades (ERD)
(Incluir imagen del ERD)

## Descripción de las Tablas

### Visitas
- **ID_Visita**: Clave primaria, identificador único para cada visita.
- **ID_Visitante**: Clave foránea que referencia `Visitantes.ID_Visitante`.
- **ID_Visitado**: Clave foránea que referencia `Visitados.ID_Visitado`.
- **Hora_Entrada**: Hora de entrada.
- **Hora_Salida**: Hora de salida.
- **Fecha_Visita**: Fecha de la visita.
- **Motivo_Visita**: Razón de la visita.
- **Estado**: Estado de la visita (por ejemplo, Activa, Completada, Cancelada).
- **Comentarios**: Comentarios adicionales sobre la visita.

### Visitantes
- **ID_Visitante**: Clave primaria, identificador único para cada visitante.
- **Nombre**: Nombre del visitante.
- **Apellido**: Apellido del visitante.
- **Direccion**: Dirección del visitante.
- **Email**: Correo electrónico del visitante.
- **FechaNacimiento**: Fecha de nacimiento del visitante.
- **FechaVencimientoCarnet**: Fecha de vencimiento de la cédula del visitante.
- **Nacionalidad**: Nacionalidad del visitante.
- **RUT**: Número único de identificación nacional.
- **Telefono**: Número de teléfono del visitante.

### Visitados
- **ID_Visitado**: Clave primaria, identificador único para cada persona visitada.
- **Nombre**: Nombre de la persona visitada.
- **Apellido**: Apellido de la persona visitada.
- **Cargo**: Cargo o rol de la persona visitada.
- **Departamento**: Departamento de la persona visitada.
- **Email**: Correo electrónico de la persona visitada.
- **Telefono**: Número de teléfono de la persona visitada.

### Accesos Prohibidos
- **ID_Acceso_Prohibido**: Clave primaria, identificador único para cada registro de acceso restringido.
- **ID_Visitante**: Clave foránea que referencia `Visitantes.ID_Visitante`.
- **Fecha_Prohibicion**: Fecha en la que se prohibió el acceso.
- **Fecha_Expiracion**: Fecha de expiración de la restricción.
- **Motivo**: Razón por la cual se restringió el acceso.

### Relaciones y Restricciones
- Los **Visitantes** y **Visitados** están vinculados a través de relaciones de claves foráneas en la tabla `Visitas`.
- La tabla **AccesosProhibidos** puede referenciar a `Visitantes` para rastrear individuos con acceso restringido.

## Consultas SQL de Ejemplo

### Obtener todos los visitantes con restricciones activas
```sql
SELECT v.Nombre, v.Apellido, a.Fecha_Prohibicion, a.Motivo
FROM Visitantes v
JOIN AccesosProhibidos a ON v.ID_Visitante = a.ID_Visitante
WHERE a.Fecha_Expiracion > GETDATE();
