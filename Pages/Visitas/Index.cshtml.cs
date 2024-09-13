using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using sisae.Data;
using sisae.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sisae.Pages.Visitas
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Visita> Visitas { get; set; }

        public async Task OnGetAsync()
        {
            // Incluir las relaciones con Visitante y Visitado
            Visitas = await _context.Visitas
                .Include(v => v.Visitante)  // Incluir datos del visitante
                .Include(v => v.Visitado)    // Incluir datos del visitado
                .ToListAsync();
        }
    }
}