using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sisae.Data;
using sisae.Models;
using System.Linq;
using System.Threading.Tasks;

namespace sisae.Pages.AccesosProhibidos
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AccesoProhibido AccesoProhibido { get; set; }

        public SelectList VisitantesSelectList { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Buscar el AccesoProhibido por su ID e incluir el Visitante relacionado
            AccesoProhibido = await _context.AccesosProhibidos
                .Include(a => a.Visitante)  // Incluimos el Visitante para evitar problemas de navegación
                .FirstOrDefaultAsync(m => m.ID_Acceso_Prohibido == id);

            // Si no se encuentra el AccesoProhibido, devolvemos NotFound
            if (AccesoProhibido == null)
            {
                return NotFound();
            }

            // Cargar la lista de visitantes para el desplegable
            VisitantesSelectList = new SelectList(await _context.Visitantes.ToListAsync(), "ID_Visitante", "Nombre");

            // Devolver la página con los datos
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Ignoramos la validación para la propiedad de navegación
            ModelState.Remove("AccesoProhibido.Visitante");

            if (!ModelState.IsValid)
            {
                VisitantesSelectList = new SelectList(await _context.Visitantes.ToListAsync(), "ID_Visitante", "Nombre");
                return Page();
            }

            // Cargar el objeto existente de la base de datos
            var accesoProhibidoExistente = await _context.AccesosProhibidos.FindAsync(AccesoProhibido.ID_Acceso_Prohibido);

            if (accesoProhibidoExistente == null)
            {
                return NotFound();
            }

            // Actualizar los campos que han sido modificados
            accesoProhibidoExistente.Fecha_Prohibicion = AccesoProhibido.Fecha_Prohibicion;
            accesoProhibidoExistente.Fecha_Expiracion = AccesoProhibido.Fecha_Expiracion;
            accesoProhibidoExistente.ID_Visitante = AccesoProhibido.ID_Visitante;
            accesoProhibidoExistente.Motivo = AccesoProhibido.Motivo;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccesoProhibidoExists(AccesoProhibido.ID_Acceso_Prohibido))
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

        private bool AccesoProhibidoExists(int id)
        {
            return _context.AccesosProhibidos.Any(e => e.ID_Acceso_Prohibido == id);
        }
    }
}