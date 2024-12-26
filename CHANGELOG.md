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