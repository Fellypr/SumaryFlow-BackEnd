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

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash-lite:generateContent?key={_apikey}";
            var prompt = "Aja como um especialista em documentação técnica e analise a transcrição de vídeo abaixo. Gere um resumo estruturado seguindo estritamente estas regras de formatação:\n\n" +
            "- Seções Principais: Use ### seguido de um título curto e direto para cada mudança de assunto (módulos).\n" +
            "- Itens de Detalhe: Dentro de cada seção, use * para listar os pontos chave.\n" +
            "- Padrão de Conteúdo: Cada item deve seguir o formato **Termo ou Conceito:** Descrição detalhada e clara. (Regra estrita: Não crie um glossário com respostas curtas. Desenvolva a explicação de cada item com 4 a 5 frases completas, explicando o contexto, o 'porquê' e incluindo os exemplos práticos mencionados no vídeo).\n" +
            "- Hierarquia: Certifique-se de que a lógica siga uma linha do tempo de aprendizado (do básico ao avançado).\n" +
            "- Restrição: Não adicione introduções como 'Aqui está o resumo' ou conclusões. Retorne apenas o conteúdo formatado em Markdown.\n\n" +
            "No final da resposta, gere um diagrama Mermaid.js no formato graph TD seguindo EXATAMENTE estas regras:\n\n" +
            "1. Use obrigatoriamente:\n" +
            "   graph TD\n\n" +
            "2. Estrutura:\n" +
            "   ID[Texto] --> ID2[Texto]\n" +
            "   ID --> ID3{Texto}\n\n" +
            "3. Regras obrigatórias:\n" +
            "- Cada nó deve ter um ID único (A, B, C, D...)\n" +
            "- Use [] para textos normais\n" +
            "- Use {} para decisões/categorias\n" +
            "- Cada linha deve conter apenas UMA relação\n" +
            "- NÃO use parágrafos\n" +
            "- NÃO use markdown (*, -, #)\n" +
            "- NÃO use explicações fora do diagrama\n" +
            "- NÃO repita nós\n" +
            "- Use textos curtos (máximo 8 palavras por nó)\n\n" +
            "4. Organização:\n" +
            "- Comece com um nó raiz (A)\n" +
            "- Expanda em níveis hierárquicos\n" +
            "- Agrupe conceitos relacionados\n\n" +
            "5. O diagrama deve ser o ÚLTIMO conteúdo da resposta\n\n" +
            "6. Retorne EXATAMENTE neste formato:\n\n" +
            "```mermaid\n" +
            "graph TD\n" +
            "    A[...]\n" +
            "    A --> B[...]\n" +
            "```\n\n" +
            $"Transcrição:\n\n{transcript}";

            var requestBody = new
            {
                contents = new[]{
                    new {parts = new[] {new {text = prompt}}}
                },
                generationConfig = new
                {
                    maxOutputTokens = 2000,
                    temperature = 0.7,
                    topP = 0.95,
                    topK = 40
                }

            };
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                var detailsError = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro ao chama Gemini: {detailsError} StatusCode: {response.ReasonPhrase} status: {response.StatusCode}");
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
                Title = v.Title ?? "Não informado",
                ThumbnaiUrl = v.ThumbnaiUrl ?? "Não informado",
                Duration = v.Duration,
                Vizualization = v.Vizualization
            }).ToList();
        }
    }
}