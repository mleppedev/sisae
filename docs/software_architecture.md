# Descripción General de la Arquitectura de Software

## Propósito y Alcance
Este documento describe la arquitectura del software Sisae, un sistema diseñado para gestionar información de visitantes y control de accesos de manera eficiente. El alcance incluye los componentes principales, sus roles y sus interacciones dentro de la aplicación.

## Diagrama de Arquitectura
(Incluir un diagrama que visualmente describa la arquitectura. Esto podría ser un diagrama UML, un diagrama de componentes, o un diagrama de bloques simple que muestre los componentes de alto nivel y sus interacciones.)

## Descripción de Componentes

### Modelos
- **Propósito**: Definir las estructuras de datos y realizar la validación de los mismos.
- **Tecnologías**: Utiliza C# y Entity Framework Core para definición y manipulación de entidades de datos.
- **Modelos Clave**:
  - `Visitante`: Representa la información del visitante, incluyendo detalles de contacto e identificación.
  - `Visitado`: Representa a las personas que son visitadas, usualmente empleados.
  - `Visita`: Captura los detalles de cada visita, incluyendo horarios y comentarios.
  - `AccesoProhibido`: Registra instancias donde el acceso de un visitante es restringido.
- Cada modelo utiliza anotaciones de datos para implementar validación y restricciones de base de datos.

### Páginas
- **Propósito**: Proveer la interfaz de usuario mediante Razor Pages para interactuar con el sistema.
- **Tecnologías**: ASP.NET Core Razor Pages, HTML, CSS, JavaScript.
- **Secciones**:
  - `Visitantes`: Todas las Razor Pages relacionadas con la gestión de visitantes, tales como creación y visualización de visitantes.
  - `Visitas`: Páginas que manejan la programación y seguimiento de visitas.
  - `AccesosProhibidos`: Gestionar las características de control de acceso para visitantes con entrada restringida.
- **Característica Clave**: Uso de `asp-for` y `asp-items` dentro de Razor Pages para enlazar elementos UI directamente a modelos de datos.

### Datos
- **Propósito**: Manejar las interacciones con la base de datos, proporcionar una capa de abstracción para el acceso a datos.
- **Componentes**:
  - `ApplicationDbContext`: Administra las conexiones a la base de datos y las operaciones específicas del contexto usando Entity Framework Core.
  - Migraciones: Gestiona las actualizaciones del esquema de la base de datos y el control de versiones.
- **Base de Datos**: SQL Server o cualquier otro sistema de base de datos compatible con Entity Framework Core.

### Controladores (si aplica)
- **Propósito**: Gestionar el enrutamiento de solicitudes e implementar la lógica de negocio (aplicable si se utiliza un patrón MVC).
- **Operaciones**: Recibir solicitudes HTTP, interactuar con el modelo/capa de datos y devolver respuestas HTTP.

### Servicios
- **Propósito**: Manejo intermedio de lógica de negocio y necesidades de comunicación/procesamiento.
- **Tecnologías**: API de Servicios Web, integraciones de terceros.
- **Ejemplos**:
  - GoogleCloudVisionService: Maneja integraciones para procesamiento de imágenes (por ejemplo, extracción de información de cédulas de identidad).

### Seguridad y Autenticación
- **Propósito**: Asegurar el control de acceso seguro y la autenticación de usuarios.
- **Enfoque**: Usar ASP.NET Core Identity y otros middleware para implementar autenticación y autorización.
- **Técnicas**: Uso de cookies HTTP seguras, HTTPS para la encriptación de datos en tránsito, y mecanismos de autenticación de dos factores.

### Middlewares
- **Propósito**: Proveer pipelines de manejo de solicitudes y manejar preocupaciones transversales como autenticación, logging, etc.
- **Ejemplos Comunes**: Middleware de autenticación, middleware de manejo de excepciones, middleware de logging.

### Configuración
- **Archivos**:
  - `appsettings.json`: Almacena datos de configuración como cadenas de conexión y configuraciones de la aplicación.
  - `launchSettings.json`: Define las variables de entorno y configuraciones de lanzamiento para el desarrollo local.
- **Mejoras**: Utilizar `UserSecrets` para manejar configuraciones sensibles durante el desarrollo.

### Tecnologías Utilizadas
- **Frontend**: HTML, CSS, Bootstrap para diseño responsivo, JavaScript.
- **Backend**: ASP.NET Core, C#, Entity Framework Core.
- **Integraciones**: Servicios de Google Cloud Vision.

### Prácticas Recomendadas
- **Código Limpio**:
