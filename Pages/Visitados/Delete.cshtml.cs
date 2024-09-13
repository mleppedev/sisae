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

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Visitado Visitado { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Visitado = await _context.Visitados.FindAsync(id);

            if (Visitado == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Visitado = await _context.Visitados.FindAsync(id);

            if (Visitado != null)
            {
                _context.Visitados.Remove(Visitado);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}