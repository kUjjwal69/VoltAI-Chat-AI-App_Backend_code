using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WebApplication1.Interface;
using WebApplication1.Models;
using WebApplication1.Settings;

namespace WebApplication1.Services
{
    public class AIServices : IAIServices
    {
        private readonly HttpClient _httpClient;
        private readonly GroqSettings _settings;

        public AIServices(HttpClient httpClient, IOptions<GroqSettings> settings)
        {
            _settings = settings.Value;
            _httpClient = httpClient;

            // 🔍 Debug (temporary)
            Console.WriteLine("API KEY LENGTH: " + _settings.ApiKey?.Length);

            // ❌ Fail fast if key missing
            if (string.IsNullOrEmpty(_settings.ApiKey))
            {
                throw new Exception("Groq API Key is missing!");
            }

            // ✅ Correct way to set header
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _settings.ApiKey);
        }

        public async Task<string> ChatAsync(List<AIChatMessage> messages)
        {
            var requestBody = new
            {
                model = _settings.Model,
                messages = messages.Select(m => new { role = m.Role, content = m.Content })
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync(_settings.BaseUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Groq API error: {response.StatusCode} - {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "No response received.";
        }

        public async Task<string> AskAI(string question)
        {
            var messages = new List<AIChatMessage>
            {
                new AIChatMessage { Role = "user", Content = question }
            };
            return await ChatAsync(messages);
        }

       
    }
}