namespace SumaryYoutubeBackend.interfaces
{
    public interface ITranscriptService
    {
        Task<string>GetStrinVideoAsync(string videoId);

    }
}