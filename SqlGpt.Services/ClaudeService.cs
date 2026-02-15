using Microsoft.Extensions.Configuration;
using SqlGpt.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SqlGpt.Services
{
    public class ClaudeService : IClaudeService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        public ClaudeService(HttpClient http, IConfiguration config) 
        {
           _http = http;
           _config = config;
        }
        public async Task<string> GetResponseAsync(string userMessage, CancellationToken ct = default)
        {

            var apiKey = _config["Claude:ApiKey"];
            
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new InvalidOperationException("Missing Claude:ApiKey (use secrets.json / user-secrets).");


            var model = _config["Claude:Model"] ?? "claude-opus-4-6"; 
            var maxTokens = int.TryParse(_config["Claude:MaxTokens"], out var mt) ? mt : 512;

            using var req = new HttpRequestMessage(HttpMethod.Post, "v1/messages");
            req.Headers.Add("x-api-key", apiKey);
            req.Headers.Add("anthropic-version", "2023-06-01");
            req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var payload = new
            {
                model = model,
                max_tokens = maxTokens,
                messages = new[]
                {
                    new { role = "user", content = userMessage }
                }
            };

            req.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var res = await _http.SendAsync(req, ct);
            var json = await res.Content.ReadAsStringAsync(ct);

            if (!res.IsSuccessStatusCode)
                throw new Exception($"Claude API error {(int)res.StatusCode}: {json}");

    
            using var doc = JsonDocument.Parse(json);
            var contentArr = doc.RootElement.GetProperty("content");
            foreach (var item in contentArr.EnumerateArray())
            {
                if (item.TryGetProperty("type", out var t) && t.GetString() == "text" &&
                    item.TryGetProperty("text", out var textEl))
                {
                    return textEl.GetString() ?? "";
                }
            }

            return ""; 
        }
    }
}
