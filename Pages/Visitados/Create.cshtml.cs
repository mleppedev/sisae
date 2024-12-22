using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using sisae.Data;
using sisae.Models;
using System.Threading.Tasks;

namespace sisae.Pages.Visitados
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly EventLoggerService _eventLoggerService;

        public CreateModel(ApplicationDbContext context, EventLoggerService eventLoggerService)
        {
            _context = context;
            _eventLoggerService = eventLoggerService;
        }

        [BindProperty]
        public Visitado Visitado { get; set; }

        public IActionResult OnGet()
        {
            // Registrar el acceso a la página de creación
            _eventLoggerService.LogEventAsync("AccesoCrearVisitado", "Acceso a la página de creación de visitados", User?.Identity?.Name);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Registrar el error de validación
                await _eventLoggerService.LogEventAsync("ErrorValidacionCrearVisitado", "Error de validación al crear un nuevo visitado", User?.Identity?.Name);
                return Page();
            }

            _context.Visitados.Add(Visitado);
            await _context.SaveChangesAsync();

            // Registrar el evento después de guardar el nuevo visitado
            await _eventLoggerService.LogEventAsync("CrearVisitado", $"Nuevo visitado creado con ID {Visitado.ID_Visitado}", User?.Identity?.Name);

            return RedirectToPage("./Index");
        }
    }
}
