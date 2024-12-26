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

## [1.0.1] - 2024-12-26

### Modificado
- Mejora de la página de perfil con diseño responsivo, tooltips y mensajes de retroalimentación para una mejor experiencia de usuario.
- Validación en tiempo real para el número de teléfono en el formulario de perfil.
- Mensajes de confirmación al guardar cambios en el perfil.
- Ajustes en el diseño de la página de creación de visitas para una mejor visualización de los datos.
- Ajustes en el diseño de la página de creación de visitados para una mejor visualización de los datos.
- Ajustes en el diseño de la página de creación de accesos prohibidos para una mejor visualización de los datos.
- Ajustes en el diseño de la página de edición de visitas para una mejor visualización de los datos.
- Ajustes en el diseño de la página de edición de visitados para una mejor visualización de los datos.
- Ajustes en el diseño de la página de edición de accesos prohibidos para una mejor visualización de los datos.
- Mejora del campo de respuesta en la página de ayuda para incluir un filtro de búsqueda.
- Las respuestas ahora se muestran en una tabla, mejorando la legibilidad y organización de la información.
- Se implementó un script para filtrar las respuestas en tiempo real, similar a la funcionalidad de la página de informes.

### Mejorado
- Mejora de la página de perfil con diseño responsivo, tooltips y mensajes de retroalimentación para una mejor experiencia de usuario.
- Validación en tiempo real para el número de teléfono en el formulario de perfil.
- Mensajes de confirmación al guardar cambios en el perfil.

### Corregido
- Solución al error 404 en la página de ayuda al enviar consultas.
- Implementación de la devolución de respuestas en el método `OnPostAsync` para asegurar que las respuestas se muestren correctamente al usuario.
