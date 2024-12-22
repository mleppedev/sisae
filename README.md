# SISAE - Sistema de Gestión de Visitantes

Este repositorio contiene el código fuente del proyecto SISAE, un sistema diseñado para gestionar la información de visitantes y el control de accesos de manera eficiente. Este documento proporciona instrucciones sobre cómo configurar, construir y ejecutar el proyecto desde cero.

## Tabla de Contenidos

1. [Requisitos Previos](#requisitos-previos)
2. [Configuración del Proyecto](#configuración-del-proyecto)
3. [Ejecución del Proyecto](#ejecución-del-proyecto)
4. [Estructura del Proyecto](#estructura-del-proyecto)
5. [Uso de Servicios Adicionales](#uso-de-servicios-adicionales)
6. [Uso de la API](#uso-de-la-api)
7. [Pruebas](#pruebas)
8. [Buenas Prácticas](#buenas-prácticas)
9. [Contribuciones](#contribuciones)
10. [Licencia](#licencia)

## Requisitos Previos

Asegúrate de tener las siguientes herramientas instaladas:

- .NET SDK 6.0 o superior
- Node.js y npm (para gestionar paquetes frontend)
- SQL Server u otro sistema de base de datos compatible con Entity Framework Core

## Configuración del Proyecto

1. **Clonar el repositorio:**

   ```bash
   git clone https://github.com/tu-usuario/tu-proyecto.git
   cd tu-proyecto
Configuración de la base de datos:

Configura tu servidor SQL y crea una base de datos para este proyecto. Actualiza la cadena de conexión en el archivo appsettings.json:

{
  "ConnectionStrings": {
    "DefaultConnection": "TuCadenaDeConexion"
  }
}

Restaura los paquetes de NuGet necesarios:

dotnet restore

Instala las dependencias de npm:

npm install

Configuración de secretos:
Si utilizas secretos adicionales, crea el archivo secrets.json y añade la información de configuración requerida.

Ejecución del Proyecto
Compilar el proyecto:

dotnet build

Migrar la base de datos:

Genera el esquema de la base de datos aplicando migraciones:

dotnet ef migrations add InitialCreate
dotnet ef database update

Ejecutar la aplicación:

dotnet run

Accede a la aplicación en http://localhost:5000.

Estructura del Proyecto
Controllers: Gestiona la lógica del servidor y las acciones de la API.
Models: Contiene las definiciones de las entidades utilizadas por Entity Framework.
Services: Incluye servicios como EventLoggerService y SignalRService para manejo de eventos y comunicaciones en tiempo real.
Pages: Usa Razor Pages para las vistas y la interfaz del usuario.
wwwroot: Archivos estáticos como CSS y JavaScript.
Migrations: Contiene los archivos de migración de Entity Framework para el versionado de la base de datos.
Uso de Servicios Adicionales
EventLoggerService: Registra eventos y errores del sistema para auditoría y seguimiento.
SignalRService: Proporciona capacidades de comunicación en tiempo real entre la aplicación y los clientes utilizando SignalR.
Uso de la API
Los endpoints disponibles se documentan en docs/api_documentation.md. Todos los endpoints requieren autenticación mediante tokens JWT.

Pruebas
Al ejecutar el proyecto en un entorno de desarrollo, puedes realizar pruebas de API y validaciones de lógicas de negocios usando herramientas como Postman o Swagger.

Scaffolding de Identity
La solución incluye scaffolding de ASP.NET Core Identity para gestionar la autenticación y autorización de usuarios. Los componentes de Identity se encuentran en el directorio Areas/Identity/.

Para actualizar el scaffolding de Identity, usa el siguiente comando:

dotnet aspnet-codegenerator identity -dc ApplicationDbContext

Buenas Prácticas
No compartas información sensible, especialmente credenciales, en commits.
Utiliza secretos de configuración y variables de entorno para gestionar información confidencial.
Actualiza el .gitignore conforme se agreguen nuevos archivos o carpetas que no deban ser versionados.

Contribuciones
Si deseas contribuir al proyecto, sigue el flujo de trabajo de GitHub para pull requests:

1. Haz un fork del proyecto.
2. Crea una nueva rama con tus cambios: `git checkout -b mi-rama`.
3. Realiza tus modificaciones y commits.
4. Envía tus cambios al repositorio remoto: `git push origin mi-rama`.
5. Crea un pull request en este repositorio.

## Licencia

Este proyecto está licenciado bajo la Licencia MIT. Para más detalles, consulta el archivo LICENSE en la raíz del repositorio.

## Recursos Adicionales

- **Documentación de .NET**: Para más detalles sobre cómo trabajar con .NET, visita la [documentación oficial de .NET](https://docs.microsoft.com/dotnet/).
- **ASP.NET Core Identity**: Visita la [documentación oficial de Identity](https://docs.microsoft.com/aspnet/core/security/authentication/identity) para aprender más sobre la autenticación y autorización.
- **Entity Framework Core**: Aprende cómo manejar las bases de datos accediendo a la [documentación de Entity Framework Core](https://docs.microsoft.com/ef/core/).

## Contacto

Para consultas referentes al proyecto o colaboración, por favor contacta a **Michael Leppe** mediante el correo electrónico **michael.leppe@gmail.com**.

Este README busca servir como un recurso completo para ayudar a los desarrolladores a configurar y contribuir al proyecto SISAE, asegurando un entorno limpio y seguro para el desarrollo continuo. Si encuentras algún error o tienes sugerencias para mejorar este documento, no dudes en contactarnos o enviar un pull request.
Este README ahora incluye detalles exhaustivos sobre la configuración, servicios adicionales, migraciones de base de datos, y uso de Identity en el sistema. También brinda recursos y contacto para soporte adicional.