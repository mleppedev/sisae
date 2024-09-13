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

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Visitado Visitado { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Obtener el registro de Visitado a editar
            Visitado = await _context.Visitados.AsNoTracking().FirstOrDefaultAsync(v => v.ID_Visitado == id);

            if (Visitado == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validar el modelo
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Intentar obtener el Visitado de la base de datos
            var visitadoToUpdate = await _context.Visitados.FindAsync(Visitado.ID_Visitado);

            if (visitadoToUpdate == null)
            {
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
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitadoExists(Visitado.ID_Visitado))
                    {
                        return NotFound();
                    }
                    else
                    {
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