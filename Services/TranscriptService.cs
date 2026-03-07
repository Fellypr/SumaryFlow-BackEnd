using SumaryYoutubeBackend.interfaces;
using SumaryYoutubeBackend.DTOs;
namespace SumaryYoutubeBackend.Services
{
    public class TranscriptServices : ITranscriptService
    {
        private readonly HttpClient _httpClient;
        private readonly string _pythonApiUrl = "http://127.0.0.1:8000/transcript/";

        public TranscriptServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string>GetStrinVideoAsync(string videoId){
            var response = await _httpClient.GetFromJsonAsync<TranscriptRespondeDto>(_pythonApiUrl + videoId);
            if(response != null && !string.IsNullOrEmpty(response.Text))
                return response.Text;
            
            throw new Exception("não foi possivel obter a legenda do video");

        }

    };
}