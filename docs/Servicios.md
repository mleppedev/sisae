# Servicios

Este documento describe los servicios implementados en el proyecto y cómo interactúan entre sí.

## Descripción de Servicios
- **ApplicationDbContext**: Proporciona acceso a las entidades de la base de datos, incluyendo `AccesosProhibidos`, `Visitados`, `Visitantes`, `Visitas` y `EventLogs`.
- **EventLoggerService**: Servicio para registrar eventos importantes en el sistema.

## Interacción
- Los servicios interactúan con el contexto de la base de datos para realizar operaciones CRUD sobre las entidades.
- Utilizan `IdentityDbContext` para gestionar la autenticación y autorización de usuarios. 