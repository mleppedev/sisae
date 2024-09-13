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

        public InformesModel(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [BindProperty]
        public string Consulta { get; set; }

        public List<Dictionary<string, object>> Respuesta { get; set; } // Cambiado a List<Dictionary<string, object>>
        public string SqlQuery { get; set; }

        public async Task OnPostAsync()
        {
            if (!string.IsNullOrEmpty(Consulta))
            {
                int maxIteraciones = 5;
                for (int i = 0; i < maxIteraciones; i++)
                {
                    string sqlQuery = await GenerarConsultaSQL(Consulta);

                    try
                    {
                        // Ejecutar la consulta generada
                        Respuesta = await EjecutarConsultaSQL(sqlQuery);
                        break;
                    }
                    catch (SqlException ex)
                    {
                        if (i == maxIteraciones - 1)
                        {
                            Respuesta = new List<Dictionary<string, object>> { new Dictionary<string, object> { { "Error", "No se pudo generar una consulta válida después de varios intentos." } } };
                        }
                        else
                        {
                            // Si hay error, corregir
                            Consulta = await CorregirErrorSQL(sqlQuery, ex.Message);
                        }
                    }
                }
            }
        }

        // Método para corregir el error de SQL
        private async Task<string> CorregirErrorSQL(string sqlQuery, string errorMessage)
        {
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
                    return jsonResponse.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                }
                else
                {
                    return sqlQuery; // Si no puede corregir el error, devolver la consulta original
                }
            }
        }

        // Método para ejecutar la consulta SQL generada
        private async Task<List<Dictionary<string, object>>> EjecutarConsultaSQL(string sqlQuery)
        {
            if (string.IsNullOrEmpty(sqlQuery))
            {
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
                        return resultado;
                    }
                    else
                    {
                        return new List<Dictionary<string, object>> { new Dictionary<string, object> { { "Mensaje", "No se encontraron resultados." } } };
                    }
                }
                catch (SqlException ex)
                {
                    return new List<Dictionary<string, object>> { new Dictionary<string, object> { { "Error", $"Error al ejecutar la consulta SQL: {ex.Message}" } } };
                }
            }
        }

        // Método para generar la consulta SQL en base a la pregunta natural
        private async Task<string> GenerarConsultaSQL(string consultaNatural)
        {
            string estructuraBaseDeDatos = await LeerEstructuraBaseDeDatos();

            if (string.IsNullOrEmpty(estructuraBaseDeDatos))
            {
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
                        new { role = "system", content = "No uses comillas dobles para los nombres de las columnas o tablas. Usa corchetes si es necesario." },
                        new { role = "system", content = "Asegúrate de que todas las consultas incluyan el formato estándar de SQL Server, y que estén correctamente alineadas con las mejores prácticas de SQL." },
                        new { role = "system", content = "Si la consulta involucra buscar algo o alguien, utiliza la cláusula SQL 'LIKE' con comodines para encontrar coincidencias aproximadas. Usa '%nombre%' por ejemplo para buscar donde el nombre está contenido en cualquier parte del campo." }
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
                    string sqlQuery = jsonResponse.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                    sqlQuery = LimpiarConsultaSQL(sqlQuery);
                    SqlQuery = sqlQuery;
                    return SqlQuery;
                }
                else
                {
                    return null;
                }
            }
        }

        // Método para generar una respuesta en lenguaje natural a partir del resultado SQL
        private async Task<string> GenerarRespuestaNatural(string resultadoSQL)
        {
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
                    return jsonResponse.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
                }
                else
                {
                    return "Error al generar la respuesta.";
                }
            }
        }

        private async Task<string> LeerEstructuraBaseDeDatos()
        {
            string filePath = Path.Combine(_env.WebRootPath, "Data", "sisae.json");

            if (System.IO.File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            else
            {
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

            return consulta;
        }
    }
}