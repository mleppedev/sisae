using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using sisae.Data;
using sisae.Models;
using System.Threading.Tasks;

namespace sisae.Pages.Visitados
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly EventLoggerService _eventLoggerService;

        public DeleteModel(ApplicationDbContext context, EventLoggerService eventLoggerService)
        {
            _context = context;
            _eventLoggerService = eventLoggerService;
        }

        [BindProperty]
        public Visitado Visitado { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Visitado = await _context.Visitados.FindAsync(id);

            if (Visitado == null)
            {
                // Registrar el intento de borrar un Visitado no encontrado
                await _eventLoggerService.LogEventAsync("EliminarVisitadoNoEncontrado", $"Intento de acceso para eliminar un visitado con ID {id} no encontrado", User?.Identity?.Name);
                return NotFound();
            }

            // Registrar el acceso a la página de eliminación para el visitado encontrado
            await _eventLoggerService.LogEventAsync("AccesoEliminarVisitado", $"Acceso a eliminar el visitado con ID {id}", User?.Identity?.Name);
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Visitado = await _context.Visitados.FindAsync(id);

            if (Visitado != null)
            {
                _context.Visitados.Remove(Visitado);
                await _context.SaveChangesAsync();

                // Registrar el evento después de la eliminación exitosa
                await _eventLoggerService.LogEventAsync("EliminarVisitado", $"Visitado con ID {id} eliminado exitosamente", User?.Identity?.Name);
            }
            else
            {
                // Registrar si el visitado a eliminar no se encontró durante el POST
                await _eventLoggerService.LogEventAsync("EliminarVisitadoNoEncontradoPost", $"Intento de eliminar un visitado con ID {id} no encontrado durante el POST", User?.Identity?.Name);
            }

            return RedirectToPage("./Index");
        }
    }
}
