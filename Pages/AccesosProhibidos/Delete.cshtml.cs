using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using sisae.Data;
using sisae.Models;
using System.Threading.Tasks;

namespace sisae.Pages.AccesosProhibidos
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AccesoProhibido AccesoProhibido { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            AccesoProhibido = await _context.AccesosProhibidos
                .Include(a => a.Visitante)
                .FirstOrDefaultAsync(m => m.ID_Acceso_Prohibido == id);

            if (AccesoProhibido == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            AccesoProhibido = await _context.AccesosProhibidos.FindAsync(id);

            if (AccesoProhibido != null)
            {
                _context.AccesosProhibidos.Remove(AccesoProhibido);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}