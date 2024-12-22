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
        private readonly EventLoggerService _eventLoggerService;

        // Constructor que inyecta la base de datos y el servicio de registro de eventos
        public CreateModel(ApplicationDbContext context, EventLoggerService eventLoggerService)
        {
            _context = context;
            _eventLoggerService = eventLoggerService;
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

            // Registrar el acceso a la creación de visitas
            await _eventLoggerService.LogEventAsync("AccesoCrearVisita", "Acceso a la página de creación de visitas", User?.Identity?.Name);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validar solamente los campos que nos interesan (no las propiedades de navegación)
            ModelState.Remove("Visita.Visitado");
            ModelState.Remove("Visita.Visitante");

            if (!ModelState.IsValid)
            {
                // Volver a cargar las listas en caso de error de validación
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

                // Registrar error de validación
                await _eventLoggerService.LogEventAsync("ErrorValidacionCrear", "Error de validación al crear una nueva visita", User?.Identity?.Name);

                return Page();
            }

            // Verificar si el visitante existe
            var visitante = await _context.Visitantes.FindAsync(Visita.ID_Visitante);
            if (visitante == null)
            {
                ModelState.AddModelError("Visita.ID_Visitante", "El visitante seleccionado no existe");

                // Volver a cargar las listas en caso de error
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

                // Registrar error de visitante no encontrado
                await _eventLoggerService.LogEventAsync("VisitanteNoEncontrado", "El visitante seleccionado no existe", User?.Identity?.Name);

                return Page();
            }

            // Guardar la nueva visita
            _context.Visitas.Add(Visita);
            await _context.SaveChangesAsync();

            // Registrar el evento después de guardar la visita
            await _eventLoggerService.LogEventAsync("CrearVisita", $"Visita creada con ID {Visita.ID_Visita}", User?.Identity?.Name);

            return RedirectToPage("./Index");
        }
    }
}
