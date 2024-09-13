using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace sisae.Pages.Help
{
    public class HelpModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env; // Necesario para acceder a los archivos

        public HelpModel(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [BindProperty]
        public string Consulta { get; set; }

        public string Respuesta { get; set; }

        public async Task OnPostAsync()
        {
            if (!string.IsNullOrEmpty(Consulta))
            {
                // Leer el archivo faq.json
                string faqData = await LeerFAQJson();

                if (string.IsNullOrEmpty(faqData))
                {
                    Respuesta = "No se pudo cargar la información del sistema.";
                    return;
                }

                // Llamada a ChatGPT con la consulta y los datos del FAQ
                string respuestaGPT = await ConsultarChatGPT(Consulta, faqData);
                Respuesta = respuestaGPT ?? "Lo siento, no pude obtener una respuesta.";
            }
        }

        // M�todo para consultar a ChatGPT con los datos del FAQ
        private async Task<string> ConsultarChatGPT(string consulta, string faqData)
        {
            using (HttpClient client = new HttpClient())
            {
                var requestBody = new
                {
                    model = "gpt-4o",
                    messages = new[]
                    {
                        new { role = "system", content = "Eres un asistente de soporte técnico que responde preguntas sobre el sistema Sisae. Aquí tienes la información completa sobre el sistema y preguntas frecuentes: " + faqData },
                        new { role = "user", content = consulta }
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
                    return null;
                }
            }
        }

        // M�todo para leer el archivo FAQ
        private async Task<string> LeerFAQJson()
        {
            string filePath = Path.Combine(_env.WebRootPath, "Data", "faq.json");

            if (System.IO.File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    return await reader.ReadToEndAsync();
                }
            }
            else
            {
                return null;
            }
        }
    }
}