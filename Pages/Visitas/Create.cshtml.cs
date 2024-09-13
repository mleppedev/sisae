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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Visita Visita { get; set; }

        public List<SelectListItem> VisitadosSelectList { get; set; }
        public List<SelectListItem> VisitantesSelectList { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Generar lista de Visitantes
            VisitantesSelectList = await _context.Visitantes.Select(v => new SelectListItem
            {
                Value = v.ID_Visitante.ToString(),
                Text = $"{v.Apellido}, {v.Nombre} ({v.RUT})"
            }).ToListAsync();

            // Generar lista de Visitados con formato "Apellido, Nombre (Cargo)"
            VisitadosSelectList = await _context.Visitados.Select(v => new SelectListItem
            {
                Value = v.ID_Visitado.ToString(),
                Text = $"{v.Apellido}, {v.Nombre} ({v.Cargo})"
            }).ToListAsync();

            // Establecer la fecha y hora actual como valores predeterminados
            Visita = new Visita
            {
                Fecha_Visita = DateTime.Now.Date, // Fecha de hoy
                Hora_Entrada = DateTime.Now.TimeOfDay // Hora actual
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validar solamente los campos que nos interesan (no las propiedades de navegaci�n)
            ModelState.Remove("Visita.Visitado");
            ModelState.Remove("Visita.Visitante");

            if (!ModelState.IsValid)
            {
                // Volver a cargar las listas en caso de error de validaci�n
                VisitantesSelectList = await _context.Visitantes.Select(v => new SelectListItem
                {
                    Value = v.ID_Visitante.ToString(),
                    Text = $"{v.Apellido}, {v.Nombre} ({v.RUT})"
                }).ToListAsync();

                VisitadosSelectList = await _context.Visitados.Select(v => new SelectListItem
                {
                    Value = v.ID_Visitado.ToString(),
                    Text = $"{v.Apellido}, {v.Nombre} ({v.Cargo})"
                }).ToListAsync();

                return Page();
            }

            // Guardar la nueva visita
            _context.Visitas.Add(Visita);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}