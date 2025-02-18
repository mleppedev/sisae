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
using Microsoft.AspNetCore.SignalR.Client;
using sisae.DTOs;
using sisae.Services;
using Microsoft.AspNetCore.Authorization;

namespace sisae.Pages.Visitas
{
    [Authorize(Roles = "Administrador, Encargado")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly EventLoggerService _eventLoggerService;
        private readonly SignalRService _signalRService;

        // Constructor que inyecta la base de datos y el servicio de registro de eventos
        public CreateModel(ApplicationDbContext context, EventLoggerService eventLoggerService, SignalRService signalRService)
        {
            _context = context;
            _eventLoggerService = eventLoggerService;
            _signalRService = signalRService;
        }

        [BindProperty]
        public Visita Visita { get; set; }

        public List<SelectListItem> VisitadosSelectList { get; set; }
        public List<SelectListItem> VisitantesSelectList { get; set; }

        public async Task<IActionResult> OnGetAsync(string rut = null, bool nuevo = false)
        {
            if (nuevo)
            {
                LimpiarVisita();
                // Establecer la fecha y hora actuales
                Visita.Fecha_Visita = DateTime.Now.Date;
                Visita.Hora_Entrada = DateTime.Now.TimeOfDay;
            }
            else if (!string.IsNullOrEmpty(rut))
            {
                var visitante = await _context.Visitantes.FirstOrDefaultAsync(v => v.RUT == rut);
                if (visitante == null)
                {
                    await _eventLoggerService.LogEventAsync("VisitanteNoEncontrado", "Visitante no encontrado, redirigiendo a creación de visitante", User?.Identity?.Name);
                    return RedirectToPage("/Visitantes/Create", new { rut = rut });
                }
                else
                {
                    Visita = new Visita { ID_Visitante = visitante.ID_Visitante };
                }
            }

            await LlenarListasDesplegablesAsync();
            await _eventLoggerService.LogEventAsync("AccesoCrearVisita", "Acceso a la página de creación de visitas", User?.Identity?.Name);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validar solamente los campos que nos interesan (no las propiedades de navegación)
            ModelState.Remove("Visita.Visitado");
            ModelState.Remove("Visita.Visitante");

            // Estado por defecto
            if (string.IsNullOrWhiteSpace(Visita.Estado))
            {
                Visita.Estado = "Activa";
            }

            if (!ModelState.IsValid)
            {
                // Volver a cargar las listas en caso de error de validación
                await LlenarListasDesplegablesAsync();

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
                await LlenarListasDesplegablesAsync();

                // Registrar error de visitante no encontrado
                await _eventLoggerService.LogEventAsync("VisitanteNoEncontrado", "El visitante seleccionado no existe", User?.Identity?.Name);

                return Page();
            }

            // Verificar si el visitante está en los accesos prohibidos
            var accesoProhibido = await _context.AccesosProhibidos
                .FirstOrDefaultAsync(a => a.ID_Visitante == Visita.ID_Visitante &&
                                        (a.Fecha_Expiracion == null || a.Fecha_Expiracion > DateTime.Now));
            if (accesoProhibido != null)
            {
                // Registrar en consola y log
                Console.WriteLine($"El visitante con ID {Visita.ID_Visitante} está prohibido.");
                await _eventLoggerService.LogEventAsync(
                    "AccesoProhibidoDetectado",
                    $"Intento de registrar una visita para un visitante con acceso prohibido. ID Visitante: {Visita.ID_Visitante}, Motivo: {accesoProhibido.Motivo}",
                    User?.Identity?.Name
                );

                // Enviar al DashboardHub
                var visitaProhibidosDto = new VisitaProhibidosDto
                {
                    Fecha = DateTime.Now,
                    RUT = visitante.RUT,
                    Nombre = visitante.Nombre,
                    Apellido = visitante.Apellido,
                    Motivo = accesoProhibido.Motivo
                };

                try
                {
                    await _signalRService.SendAccesoProhibidoUpdateAsync(visitaProhibidosDto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al enviar el intento prohibido al DashboardHub: {ex.Message}");
                }

                // Agregar un error al modelo
                ModelState.AddModelError(string.Empty, "El visitante tiene acceso prohibido y no puede registrarse.");
                await LlenarListasDesplegablesAsync();
                return Page();
            }

            // Guardar la nueva visita
            _context.Visitas.Add(Visita);
            await _context.SaveChangesAsync();

            // Crear el DTO
            var dto = new VisitaDto
            {
                ID_Visita = Visita.ID_Visita,
                Fecha_Visita = Visita.Fecha_Visita.Add(Visita.Hora_Entrada),
                Estado = Visita.Estado,
                RUT = visitante?.RUT ?? string.Empty,
                Nombre = visitante?.Nombre ?? string.Empty,
                Apellido = visitante?.Apellido ?? string.Empty
            };
            Console.WriteLine($"DTO generado: ID={dto.ID_Visita}, Fecha={dto.Fecha_Visita}, Estado={dto.Estado}, RUT={dto.RUT}, Nombre={dto.Nombre}, Apellido={dto.Apellido}");

            // Invocar el método del servicio SignalR
            try
            {
                await _signalRService.SendVisitaUpdateAsync(dto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al invocar SignalRService: {ex.Message}");
                // O loggear el error
                throw;
            }

            // Registrar el evento de creación de visita
            await _eventLoggerService.LogEventAsync("CrearVisita", $"Visita creada con ID {Visita.ID_Visita}", User?.Identity?.Name);

            return RedirectToPage("./Index");
        }

        public async Task<IActionResult> OnGetVerifyRutAsync(string rut)
        {
            if (string.IsNullOrEmpty(rut))
            {
                return BadRequest("El RUT es requerido.");
            }

            var visitante = await _context.Visitantes.FirstOrDefaultAsync(v => v.RUT == rut);
            if (visitante == null)
            {
                return NotFound();
            }

            return new JsonResult(new { idVisitante = visitante.ID_Visitante, nombreVisitante = $"{visitante.Apellido}, {visitante.Nombre}" });
        }

        private async Task LlenarListasDesplegablesAsync()
        {
            // Generar lista de Visitantes
            VisitantesSelectList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "Seleccione..." } // Opción predeterminada
            };
            VisitantesSelectList.AddRange(await _context.Visitantes
                .OrderBy(v => v.Apellido)
                .ThenBy(v => v.Nombre)
                .Select(v => new SelectListItem
                {
                    Value = v.ID_Visitante.ToString(),
                    Text = $"{v.Apellido}, {v.Nombre} ({v.RUT})"
                }).ToListAsync());

            // Generar lista de Visitados
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
        }
        public void LimpiarVisita()
        {
            Visita = new Visita();
            Visita.ID_Visitante = 0; // Restablecer el campo ID_Visitante
        }
    }
}
