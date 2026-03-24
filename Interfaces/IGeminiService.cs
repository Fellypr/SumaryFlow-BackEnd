using SumaryYoutubeBackend.Models;
using SumaryYoutubeBackend.DTOs;
namespace SumaryYoutubeBackend.interfaces
{
    public interface IGeminiService
    {
        Task<GeminiResult> GenerateSumaryAsync(string transcript);
    }

    public interface IGetGeminiServiceUserAsync
    {
        Task<List<GeminiResult>> GetGeminiServiceUserAsync(IGetGeminiServiceUserDto dto);
    }

}