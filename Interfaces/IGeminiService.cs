using SumaryYoutubeBackend.Models;

namespace SumaryYoutubeBackend.interfaces
{
    public interface IGeminiService 
    {
        Task<GeminiResult> GenerateSumaryAsync(string transcript);
    }

}