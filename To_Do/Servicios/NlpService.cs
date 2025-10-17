using DB_ToDo;
using System.Text.Json.Serialization;

namespace To_Do.Servicios
{
    public class NlpResult
    {
        [JsonPropertyName("dia")]
        public string Day { get; set; }

        [JsonPropertyName("mes")]
        public string Month { get; set; }

        [JsonPropertyName("hora")]
        public string Hour { get; set; }

        [JsonPropertyName("fecha_completa")]
        public string Full_Date { get; set; }

        [JsonPropertyName("detecto_recordatorio")]
        public bool Reminder_Detected { get; set; }

        [JsonPropertyName("texto_original")]
        public string Original_Text { get; set; }

        [JsonPropertyName("entidades_detectadas")]
        public List<string> Entities_Detected { get; set; }
    }

    public class NlpService
    {
        private readonly HttpClient _http;
        private readonly ToDoContext _context;

        public NlpService(HttpClient http, ToDoContext context)
        {
            _http = http;
            _context = context;
        }

        public async Task<NlpResult> AnalyzeTextAsync(string text)
        {
            var requestBody = new
            {
                texto = text
            };

            var response = await _http.PostAsJsonAsync("http://127.0.0.1:8000/analizar/", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorMsg = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error NLP API: {response.StatusCode} - {errorMsg}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<NlpResult>();
        }
    }
}
