using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sisae.Data;
using sisae.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sisae.Pages.Visitas
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Visita Visita { get; set; }

        public List<Visitado> Visitados { get; set; }
        public List<SelectListItem> VisitantesSelectList { get; set; }

        // Obtener la visita existente y las listas de visitantes/visitados
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Buscar la visita por su ID
            Visita = await _context.Visitas.FindAsync(id);

            if (Visita == null)
            {
                return NotFound();
            }

            // Poblar los SelectLists para Visitantes y Visitados
            Visitados = await _context.Visitados.ToListAsync();
            VisitantesSelectList = await _context.Visitantes.Select(v => new SelectListItem
            {
                Value = v.ID_Visitante.ToString(),
                Text = $"{v.Apellido}, {v.Nombre} ({v.RUT})"
            }).ToListAsync();

            return Page();
        }

        // Actualizar la visita editada
        public async Task<IActionResult> OnPostAsync()
        {
            // Validar solo los campos requeridos
            ModelState.Remove("Visita.Visitado");
            ModelState.Remove("Visita.Visitante");

            if (!ModelState.IsValid)
            {
                // Volver a cargar los datos en caso de error de validación
                Visitados = await _context.Visitados.ToListAsync();
                VisitantesSelectList = await _context.Visitantes.Select(v => new SelectListItem
                {
                    Value = v.ID_Visitante.ToString(),
                    Text = $"{v.Apellido}, {v.Nombre} ({v.RUT})"
                }).ToListAsync();
                return Page();
            }

            // Adjuntar la visita y establecer su estado como modificado
            _context.Attach(Visita).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VisitaExists(Visita.ID_Visita))
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

        // Verificar si la visita existe
        private bool VisitaExists(int id)
        {
            return _context.Visitas.Any(e => e.ID_Visita == id);
        }
    }
}