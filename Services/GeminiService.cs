using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using SumaryYoutubeBackend.interfaces;
using SumaryYoutubeBackend.DTOs;
using SumaryYoutubeBackend.Models;
namespace SumaryYoutubeBackend.Services
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apikey;

        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apikey = configuration["ExternalServices:GeminiApiKey"];
        }

        public async Task<GeminiResult> GenerateSumaryAsync(string transcript)
        {
            
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apikey}";
            var prompt = "Analise a seguinte transcrição de vídeo e retorne um resumo estruturado em tópicos explicativos." + $"No final, gere um código Mermaid.js para um mapa mental do conteúdo. " + $"Transcrição:\n\n{transcript}";

            var requestBody = new
            {
                contents = new[]{
                    new {parts = new[] {new {text = prompt}}}
                }

            };
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var detailsError = await response.Content.ReadAsStringAsync();
                throw new Exception("Erro ao chama Gemini:" + response.ReasonPhrase);
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var result = JsonSerializer.Deserialize<GeminiResponseDto>(jsonResponse, options);


            var fullText = result?.Candidates?[0].Content.Parts[0].Text ?? "Erro ao gerar resumo";

            var responseSumary = new GeminiResult();
            if (fullText.Contains("```mermaid"))
            {
                var parts = fullText.Split(new[] { "```mermaid", "```" }, StringSplitOptions.None);
                responseSumary.Summary = parts[0].Trim();
                responseSumary.MindMap = parts.Length > 1 ? parts[1].Trim() : null;
            }
            else
            {
                responseSumary.Summary = fullText;
                responseSumary.MindMap = null;
            }

            return responseSumary;

        }
    }
}