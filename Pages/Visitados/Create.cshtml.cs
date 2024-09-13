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

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Visitado Visitado { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Visitados.Add(Visitado);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}