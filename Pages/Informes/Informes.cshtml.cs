using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Data.SqlClient; // Para ejecutar las consultas SQL dinámicas
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic; // Para usar List y Dictionary
using System.IO; // Para usar Path y StreamReader

namespace sisae.Pages.Informes
{
    public class InformesModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly EventLoggerService _eventLoggerService;
        private const int MaxIteraciones = 5;

        public InformesModel(IConfiguration configuration, IWebHostEnvironment env, EventLoggerService eventLoggerService)
        {
            _configuration = configuration;
            _env = env;
            _eventLoggerService = eventLoggerService;
        }

        [BindProperty]
        public string Consulta { get; set; }

        [BindProperty]
        public string SelectedQuery { get; set; }

        public List<Dictionary<string, object>> Respuesta { get; set; } // Cambiado a List<Dictionary<string, object>>
        public string SqlQuery { get; set; }

        public async Task OnPostAsync()
        {
            if (!string.IsNullOrEmpty(SelectedQuery))
            {
                // Asignar la consulta en base a la opción seleccionada
                switch (SelectedQuery)
                {
                    case "1":
                        SqlQuery = "SELECT TOP 1 CONCAT(VU.NOMBRE, ' ', VU.APELLIDO) AS VISITANTE, COUNT(V.ID_Visita) AS TOTAL_VISITAS FROM Visitas V JOIN Visitantes VU ON V.ID_Visitante = VU.ID_Visitante GROUP BY CONCAT(VU.NOMBRE, ' ', VU.APELLIDO) ORDER BY TOTAL_VISITAS DESC;";
                        break;
                    case "2":
                        SqlQuery = "SELECT TOP 100 UPPER(CONCAT(v.NOMBRE, ' ', v.APELLIDO)) AS VISITANTE, CONVERT(VARCHAR, a.FECHA_PROHIBICION, 103) AS 'FECHA PROHIBICION', CONVERT(VARCHAR, a.FECHA_EXPIRACION, 103) AS 'FECHA EXPIRACION', UPPER(a.MOTIVO) AS MOTIVO FROM AccesosProhibidos a JOIN Visitantes v ON a.ID_Visitante = v.ID_Visitante ORDER BY a.FECHA_PROHIBICION DESC, a.FECHA_EXPIRACION DESC;";
                        break;
                    case "3":
                        SqlQuery = "SELECT COUNT(*) AS TOTAL_VISITAS_HOY FROM Visitas WHERE CONVERT(VARCHAR, Fecha_Visita, 103) = CONVERT(VARCHAR, GETDATE(), 103);";
                        break;
                    case "4":
                        SqlQuery = "SELECT TOP 100 UPPER(NOMBRE + ' ' + APELLIDO) AS VISITADO, UPPER(CARGO) AS CARGO, UPPER(DEPARTAMENTO) AS DEPARTAMENTO, UPPER(EMAIL) AS EMAIL, TELEFONO FROM Visitados ORDER BY VISITADO;";
                        break;
                    case "5":
                        SqlQuery = "SELECT TOP 100 UPPER(CONCAT(vd.Nombre, ' ', vd.Apellido)) AS VISITADO, COUNT(v.ID_Visita) AS TOTAL_VISITAS FROM Visitas v JOIN Visitados vd ON v.ID_Visitado = vd.ID_Visitado GROUP BY vd.Nombre, vd.Apellido ORDER BY COUNT(v.ID_Visita) DESC, MAX(v.Fecha_Visita) DESC, MAX(v.Hora_Entrada) DESC;";
                        break;
                    case "6":
                        SqlQuery = "SELECT TOP 100 CONCAT(VISITANTES.NOMBRE, ' ', VISITANTES.APELLIDO) AS VISITANTE, CONCAT(VISITADOS.NOMBRE, ' ', VISITADOS.APELLIDO) AS VISITADO, COUNT(Visitas.ID_Visita) AS CANTIDAD_VISITAS FROM Visitas JOIN Visitantes ON Visitas.ID_Visitante = Visitantes.ID_Visitante JOIN Visitados ON Visitas.ID_Visitado = Visitados.ID_Visitado GROUP BY CONCAT(VISITANTES.NOMBRE, ' ', VISITANTES.APELLIDO), CONCAT(VISITADOS.NOMBRE, ' ', VISITADOS.APELLIDO) ORDER BY CANTIDAD_VISITAS DESC;";
                        break;
                    case "7":
                        SqlQuery = "SELECT TOP 1000 [ID] AS IDENTIFICADOR, [EventType] AS TIPO_EVENTO, [Description] AS DESCRIPCIÓN, [EventDate] AS FECHA_EVENTO, [UserId] AS ID_USUARIO FROM [sisae_db].[dbo].[EventLogs] ORDER BY [EventDate] DESC;";
                        break;
                    // Agregar más casos según sea necesario
                }

                // Ejecutar la consulta directamente
                Respuesta = await EjecutarConsultaSQL(SqlQuery);
            }
            else if (!string.IsNullOrEmpty(Consulta))
            {
                // Lógica existente para manejar consultas en lenguaje natural
                await _eventLoggerService.LogEventAsync("OnPostAsync", $"Consulta recibida del usuario: {Consulta}", User?.Identity?.Name);

                int maxIteraciones = 5;
                for (int i = 0; i < maxIteraciones; i++)
                {
                    await _eventLoggerService.LogEventAsync("GenerarConsultaSQL", $"Intento #{i + 1} de generar la consulta SQL...", User?.Identity?.Name);
                    string sqlQuery = await GenerarConsultaSQL(Consulta);

                    try
                    {
                        // Intenta ejecutar la consulta generada
                        Respuesta = await EjecutarConsultaSQL(sqlQuery);
                        SqlQuery = sqlQuery; // Muestra la consulta generada en la UI
                        break; // Sale del bucle si la ejecución es exitosa
                    }
                    catch (SqlException ex)
                    {
                        await HandleSQLError(i, sqlQuery, ex);
                    }
                }
            }
        }

