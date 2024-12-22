using sisae.Data;
using sisae.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

public class EventLoggerService
{
    private readonly IServiceProvider _serviceProvider;

    // Constructor que recibe IServiceProvider para crear instancias de ApplicationDbContext
    public EventLoggerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    // Método para registrar eventos de forma asíncrona
    public async Task LogEventAsync(string eventType, string description, string userId)
    {
        // Crear un nuevo alcance para obtener una nueva instancia de ApplicationDbContext
        using (var scope = _serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var log = new EventLog
            {
                EventType = eventType,
                Description = description,
                UserId = userId
            };

            context.EventLogs.Add(log);
            await context.SaveChangesAsync();
        }
    }
}
