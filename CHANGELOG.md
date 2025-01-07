# Registro de Cambios

## [1.0.0] - 2024-12-26

### Añadido
- Implementación de `localStorage` para persistir datos del gráfico y tablas en el dashboard.
- Funcionalidad de clic en nodos del gráfico para mostrar información detallada.
- Botón para limpiar el dashboard, que borra el gráfico, las tablas y el almacenamiento local.
- Funcionalidad para exportar las tablas de Visitas y Prohibidos a CSV.
- Hacer dropdown de visitante solo lectura y actualizarlo después de verificar el RUT en la página de creación de visitas.
- Lógica de reintento en la conexión a la base de datos usando `EnableRetryOnFailure`.
- Funcionalidad de exportación a CSV para las tablas de Visitas, Accesos Prohibidos y Visitados.
- Paginación en las tablas del Dashboard asi como también en las páginas de Accesos Prohibidos, Visitas y Visitados usando DataTables.

### Modificado
- Ajustes en el diseño del dashboard para maximizar el uso del espacio horizontal y centrar el contenido.
- Estilo del botón de limpieza para mejorar su visibilidad.
- Cambio del botón de limpieza a texto cliqueable para mejorar la visibilidad y funcionalidad.
- Almacenamiento del ID del visitante en una variable y actualización del dropdown para mostrar el visitante seleccionado como referencia.
- Configuración de DataTables para mostrar 20 registros por página y ordenar por fecha y hora de entrada de forma descendente en la página de Visitas.
- Mejora de la UI/UX en las páginas de índice de Visitas, Accesos Prohibidos y Visitados usando Bootstrap para un diseño más limpio y responsivo.

### Eliminado
- Restricciones de ancho máximo en el contenedor del dashboard.

## [1.0.1] - 2024-12-28

### Añadido
- Implementación de `SmtpEmailSender` para el envío de correos electrónicos utilizando SMTP.
- Configuración de credenciales de correo electrónico en `secrets.json` para mejorar la seguridad.
- Implementación de la devolución de respuestas en el método `OnPostAsync` para asegurar que las respuestas se muestren correctamente al usuario.
- Implementación de la funcionalidad de envío de correos electrónicos para notificar a los administradores sobre las visitas registradas.
- Implementación de MailerSend para el envío de correos electrónicos utilizando TLS.

### Modificado
- Automatización de la verificación del RUT y selección del visitante en el formulario de registro de visitas al regresar desde el formulario de creación de visitantes.
- Mejora en el flujo de redirección para asegurar que el RUT se prellene y se verifique automáticamente.
- Actualización de la política de privacidad.
- Reorganización del contenido en la vista `Create.cshtml` para mostrar las imágenes y descripciones en dos columnas, cada una con dos filas, mejorando la presentación visual.
- Ajuste del script SQL para asegurar la compatibilidad con un servidor SQL Server local, eliminando opciones específicas de Azure y configurando las opciones de la base de datos para un entorno local.
- Actualización de `Program.cs` para utilizar las credenciales de correo electrónico desde `secrets.json`.
- Ajuste de la lógica de envío de correos electrónicos para asegurar que los correos se envíen correctamente.
- Actualización de la configuración de correo electrónico para utilizar MailerSend con seguridad de conexión TLS.

### Mejorado
- Mejora de la página de perfil con diseño responsivo, tooltips y mensajes de retroalimentación para una mejor experiencia de usuario.
- Validación en tiempo real para el número de teléfono en el formulario de perfil.
- Mensajes de confirmación al guardar cambios en el perfil.

### Corregido
- Solución al error 404 en la página de ayuda al enviar consultas.
- Implementación de la devolución de respuestas en el método `OnPostAsync` para asegurar que las respuestas se muestren correctamente al usuario.

## [1.0.2] - 2025-01-04

### Modificado
- Actualización del diseño de las vistas `ChangePassword.cshtml`, `TwoFactorAuthentication.cshtml`, `PersonalData.cshtml`, `DeletePersonalData.cshtml`, `EnableAuthenticator.cshtml`, `ResetAuthenticator.cshtml`, `Index.cshtml`, `Edit.cshtml`, `Create.cshtml`, `Informes.cshtml`, `Help.cshtml`, `AccesosProhibidos/Index.cshtml`, `AccesosProhibidos/Edit.cshtml`, `AccesosProhibidos/Delete.cshtml`, `AccesosProhibidos/Create.cshtml`, `Error.cshtml`, `Privacy.cshtml`, y `Dashboard.cshtml` para centrar el contenido y mejorar la consistencia visual.
- Ocultación del enlace "Inicios de Sesión Externos" en la vista `Index.cshtml`.
- Mejora en la disposición de los elementos en `Help.cshtml` para una mejor organización y usabilidad.
- Actualización de la lógica en `Help.cshtml.cs` para asegurar la consistencia con las prácticas actuales y mejorar la robustez en el manejo de consultas y respuestas.

### Añadido
- Botón "NUEVO" en `Informes.cshtml` para limpiar los campos de entrada y enfocar el campo de consulta.
- Botón "NUEVO" en `Help.cshtml` para limpiar los campos de entrada y enfocar el campo de consulta.
- Botón "NUEVO" en `Visitados/Create.cshtml` para limpiar los campos de entrada y enfocar el campo de consulta.
- Botón "NUEVO" en `Visitados/Edit.cshtml` para limpiar los campos de entrada y enfocar el campo de consulta.
- Botón "NUEVO" en `Visitados/Delete.cshtml` para limpiar los campos de entrada y enfocar el campo de consulta.
- Botón "NUEVO" en `Visitados/Index.cshtml` para limpiar los campos de entrada y enfocar el campo de consulta.

### Mejorado
- Mejora de la página de perfil con diseño responsivo, tooltips y mensajes de retroalimentación para una mejor experiencia de usuario.
- Validación en tiempo real para el número de teléfono en el formulario de perfil.
- Mensajes de confirmación al guardar cambios en el perfil.

### Corregido
- Se implementó y restauró la funcionalidad de gestión del perfil de usuarios en una aplicación ASP.NET Core con Identity, garantizando el correcto guardado del número de teléfono mediante el uso del método SetPhoneNumberAsync. Además, se preservó el diseño original, incluyendo el menú de navegación lateral y los estilos del formulario, mientras se incorporaron validaciones robustas del lado del servidor con mensajes dinámicos de éxito y error. Finalmente, se sincronizaron los cambios en tiempo real con RefreshSignInAsync, logrando una experiencia de usuario intuitiva y funcional.

## [1.0.3] - 2024-12-30

### Añadido
- Inclusión de un botón "NUEVO" en el formulario de `Informes.cshtml` para limpiar los campos de entrada.

### Modificado
- Ajustes en la función `formatearFechaYYYYMMDD` para manejar correctamente el formato de las fechas.
- Verificación y corrección de la asignación de fechas de nacimiento y vencimiento en el formulario de creación de visitantes.
- Mejora en la disposición de elementos en el formulario de `Create.cshtml` utilizando el sistema de grillas de Bootstrap.
- Actualización de los nombres de columnas en la consulta predefinida para listar los últimos 1000 eventos de `EventLog`.