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

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Visitado> Visitados { get; set; }

        public async Task OnGetAsync()
        {
            Visitados = await _context.Visitados.ToListAsync();
        }
    }
}