        private async Task HandleSQLError(int iteration, string sqlQuery, SqlException ex)
        {
            // Log: error al ejecutar la consulta
            await _eventLoggerService.LogEventAsync(
                "EjecutarConsultaSQL_Error",
                $"Error al ejecutar la consulta. Mensaje: {ex.Message}",
                User?.Identity?.Name
            );

            // Si ya es el último intento, devolver el error
            int maxIteraciones = 5;
            if (iteration == maxIteraciones - 1)
            {
                Respuesta = new List<Dictionary<string, object>> 
                { 
                    new Dictionary<string, object> { { "Error", "No se pudo generar una consulta válida después de varios intentos." } } 
                };

                await _eventLoggerService.LogEventAsync(
                    "GenerarConsultaSQL_Fallo",
                    "Se agotaron los intentos de corrección de la consulta SQL sin éxito.",
                    User?.Identity?.Name
                );
            }
            else
            {
                // Log: se procede a corregir la consulta
                await _eventLoggerService.LogEventAsync(
                    "CorregirErrorSQL",
                    $"Intentando corregir la consulta SQL (Iteración #{iteration + 1}). Error original: {ex.Message}",
                    User?.Identity?.Name
                );

                // Si hay error, corregir
                Consulta = await CorregirErrorSQL(sqlQuery, ex.Message);
            }
        }

        // Método para corregir el error de SQL
        private async Task<string> CorregirErrorSQL(string sqlQuery, string errorMessage)
        {
            // Log: inicio de la corrección del error
            await _eventLoggerService.LogEventAsync(
                "CorregirErrorSQL",
                $"Iniciando corrección de error SQL con mensaje: {errorMessage}",
                User?.Identity?.Name
            );

            using (HttpClient client = new HttpClient())
            {
                var requestBody = new
                {
                    model = "gpt-4o",
                    messages = new[]
                    {
                        new { role = "system", content = $"Corrige la siguiente consulta SQL en base al error devuelto por SQL Server: {errorMessage}" },
                        new { role = "user", content = sqlQuery }
                    },
                    max_tokens = 1500
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", _configuration["ApiKeys:OpenAI"]);

                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonDocument.Parse(result);

                    // Log: corrección recibida satisfactoriamente
                    await _eventLoggerService.LogEventAsync(
                        "CorregirErrorSQL",
                        $"Se recibió una posible corrección de la consulta SQL.",
                        User?.Identity?.Name
                    );

                    return jsonResponse.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                }
                else
                {
                    // Log: fallo al contactar la API de OpenAI
                    await _eventLoggerService.LogEventAsync(
                        "CorregirErrorSQL_Error",
                        $"No se pudo contactar a la API para corregir el SQL. Codigo HTTP: {response.StatusCode}",
                        User?.Identity?.Name
                    );

                    return sqlQuery; // Si no puede corregir el error, devolver la consulta original
                }
            }
        }

        // Método para ejecutar la consulta SQL generada
        private async Task<List<Dictionary<string, object>>> EjecutarConsultaSQL(string sqlQuery)
        {
            if (string.IsNullOrEmpty(sqlQuery))
            {
                // Log: la consulta está vacía
                await _eventLoggerService.LogEventAsync(
                    "EjecutarConsultaSQL",
                    "La consulta SQL está vacía o no es válida, se retorna un error.",
                    User?.Identity?.Name
                );

                return new List<Dictionary<string, object>> { new Dictionary<string, object> { { "Error", "La consulta SQL está vacía o no es válida." } } };
            }

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                await connection.OpenAsync();

                try
                {
                    SqlDataReader reader = await command.ExecuteReaderAsync();
                    var resultado = new List<Dictionary<string, object>>();

                    if (reader.HasRows)
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader[i];
                            }
                            resultado.Add(row);
                        }

