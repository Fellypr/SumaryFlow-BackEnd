using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using SumaryYoutubeBackend.interfaces;
using SumaryYoutubeBackend.DTOs;
using SumaryYoutubeBackend.Models;
using SumaryYoutubeBackend.dbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq;
namespace SumaryYoutubeBackend.Services
{
    public class GeminiService : IGeminiService, IGetGeminiServiceUserAsync
    {
        private readonly HttpClient _httpClient;
        private readonly string _apikey;
        private readonly SumaryYoutubeDbContext _context;

        public GeminiService(HttpClient httpClient, IConfiguration configuration, SumaryYoutubeDbContext context)
        {
            _httpClient = httpClient;
            _apikey = configuration["ExternalServices:GeminiApiKey"];
            _context = context;
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
        
        public async Task<List<GeminiResult>> GetGeminiServiceUserAsync(IGetGeminiServiceUserDto dto)
        {
            var query = _context.VideoSummaries.AsQueryable();

            query = query.Where(v => v.IdUser == dto.IdUser);
            
            var termo = dto.Title?.Trim();
            if (!string.IsNullOrWhiteSpace(termo))
            {
                query = query.Where(v => EF.Functions.ILike(v.Title, $"%{termo}%"));
            }

            var videoSummary = await query
                .OrderByDescending(v => v.DateCreateSumary)
                .ToListAsync();

            if (!videoSummary.Any())
            {
                throw new Exception($"Nenhum resumo encontrado com este título: {dto.Title}");
            }

            return videoSummary.Select(v => new GeminiResult
            {
                Summary = v.TextGemini,
                MindMap = v.MindMap,
                DateCreateSumary = v.DateCreateSumary,
                Title = v.Title ?? "Não informado"
            }).ToList();
        }
    }
}