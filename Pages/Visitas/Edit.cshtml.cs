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
        private readonly EventLoggerService _eventLoggerService;

        public EditModel(ApplicationDbContext context, EventLoggerService eventLoggerService)
        {
            _context = context;
            _eventLoggerService = eventLoggerService;
        }

        [BindProperty]
        public Visita Visita { get; set; }

        public List<Visitado> Visitados { get; set; }
        public List<SelectListItem> VisitantesSelectList { get; set; }
        public List<SelectListItem> VisitadosSelectList { get; set; }

        // Obtener la visita existente y las listas de visitantes/visitados
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                await _eventLoggerService.LogEventAsync("ErrorAcceso", "Intento de acceso al editor de visitas sin especificar ID", User?.Identity?.Name);
                return NotFound();
            }

            // Buscar la visita por su ID
            Visita = await _context.Visitas.FindAsync(id);
            if (Visita == null)
            {
                await _eventLoggerService.LogEventAsync("VisitaNoEncontrada", $"No se encontró la visita con ID {id}", User?.Identity?.Name);
                return NotFound();
            }

            // Poblar los SelectLists para Visitantes y Visitados
            Visitados = await _context.Visitados.ToListAsync();
            VisitantesSelectList = await _context.Visitantes.Select(v => new SelectListItem
            {
                Value = v.ID_Visitante.ToString(),
                Text = $"{v.Apellido}, {v.Nombre} ({v.RUT})"
            }).ToListAsync();

            VisitadosSelectList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Seleccione..." } // Opción predeterminada
            };
            VisitadosSelectList.AddRange(await _context.Visitados
                .OrderBy(v => v.Apellido)
                .ThenBy(v => v.Nombre)
                .Select(v => new SelectListItem
                {
                    Value = v.ID_Visitado.ToString(),
                    Text = $"{v.Apellido}, {v.Nombre} ({v.Cargo})"
                }).ToListAsync());

            await _eventLoggerService.LogEventAsync("AccesoEditorVisita", $"Acceso al editor de visitas para la visita con ID {id}", User?.Identity?.Name);
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

                await _eventLoggerService.LogEventAsync("ErrorValidacionEditar", "Error de validación al editar la visita", User?.Identity?.Name);
                return Page();
            }

            // Adjuntar la visita y establecer su estado como modificado
            _context.Attach(Visita).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                await _eventLoggerService.LogEventAsync("VisitaEditada", $"Visita con ID {Visita.ID_Visita} editada exitosamente", User?.Identity?.Name);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!VisitaExists(Visita.ID_Visita))
                {
                    await _eventLoggerService.LogEventAsync("ErrorEdicion", $"Fallo al editar: la visita con ID {Visita.ID_Visita} no existe. Error: {ex.Message}", User?.Identity?.Name);
                    return NotFound();
                }
                else
                {
                    await _eventLoggerService.LogEventAsync("ErrorEdicion", $"Excepción concurrente al editar la visita con ID {Visita.ID_Visita}. Error: {ex.Message}", User?.Identity?.Name);
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
