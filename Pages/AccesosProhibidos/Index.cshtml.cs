using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using sisae.Data;
using sisae.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sisae.Pages.AccesosProhibidos
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<AccesoProhibido> AccesosProhibidos { get; set; }

        public async Task OnGetAsync()
        {
            AccesosProhibidos = await _context.AccesosProhibidos
                .Include(a => a.Visitante)
                .ToListAsync();
        }
    }
}