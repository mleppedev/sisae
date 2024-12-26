# Implementación

Este documento describe la estructura del proyecto y el propósito de cada directorio.

## Estructura del Proyecto
- **Data/**: Contiene el contexto de la base de datos y las migraciones.
- **Services/**: Incluye servicios que encapsulan la lógica de negocio.
- **Models/**: Define las clases de modelo utilizadas en la aplicación.
- **Pages/**: Contiene las páginas de la aplicación si se trata de un proyecto Razor Pages.
- **Hubs/**: Relacionado con SignalR para comunicación en tiempo real.
- **wwwroot/**: Archivos estáticos como CSS, JavaScript e imágenes.

## Detalles de Implementación
- **Program.cs**: Configura el `DbContext` con la cadena de conexión y habilita el registro de datos sensibles.
- **Startup**: Configura servicios y middleware necesarios para la aplicación.

## Referencias
- `Data/` es utilizado en `Program.cs` para inicializar el contexto de la base de datos.
- `Services/` se inyecta en los controladores y hubs para realizar operaciones de negocio.
- `Models/` define las entidades como `Visitante`, `Visita`, etc., que son utilizadas en `ApplicationDbContext`.
- `Pages/` contiene las vistas Razor que interactúan con los controladores para mostrar datos al usuario. 

# Servicios

Este documento describe los servicios implementados en el proyecto y cómo interactúan entre sí.

## Descripción de Servicios
- **ApplicationDbContext**: Proporciona acceso a las entidades de la base de datos, incluyendo `AccesosProhibidos`, `Visitados`, `Visitantes`, `Visitas` y `EventLogs`.
- **EventLoggerService**: Servicio para registrar eventos importantes en el sistema.

## Interacción
- Los servicios interactúan con el contexto de la base de datos para realizar operaciones CRUD sobre las entidades.
- Utilizan `IdentityDbContext` para gestionar la autenticación y autorización de usuarios.

## Referencias
- `ApplicationDbContext` es utilizado en `DashboardHub` para acceder a los datos.
- `EventLoggerService` se utiliza en `DashboardHub` para registrar eventos de acceso y visitas.
- Los servicios son inyectados en los controladores para realizar operaciones de negocio. 

# Librerías

Este documento lista las librerías y paquetes utilizados en el proyecto, junto con su propósito.

## Lista de Librerías
- **chart.js**: Utilizado para crear gráficos interactivos en la aplicación.
- **EntityFrameworkCore**: Utilizado para interactuar con la base de datos de manera eficiente.
- **SignalR**: Para comunicación en tiempo real entre el servidor y los clientes.

## Referencias
- `chart.js` se utiliza en las páginas de la aplicación para mostrar datos visuales.
- `EntityFrameworkCore` se utiliza en `ApplicationDbContext` para gestionar las operaciones de base de datos.
- `SignalR` se utiliza en `DashboardHub` para enviar actualizaciones en tiempo real a los clientes. 

# Migraciones

Este documento detalla las migraciones de base de datos y cómo aplicarlas.

## Detalles de Migraciones
- Las migraciones se utilizan para actualizar el esquema de la base de datos de manera controlada.
- Incluyen cambios en las tablas `AccesosProhibidos`, `Visitados`, `Visitantes`, y `Visitas`.

## Aplicación de Migraciones
- Utilizar el comando `Update-Database` en la consola de administración de paquetes para aplicar las migraciones.
- Asegurarse de que la cadena de conexión esté correctamente configurada en `appsettings.json`.

## Referencias
- Las migraciones se generan y aplican utilizando `EntityFrameworkCore` en el contexto de `ApplicationDbContext`.
- Los modelos definidos en `Models/` son utilizados para crear y actualizar las tablas de la base de datos. 

# Hubs

Este documento describe los hubs de SignalR utilizados en el proyecto y sus casos de uso.

## Descripción de Hubs
- **DashboardHub**: Proporciona actualizaciones en tiempo real sobre las entidades `Visita` y `AccesoProhibido`.

## Casos de Uso
- **SendVisitaUpdate**: Envía actualizaciones sobre visitas a los clientes conectados.
- **SendAccesoProhibidoUpdate**: Envía actualizaciones sobre accesos prohibidos a los clientes conectados.
- Utiliza `ApplicationDbContext` para acceder a los datos y `EventLoggerService` para registrar eventos.

## Referencias
- `DashboardHub` se utiliza en la aplicación para gestionar la comunicación en tiempo real con los clientes.
- Los hubs interactúan con los servicios y el contexto de la base de datos para enviar actualizaciones en tiempo real. 