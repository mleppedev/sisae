using Microsoft.AspNetCore.SignalR;
using sisae.Data;
using sisae.Models;
using System.Text.Json;
using sisae.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

public class DashboardHub : Hub
{
    private readonly ApplicationDbContext _context;
    private readonly EventLoggerService _eventLoggerService;

    public DashboardHub(ApplicationDbContext context, EventLoggerService eventLoggerService)
    {
        Console.WriteLine("Entrando al constructor DashboardHub...");
        _context = context;
        _eventLoggerService = eventLoggerService;
    }

    public async Task SendVisitaUpdate(VisitaDto visita)
    {
        if (visita == null) throw new ArgumentNullException(nameof(visita));

        var message = JsonSerializer.Serialize(new
        {
            fecha = visita.Fecha_Visita.ToString("dd-MM-yyyy HH:mm"),
            estado = visita.Estado,
            rut = visita.RUT,
            nombre = visita.Nombre,
            apellido = visita.Apellido
        });

        Console.WriteLine($"Mensaje serializado: {message}");

        // Intenta enviar el mensaje a los clientes
        await Clients.All.SendAsync("ReceiveVisita", message);

        // Loguea después de intentar enviar el mensaje
        await _eventLoggerService.LogEventAsync(
            "SendVisitaUpdate",
            $"Actualización de visita enviada con ID {visita.ID_Visita} y Estado: {visita.Estado}",
            Context.User?.Identity?.Name ?? "Sistema"
        );
    }

    public async Task SendAccesoProhibidoUpdate(VisitaProhibidosDto visitaProhibidosDto)
    {
        if (visitaProhibidosDto == null) throw new ArgumentNullException(nameof(visitaProhibidosDto));

        var message = JsonSerializer.Serialize(new
        {
            fecha = visitaProhibidosDto.Fecha.ToString("dd-MM-yyyy HH:mm"),
            rut = visitaProhibidosDto.RUT,
            nombre = visitaProhibidosDto.Nombre,
            apellido = visitaProhibidosDto.Apellido,
            motivo = visitaProhibidosDto.Motivo
        });

        Console.WriteLine($"Mensaje de acceso prohibido serializado: {message}");

        // Enviar mensaje a los clientes
        await Clients.All.SendAsync("ReceiveAccesoProhibido", message);

        // Registrar evento en el log
        await _eventLoggerService.LogEventAsync(
            "SendAccesoProhibidoUpdate",
            $"Intento de acceso prohibido enviado: RUT {visitaProhibidosDto.RUT}, Motivo: {visitaProhibidosDto.Motivo}",
            Context.User?.Identity?.Name ?? "Sistema"
        );
    }
}
