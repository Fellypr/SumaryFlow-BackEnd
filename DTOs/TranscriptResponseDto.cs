namespace SumaryYoutubeBackend.DTOs
{
    public class TranscriptRespondeDto
    {
        public string Text { get; set; }
        public string Error { get; set; }
        public string Language { get; set; }
        public bool? Translated { get; set; }
    }

}