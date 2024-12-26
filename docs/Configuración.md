# Configuración

Este documento detalla cómo configurar el entorno de desarrollo para el proyecto.

## Requisitos
- .NET SDK
- Node.js y npm
- SQL Server para la base de datos

## Archivos de Configuración
- **appsettings.json**: 
  - **ConnectionStrings**: Define la cadena de conexión a la base de datos Azure SQL.
  - **Logging**: Configura los niveles de registro para la aplicación.
  - **AllowedHosts**: Especifica los hosts permitidos para la aplicación.
- **secrets.json**: Contiene configuraciones sensibles que no deben ser compartidas públicamente. 