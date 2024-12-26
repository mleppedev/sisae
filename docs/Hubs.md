# Hubs

Este documento describe los hubs de SignalR utilizados en el proyecto y sus casos de uso.

## Descripción de Hubs
- **DashboardHub**: Proporciona actualizaciones en tiempo real sobre las entidades `Visita` y `AccesoProhibido`.

## Casos de Uso
- **SendVisitaUpdate**: Envía actualizaciones sobre visitas a los clientes conectados.
- **SendAccesoProhibidoUpdate**: Envía actualizaciones sobre accesos prohibidos a los clientes conectados.
- Utiliza `ApplicationDbContext` para acceder a los datos y `EventLoggerService` para registrar eventos.