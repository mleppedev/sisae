# Migraciones

Este documento detalla las migraciones de base de datos y cómo aplicarlas.

## Detalles de Migraciones
- Las migraciones se utilizan para actualizar el esquema de la base de datos de manera controlada.
- Incluyen cambios en las tablas `AccesosProhibidos`, `Visitados`, `Visitantes`, y `Visitas`.

## Aplicación de Migraciones
- Utilizar el comando `Update-Database` en la consola de administración de paquetes para aplicar las migraciones.
- Asegurarse de que la cadena de conexión esté correctamente configurada en `appsettings.json`. 