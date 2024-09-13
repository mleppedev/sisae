using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using System.Text.RegularExpressions;
using sisae.Data;
using sisae.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace sisae.Pages.Visitantes
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly GoogleCloudVisionService _visionService;

        public CreateModel(ApplicationDbContext context, GoogleCloudVisionService visionService)
        {
            _context = context;
            _visionService = visionService;
        }

        [BindProperty]
        public Visitante Visitante { get; set; }

        // Método principal para crear visitante
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Buscar el visitante en la base de datos usando el RUT
            var visitanteExistente = await _context.Visitantes.FirstOrDefaultAsync(v => v.RUT == Visitante.RUT);

            if (visitanteExistente != null)
            {
                // Si el visitante ya existe, actualiza sus datos
                visitanteExistente.Nombre = Visitante.Nombre;
                visitanteExistente.Apellido = Visitante.Apellido;
                visitanteExistente.FechaNacimiento = Visitante.FechaNacimiento;
                visitanteExistente.Direccion = Visitante.Direccion;
                visitanteExistente.Telefono = Visitante.Telefono;
                visitanteExistente.Email = Visitante.Email;
                visitanteExistente.Nacionalidad = Visitante.Nacionalidad;
                visitanteExistente.FechaVencimientoCarnet = Visitante.FechaVencimientoCarnet;
                visitanteExistente.FotoBiometrica = Visitante.FotoBiometrica;

                // Guardar los cambios en la base de datos
                _context.Visitantes.Update(visitanteExistente);
            }
            else
            {
                // Si no existe, crea un nuevo visitante
                var nuevoVisitante = new Visitante
                {
                    Nombre = Visitante.Nombre,
                    Apellido = Visitante.Apellido,
                    RUT = Visitante.RUT,
                    FechaNacimiento = Visitante.FechaNacimiento,
                    Direccion = Visitante.Direccion,
                    Telefono = Visitante.Telefono,
                    Email = Visitante.Email,
                    Nacionalidad = Visitante.Nacionalidad,
                    FechaVencimientoCarnet = Visitante.FechaVencimientoCarnet,
                    FotoBiometrica = Visitante.FotoBiometrica
                };

                _context.Visitantes.Add(nuevoVisitante);
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }

        // Método que procesa el OCR de la cédula y busca si el visitante ya existe
        public async Task<IActionResult> OnPostScanCedulaAsync([FromBody] JsonElement data)
        {
            try
            {
                if (data.TryGetProperty("imageBase64", out JsonElement base64ImageElement))
                {
                    string base64Image = base64ImageElement.GetString();

                    if (string.IsNullOrEmpty(base64Image))
                    {
                        return new JsonResult(new { success = false, message = "No se ha proporcionado una imagen válida." });
                    }

                    // Procesar la imagen con Google Cloud Vision OCR
                    var ocrResult = await _visionService.DetectTextFromImageAsync(base64Image);

                    // Parsear la respuesta JSON del OCR
                    JsonDocument ocrResponse = JsonDocument.Parse(ocrResult);
                    var textAnnotations = ocrResponse.RootElement
                        .GetProperty("responses")[0]
                        .GetProperty("textAnnotations");

                    // Obtener el texto completo extraído
                    string ocrText = textAnnotations[0].GetProperty("description").GetString();

                    // Extraer los datos del MRZ (usando las funciones ya creadas)
                    string run = ExtractRunMRZ(ocrText);
                    string[] nombres = ExtractNombreCompletoMRZ(ocrText);
                    string primerApellido = nombres[0];
                    string segundoApellido = nombres[1];
                    string nombre = nombres[2];
                    string nacimiento = ExtractFechaNacimientoMRZ(ocrText);
                    string vencimiento = ExtractFechaVencimientoMRZ(ocrText);
                    string nacionalidad = ExtractNacionalidadMRZ(ocrText); // Extraer la nacionalidad

                    // Comprobar si el visitante ya está registrado en la base de datos por su RUT
                    var visitanteExistente = await _context.Visitantes.FirstOrDefaultAsync(v => v.RUT == run);

                    if (visitanteExistente != null)
                    {
                        // Si el visitante ya existe, devolver los datos del visitante
                        return new JsonResult(new
                        {
                            success = true,
                            run = visitanteExistente.RUT,
                            nombre = visitanteExistente.Nombre,
                            primerApellido = visitanteExistente.Apellido.Split(' ')[0],
                            segundoApellido = visitanteExistente.Apellido.Split(' ').Length > 1 ? visitanteExistente.Apellido.Split(' ')[1] : "",
                            direccion = visitanteExistente.Direccion,
                            telefono = visitanteExistente.Telefono,
                            email = visitanteExistente.Email,
                            nacionalidad = visitanteExistente.Nacionalidad, // Devolver nacionalidad
                            nacimiento = visitanteExistente.FechaNacimiento.ToString("yyyy-MM-dd"),
                            vencimiento = visitanteExistente.FechaVencimientoCarnet.ToString("yyyy-MM-dd")
                        });
                    }
                    else
                    {
                        // Si el visitante no existe, devolver solo los datos extraídos de la cédula
                        return new JsonResult(new
                        {
                            success = true,
                            run,
                            nombre,
                            primerApellido,
                            segundoApellido,
                            nacionalidad,  // Nacionalidad extraída del MRZ
                            nacimiento,
                            vencimiento
                        });
                    }
                }
                else
                {
                    return new JsonResult(new { success = false, message = "No se encontró el campo 'imageBase64'." });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        private string ExtractFechaNacimientoMRZ(string ocrText)
        {
            // Extraer la fecha de nacimiento en formato YYMMDD antes de la letra "M" o "F"
            var nacimientoRegex = new Regex(@"(\d{2})(\d{2})(\d{2})\d{1}[MF]");
            var match = nacimientoRegex.Match(ocrText);
            if (match.Success)
            {
                string year = match.Groups[1].Value;
                string month = match.Groups[2].Value;
                string day = match.Groups[3].Value;

                // Si el año es mayor a 50, lo asumimos del siglo XX, si es menor, del siglo XXI
                int yearInt = int.Parse(year);
                string fullYear = yearInt > 50 ? "19" + year : "20" + year;

                // Devolver la fecha en formato dd-mm-yyyy
                return $"{day}-{month}-{fullYear}";
            }
            return "Fecha de nacimiento no encontrada";
        }

        private string ExtractFechaVencimientoMRZ(string ocrText)
        {
            // Extraer la fecha de vencimiento en formato YYMMDD después de la letra "M" o "F"
            var vencimientoRegex = new Regex(@"[MF](\d{2})(\d{2})(\d{2})");
            var match = vencimientoRegex.Match(ocrText);
            if (match.Success)
            {
                string year = match.Groups[1].Value;
                string month = match.Groups[2].Value;
                string day = match.Groups[3].Value;

                // Asumimos que las fechas de vencimiento siempre son del siglo XXI
                string fullYear = "20" + year;

                // Devolver la fecha en formato dd-mm-yyyy
                return $"{day}-{month}-{fullYear}";
            }
            return "Fecha de vencimiento no encontrada";
        }

        private string ExtractNacionalidadMRZ(string ocrText)
        {
            // Expresión regular para extraer la sigla del país en formato INCHL
            var nacionalidadRegex = new Regex(@"IN([A-Z]{3})");
            var match = nacionalidadRegex.Match(ocrText);
            return match.Success ? match.Groups[1].Value : "Nacionalidad no encontrada";
        }

        private string[] ExtractNombreCompletoMRZ(string ocrText)
        {
            // Expresión regular para extraer la parte del nombre y apellidos en formato APELLIDO1<APELLIDO2<<NOMBRES
            var nombreRegex = new Regex(@"([A-Z]+)<([A-Z]+)<<([A-Z<]+)");
            var match = nombreRegex.Match(ocrText);

            if (match.Success)
            {
                string primerApellido = match.Groups[1].Value.Replace("<", " ").Trim();  // Primer apellido
                string segundoApellido = match.Groups[2].Value.Replace("<", " ").Trim(); // Segundo apellido
                string nombre = match.Groups[3].Value.Replace("<", " ").Trim();          // Nombre, reemplazando los < por espacios

                // Devolver los apellidos y nombre en un array
                return new string[] { primerApellido, segundoApellido, nombre };
            }

            return new string[] { "Primer apellido no encontrado", "Segundo apellido no encontrado", "Nombre no encontrado" };
        }

        private string ExtractNumDocMRZ(string ocrText)
        {
            // Extraer el número de documento del MRZ (ubicado después de INCHL)
            var numDocRegex = new Regex(@"INCHL(\d{9})");
            var match = numDocRegex.Match(ocrText);
            return match.Success ? match.Groups[1].Value : "Número de documento no encontrado";
        }

        private string ExtractRunMRZ(string ocrText)
        {
            // Expresión regular para extraer la parte del RUT en formato CHLXXXXXXXX<DV
            var runRegex = new Regex(@"CHL(\d{7,8})<([0-9Kk])");
            var match = runRegex.Match(ocrText);

            if (match.Success)
            {
                string run = match.Groups[1].Value; // Parte numérica del RUN
                string dv = match.Groups[2].Value;  // Dígito verificador

                // Formatear el RUN añadiendo los puntos y el guion
                string formattedRun = $"{run.Substring(0, 2)}.{run.Substring(2, 3)}.{run.Substring(5)}-{dv.ToUpper()}";

                return formattedRun;
            }

            return "RUN no encontrado";
        }
    }
}