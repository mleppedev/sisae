Documentación de la API

Este documento proporciona detalles sobre los endpoints de la API disponibles para la gestión del sistema Sisae. Todos los endpoints requieren autenticación.

Autenticación
La autenticación se realiza mediante tokens JWT. Incluye el token en el encabezado de autorización para todas las solicitudes:

Authorization: Bearer
Endpoints Disponibles

1. VISITANTES
Obtener todos los visitantes

Método: `GET`
Endpoint: `/api/visitantes`
Descripción: Recibe una lista de todos los visitantes registrados.
Parámetros de Solicitud: Ninguno
Códigos de Respuesta: 200 OK: Lista de visitantes devuelta exitosamente.
Ejemplo de Respuesta:
  [
    {
      "id_visitante": 1,
      "nombre": "Juan",
      "apellido": "Pérez",
      "email": "juan.perez@example.com"
    }
  ]

Crear un nuevo visitante
Método: POST
Endpoint: /api/visitantes
Descripción: Crea un nuevo visitante en el sistema.
Cuerpo de la Solicitud:
{
  "nombre": "Juan",
  "apellido": "Pérez",
  "email": "juan.perez@example.com",
  "direccion": "Calle Falsa 123",
  "rut": "12.345.678-9",
  "nacionalidad": "Chile",
  "telefono": "+56912345678"
}

Códigos de Respuesta:
201 Created: Visitante creado exitosamente.
400 Bad Request: Datos inválidos en la solicitud.

Ejemplo de Respuesta Exitosa:
{
  "id_visitante": 2,
  "nombre": "Juan",
  "apellido": "Pérez"
}

2. VISITAS
Registrar una nueva visita
Método: POST
Endpoint: /api/visitas
Descripción: Registra una nueva visita.
Cuerpo de la Solicitud:
{
  "id_visitante": 1,
  "id_visitado": 2,
  "fecha_visita": "2023-10-15",
  "hora_entrada": "09:00:00",
  "motivo_visita": "Visita de negocio"
}

Códigos de Respuesta:
201 Created: Visita registrada exitosamente.
400 Bad Request: Datos de solicitud inválidos.

Ejemplo de Respuesta Exitosa:
{
  "id_visita": 45,
  "estado": "Activa"
}

3. ACCESOS PROHIBIDOS
Listar accesos prohibidos
Método: GET
Endpoint: /api/accesosprohibidos
Descripción: Obtiene una lista de accesos prohibidos activos.
Códigos de Respuesta: 200 OK: Datos devueltos exitosamente.

Ejemplo de Respuesta:
[
  {
    "id_acceso_prohibido": 10,
    "id_visitante": 1,
    "fecha_prohibicion": "2023-05-01",
    "motivo": "Incidente de seguridad"
  }
]

ERRORES COMUNES
401 Unauthorized: El token de autenticación es inválido o está ausente.
404 Not Found: El recurso solicitado no existe.
500 Internal Server Error: Ocurrió un error inesperado en el servidor.