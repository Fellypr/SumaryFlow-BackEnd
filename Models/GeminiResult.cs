namespace SumaryYoutubeBackend.Models
{
    public class GeminiResult
    {
        public string Summary { get; set; }
        public string MindMap { get; set; }
        public DateTime DateCreateSumary { get; set; }
        public string Title { get; set; }
        public string ThumbnaiUrl { get; set; }
    }
}