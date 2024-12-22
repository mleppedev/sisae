using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using sisae.Data;
using sisae.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace sisae.Pages.Visitados
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly EventLoggerService _eventLoggerService;

        public EditModel(ApplicationDbContext context, EventLoggerService eventLoggerService)
        {
            _context = context;
            _eventLoggerService = eventLoggerService;
        }

        [BindProperty]
        public Visitado Visitado { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Obtener el registro de Visitado a editar
            Visitado = await _context.Visitados.AsNoTracking().FirstOrDefaultAsync(v => v.ID_Visitado == id);

            if (Visitado == null)
            {
                // Registrar el intento de editar un Visitado no encontrado
                await _eventLoggerService.LogEventAsync("EditarVisitadoNoEncontrado", $"Intento de editar un visitado con ID {id} no encontrado", User?.Identity?.Name);
                return NotFound();
            }

            // Registrar el acceso a la página de edición del visitado
            await _eventLoggerService.LogEventAsync("AccesoEditarVisitado", $"Acceso a edición del visitado con ID {id}", User?.Identity?.Name);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                // Registrar el error de validación
                await _eventLoggerService.LogEventAsync("ErrorValidacionEditarVisitado", "Error de validación al editar un visitado", User?.Identity?.Name);
                return Page();
            }

            // Intentar obtener el Visitado de la base de datos
            var visitadoToUpdate = await _context.Visitados.FindAsync(Visitado.ID_Visitado);

            if (visitadoToUpdate == null)
            {
                // Registrar si el visitado a editar no se encontró durante el POST
                await _eventLoggerService.LogEventAsync("EditarVisitadoNoEncontradoPost", $"Intento de editar un visitado con ID {Visitado.ID_Visitado} no encontrado durante el POST", User?.Identity?.Name);
                return NotFound();
            }

            // Actualizar los valores
            if (await TryUpdateModelAsync(
                visitadoToUpdate,
                "Visitado",
                v => v.Nombre, v => v.Apellido, v => v.Cargo, v => v.Departamento, v => v.Telefono, v => v.Email))
            {
                try
                {
                    await _context.SaveChangesAsync();

                    // Registrar el evento después de la edición exitosa
                    await _eventLoggerService.LogEventAsync("EditarVisitado", $"Visitado con ID {Visitado.ID_Visitado} editado exitosamente", User?.Identity?.Name);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitadoExists(Visitado.ID_Visitado))
                    {
                        await _eventLoggerService.LogEventAsync("ErrorEdicionVisitadoInexistente", $"El visitado con ID {Visitado.ID_Visitado} no existe en la base de datos al intentar editar", User?.Identity?.Name);
                        return NotFound();
                    }
                    else
                    {
                        await _eventLoggerService.LogEventAsync("ErrorConcurrencia", $"Excepción concurrente al editar el visitado con ID {Visitado.ID_Visitado}", User?.Identity?.Name);
                        throw;
                    }
                }

                return RedirectToPage("./Index");
            }

            return Page();
        }

        private bool VisitadoExists(int id)
        {
            return _context.Visitados.Any(e => e.ID_Visitado == id);
        }
    }
}