                        // Log: se obtuvo un conjunto de filas
                        await _eventLoggerService.LogEventAsync(
                            "EjecutarConsultaSQL",
                            $"Se obtuvieron {resultado.Count} filas en la consulta.",
                            User?.Identity?.Name
                        );

                        return resultado;
                    }
                    else
                    {
                        // Log: la consulta no devolvió filas
                        await _eventLoggerService.LogEventAsync(
                            "EjecutarConsultaSQL",
                            "La consulta no arrojó resultados (HasRows = false).",
                            User?.Identity?.Name
                        );

                        return new List<Dictionary<string, object>> { new Dictionary<string, object> { { "Mensaje", "No se encontraron resultados." } } };
                    }
                }
                catch (SqlException ex)
                {
                    // Log: error al ejecutar la consulta
                    await _eventLoggerService.LogEventAsync(
                        "EjecutarConsultaSQL_Exception",
                        $"Excepción SQL: {ex.Message}",
                        User?.Identity?.Name
                    );

                    return new List<Dictionary<string, object>> { new Dictionary<string, object> { { "Error", $"Error al ejecutar la consulta SQL: {ex.Message}" } } };
                }
            }
        }

        // Método para generar la consulta SQL en base a la pregunta natural
        private async Task<string> GenerarConsultaSQL(string consultaNatural)
        {
            // Log: inicio de la generación de la consulta SQL
            await _eventLoggerService.LogEventAsync(
                "GenerarConsultaSQL",
                "Iniciando la generación de la consulta en base al lenguaje natural proporcionado por el usuario.",
                User?.Identity?.Name
            );

            string estructuraBaseDeDatos = await LeerEstructuraBaseDeDatos();

            if (string.IsNullOrEmpty(estructuraBaseDeDatos))
            {
                // Log: no se encontró la estructura de la BD
                await _eventLoggerService.LogEventAsync(
                    "GenerarConsultaSQL_Error",
                    "No se pudo leer la estructura de la base de datos (archivo sisae.json).",
                    User?.Identity?.Name
                );

                return "Error: No se pudo leer la estructura de la base de datos.";
            }

            using (HttpClient client = new HttpClient())
            {
                var requestBody = new
                {
                    model = "gpt-4o",
                    messages = new[]
                    {
                        new { role = "system", content = "Eres un experto en SQL Server que genera consultas SQL para bases de datos SQL Server basadas en la siguiente estructura de la base de datos: " + estructuraBaseDeDatos },
                        new { role = "user", content = consultaNatural },
                        new { role = "system", content = "Proporciona solo la consulta SQL, sin ninguna explicación, ni contexto adicional, ni formateo de código como ```." },
                        new { role = "system", content = "Asegúrate de que la consulta esté correctamente optimizada y sea compatible con SQL Server. Usa 'TOP' en lugar de 'LIMIT'." },
                        new { role = "system", content = "No incluyas ninguna columna cuyo nombre contenga 'ID'. Estas columnas deben ser excluidas del resultado final." },
                        new { role = "system", content = "Evita separar nombres y apellidos. En su lugar, combina ambos en un solo campo llamado 'VISITANTE' o 'VISITADO' según corresponda." },
                        new { role = "system", content = "Asegúrate de que todas las consultas sigan el estándar de SQL Server. Los comandos SQL como SELECT, FROM, WHERE, etc., deben estar en MAYÚSCULAS." },
                        new { role = "system", content = "Evita usar símbolos especiales como corchetes, comillas dobles o caracteres innecesarios en los nombres de las tablas y columnas." },
                        new { role = "system", content = "Entrega las fechas en formato 'dd/MM/yyyy' usando CONVERT(VARCHAR, campo_fecha, 103). Entrega las horas en formato 'HH:mm' usando CONVERT(VARCHAR, campo_hora, 108)." },
                        new { role = "system", content = "Renombra las columnas en el resultado con nombres amigables utilizando 'AS'. Por ejemplo, 'employee_name' como 'Nombre del Empleado'." },
                        new { role = "system", content = "Incluye solo la información relevante para la consulta y limita los resultados a un máximo de 100 filas utilizando 'TOP 100'." },
                        new { role = "system", content = "Ordena los resultados primero por 'FECHA VISITA' (más reciente primero) y luego por 'HORA ENTRADA' (más reciente primero)." },
                        new { role = "system", content = "Todas las cadenas de texto deben ser devueltas en MAYÚSCULAS para mantener consistencia." },
                        new { role = "system", content = "Asegúrate de que los nombres de las columnas y los valores de las filas estén ordenados de forma lógica, presentando primero nombres completos, seguidos por fechas, horas y luego los detalles relevantes." }
                    },
                    max_tokens = 1500
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", _configuration["ApiKeys:OpenAI"]);

                // Log: enviando la solicitud a la API de OpenAI
                await _eventLoggerService.LogEventAsync(
                    "GenerarConsultaSQL",
                    "Enviando la petición a OpenAI para generar la consulta SQL.",
                    User?.Identity?.Name
                );

                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonDocument.Parse(result);
                    string sqlQuery = jsonResponse.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                    sqlQuery = LimpiarConsultaSQL(sqlQuery);
                    SqlQuery = sqlQuery;

                    return SqlQuery;
                }
                else
                {
                    // Log: fallo de la API
                    await _eventLoggerService.LogEventAsync(
                        "GenerarConsultaSQL_Error",
                        $"La API de OpenAI devolvió un error: {response.StatusCode}",
                        User?.Identity?.Name
                    );

                    return null;
                }
            }
        }

        // Método para generar una respuesta en lenguaje natural a partir del resultado SQL
        private async Task<string> GenerarRespuestaNatural(string resultadoSQL)
        {
            // Log: inicio de la conversión de resultado SQL a lenguaje natural
            await _eventLoggerService.LogEventAsync(
                "GenerarRespuestaNatural",
                "Convirtiendo el resultado SQL a una respuesta en lenguaje natural.",
                User?.Identity?.Name
            );

            using (HttpClient client = new HttpClient())
            {
                var requestBody = new
                {
                    model = "gpt-4o",
                    messages = new[]
                    {
                        new { role = "system", content = "Convierte los siguientes datos SQL en una respuesta en lenguaje natural clara y organizada." },
                        new { role = "user", content = $"Convierte los siguientes datos SQL en una respuesta estructurada: {resultadoSQL}" }
                    },
                    max_tokens = 1500
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Add("Authorization", _configuration["ApiKeys:OpenAI"]);

                var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var jsonResponse = JsonDocument.Parse(result);

                    // Log: respuesta en lenguaje natural generada con éxito
                    await _eventLoggerService.LogEventAsync(
                        "GenerarRespuestaNatural",
                        "Respuesta en lenguaje natural generada correctamente.",
                        User?.Identity?.Name
                    );

                    return jsonResponse.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                }
                else
                {
                    // Log: fallo al solicitar la generación del texto
                    await _eventLoggerService.LogEventAsync(
                        "GenerarRespuestaNatural_Error",
                        $"Error al generar la respuesta natural. HTTP Status: {response.StatusCode}",
                        User?.Identity?.Name
                    );

                    return "Error al generar la respuesta.";
                }
            }
        }

        private async Task<string> LeerEstructuraBaseDeDatos()
        {
            // Log: leyendo el archivo con la estructura de la BD
            await _eventLoggerService.LogEventAsync(
                "LeerEstructuraBaseDeDatos",
                "Intentando leer la estructura de la base de datos desde el archivo sisae.json",
                User?.Identity?.Name
            );

            string filePath = Path.Combine(_env.WebRootPath, "Data", "sisae.json");

            if (System.IO.File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    // Log: archivo leído con éxito
                    await _eventLoggerService.LogEventAsync(
                        "LeerEstructuraBaseDeDatos",
                        "Archivo sisae.json leído correctamente.",
                        User?.Identity?.Name
                    );

                    return await reader.ReadToEndAsync();
                }
            }
            else
            {
                // Log: archivo no encontrado
                await _eventLoggerService.LogEventAsync(
                    "LeerEstructuraBaseDeDatos_Error",
                    "No se encontró el archivo sisae.json en la carpeta Data.",
                    User?.Identity?.Name
                );

                return null; // Manejar el error si el archivo no existe
            }
        }

        private string LimpiarConsultaSQL(string consulta)
        {
            // Elimina etiquetas de código como ```sql o ```
            consulta = consulta.Replace("```sql", "").Replace("```", "");

            // Elimina saltos de línea innecesarios
            consulta = consulta.Replace("\n", " ").Replace("\r", " ");

            // Reemplaza múltiples espacios por un solo espacio
            consulta = System.Text.RegularExpressions.Regex.Replace(consulta, @"\s+", " ");

            // Quita espacios innecesarios al principio o final
            consulta = consulta.Trim();

            // Log: se ha limpiado la consulta SQL
            //_eventLoggerService no es async => no usar await, o bien hacerlo en un método async separado si lo deseas
            _eventLoggerService.LogEventAsync(
                "LimpiarConsultaSQL",
                $"Consulta SQL limpiada: {consulta}",
                User?.Identity?.Name
            );

            return consulta;
        }
    }
}