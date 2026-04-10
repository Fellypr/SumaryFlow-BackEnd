using SumaryYoutubeBackend.interfaces;
using SumaryYoutubeBackend.DTOs;
using System.Text.Json;
using SumaryYoutubeBackend.Exceptions;
namespace SumaryYoutubeBackend.Services
{
    public class TranscriptServices : ITranscriptService
    {
        private readonly HttpClient _httpClient;
        private readonly string _pythonApiUrl;

        public TranscriptServices(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _pythonApiUrl = configuration["ExternalServices:TranscriptServiceUrl"] ?? "http://127.0.0.1:8000/transcript/";
        }
        public async Task<string> GetStrinVideoAsync(string videoId)
        {
            var httpResponse = await _httpClient.GetAsync(_pythonApiUrl + videoId);
            var body = await httpResponse.Content.ReadAsStringAsync();

            if (!httpResponse.IsSuccessStatusCode)
            {
                
                try
                {
                    using var doc = JsonDocument.Parse(body);
                    if (doc.RootElement.TryGetProperty("detail", out var detail))
                    {
                        var code = detail.TryGetProperty("code", out var c) ? c.GetString() : null;
                        var message = detail.TryGetProperty("message", out var m) ? m.GetString() : null;
                        throw new TranscriptServiceException(
                            statusCode: (int)httpResponse.StatusCode,
                            code: code ?? "TRANSCRIPT_ERROR",
                            message: message ?? "Falha ao obter legenda do vídeo.");
                    }
                }
                catch (JsonException)
                {
                    
                }

                throw new TranscriptServiceException(
                    statusCode: (int)httpResponse.StatusCode,
                    code: "TRANSCRIPT_ERROR",
                    message: "Falha ao obter legenda do vídeo.");
            }

            TranscriptRespondeDto response = null;
            try
            {
                response = JsonSerializer.Deserialize<TranscriptRespondeDto>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
        
            }

            if (response != null && !string.IsNullOrWhiteSpace(response.Text))
                return response.Text;

            throw new TranscriptServiceException(
                statusCode: 422,
                code: "TRANSCRIPT_EMPTY",
                message: "A legenda retornou vazia para este vídeo.");

        }

    };
}