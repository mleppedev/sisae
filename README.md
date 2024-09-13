SISAE

Este repositorio contiene el código fuente del proyecto Nombre del Proyecto. Por motivos de seguridad y eficiencia, ciertos archivos y carpetas no están incluidos en el repositorio. A continuación, se detalla qué elementos se han excluido y cómo configurarlos en tu entorno local.

Archivos y Carpetas Excluidos
Ignorar archivos de configuración con credenciales
secrets.json
appsettings.json

Estos archivos contienen información sensible, como claves API, contraseñas y cadenas de conexión a bases de datos. Para proteger estas credenciales y evitar comprometer la seguridad, no se incluyen en el repositorio.

Acción requerida: Cada desarrollador debe crear sus propios archivos de configuración basados en los ejemplos proporcionados o según las necesidades de su entorno local.

Ignorar carpetas de compilación
bin/
obj/

Estas carpetas son generadas automáticamente durante el proceso de compilación y contienen archivos binarios y objetos intermedios. No es necesario incluirlas en el control de versiones, ya que pueden regenerarse en cada entorno de desarrollo.

Ignorar otros archivos temporales o de configuración
*.user
*.suo
*.log
*.tmp

Estos archivos son específicos del entorno local y pueden variar entre diferentes máquinas. Se excluyen para mantener el repositorio limpio y evitar conflictos innecesarios.

Configuración del Entorno Local
Para configurar el proyecto en tu entorno local, sigue estos pasos:

Clona el repositorio:

bash
git clone https://github.com/tu-usuario/tu-proyecto.git

Crea los archivos de configuración necesarios:

appsettings.json

Crea un archivo appsettings.json en la raíz del proyecto con el siguiente contenido de ejemplo:

json
{
  "ConnectionStrings": {
    "DefaultConnection": "TuCadenaDeConexion"
  },
  "AppSettings": {
    "ClaveAPI": "TuClaveAPI"
  }
}

Reemplaza "TuCadenaDeConexion" y "TuClaveAPI" con tus valores correspondientes.

secrets.json

Si tu aplicación utiliza secretos adicionales, crea un archivo secrets.json siguiendo el formato requerido por el proyecto.

Restaura los paquetes NuGet:

bash
dotnet restore
Compila el proyecto:

bash
dotnet build
Ejecuta la aplicación:

bash
dotnet run

Buenas Prácticas
No compartir información sensible: Nunca incluyas credenciales o información sensible en los commits o en el código fuente compartido.
Mantener actualizado el .gitignore: Si agregas nuevos archivos o carpetas que no deban ser versionados, actualiza el archivo .gitignore en consecuencia.
Uso de variables de entorno: Considera utilizar variables de entorno para gestionar información sensible en lugar de archivos de configuración.