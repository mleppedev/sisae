using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using sisae.Data;
using sisae.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sisae.Pages.Visitados
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly EventLoggerService _eventLoggerService;

        public IndexModel(ApplicationDbContext context, EventLoggerService eventLoggerService)
        {
            _context = context;
            _eventLoggerService = eventLoggerService;
        }

        public IList<Visitado> Visitados { get; set; }

        public async Task OnGetAsync()
        {
            // Cargar todos los registros de Visitados
            Visitados = await _context.Visitados.ToListAsync();

            // Registrar el acceso a la página de índice de Visitados
            await _eventLoggerService.LogEventAsync("AccesoIndexVisitados", "Acceso a la lista de visitados", User?.Identity?.Name);
        }
    }
}
