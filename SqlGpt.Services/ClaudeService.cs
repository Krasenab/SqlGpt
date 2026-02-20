using Microsoft.Extensions.Configuration;
using SqlGpt.Infrastructure.InfrastructureModels;
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
        public async Task<string> GetResponseAsync(List<ClaudeMessage> messages, CancellationToken ct = default)
        {

            var apiKey = _config["Claude:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("Missing Claude:ApiKey (use secrets.json / user-secrets).");
            }

            var model = _config["Claude:Model"] ?? "claude-opus-4-6"; 
            var maxTokens = int.TryParse(_config["Claude:MaxTokens"], out var mt) ? mt : 250; // default ми е 512 tokena

            var systemPrompt = _config["Claude:SystemPrompt"];

            using var requestForClaude = new HttpRequestMessage(HttpMethod.Post, "v1/messages");

            requestForClaude.Headers.Add("x-api-key", apiKey);
            requestForClaude.Headers.Add("anthropic-version", "2023-06-01");
            requestForClaude.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            

            var payload = new
            {
                model = model,
                max_tokens = maxTokens,
                system = systemPrompt,
                temperature=0.1,
                messages = messages.Select(x => new
                {
                    role = x.Role,
                    content = x.Content
                }).ToList()
            };

            requestForClaude.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            using var responseFromClaude = await _http.SendAsync(requestForClaude, ct);
            var json = await responseFromClaude.Content.ReadAsStringAsync(ct);

            if (!responseFromClaude.IsSuccessStatusCode)
            {
                // throw new Exception($"Claude API error {(int)responseFromClaude.StatusCode}: {json}");
                throw new Exception("LLM service in unavailable");
            }
    
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
