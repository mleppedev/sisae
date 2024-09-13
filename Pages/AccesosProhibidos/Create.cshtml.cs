using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sisae.Models;
using System.Linq;
using System.Threading.Tasks;
using sisae.Data;

namespace sisae.Pages.AccesosProhibidos
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AccesoProhibido AccesoProhibido { get; set; }

        public List<SelectListItem> VisitantesSelectList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Generar lista de Visitantes
            VisitantesSelectList = await _context.Visitantes.Select(v => new SelectListItem
            {
                Value = v.ID_Visitante.ToString(),
                Text = $"{v.Apellido}, {v.Nombre} ({v.RUT})"
            }).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Ignorar la validación para la propiedad Visitante
            ModelState.Remove("AccesoProhibido.Visitante");

            if (!ModelState.IsValid)
            {
                VisitantesSelectList = await _context.Visitantes.Select(v => new SelectListItem
                {
                    Value = v.ID_Visitante.ToString(),
                    Text = $"{v.Apellido}, {v.Nombre} ({v.RUT})"
                }).ToListAsync();

                return Page();
            }

            // Asegúrate de que el ID del visitante esté asignado correctamente
            var visitante = await _context.Visitantes.FindAsync(AccesoProhibido.ID_Visitante);
            if (visitante == null)
            {
                ModelState.AddModelError(string.Empty, "El visitante seleccionado no existe.");
                return Page();
            }

            _context.AccesosProhibidos.Add(AccesoProhibido);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}