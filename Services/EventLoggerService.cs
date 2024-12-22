using sisae.Data;
using sisae.Models;
using System.Threading.Tasks;

public class EventLoggerService
{
    private readonly ApplicationDbContext _context;

    public EventLoggerService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task LogEventAsync(string eventType, string description, string userId)
    {
        var log = new EventLog
        {
            EventType = eventType,
            Description = description,
            UserId = userId
        };

        _context.EventLogs.Add(log);
        await _context.SaveChangesAsync();
    }

}
