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

## [1.0.2] - 2024-12-28

### Modificado
- Traducción de las páginas de confirmación de registro y restablecimiento de contraseña al español para mejorar la experiencia del usuario hispanohablante.