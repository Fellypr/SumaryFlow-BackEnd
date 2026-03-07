using System.ComponentModel.DataAnnotations;

namespace SumaryYoutubeBackend.Models
{
    public class VideoSummary
    {
        [Key]
        public int IdVideo { get; set; }
        [Required]
        public string CodeVideoId { get; set; }
        public string Title { get; set; }
        public string ThumbnaiUrl { get; set; }
        [Required]
        public string Transcript { get; set; }
        [Required]
        public string TextGemini { get; set; }
        public string MindMap { get; set; }
        public DateTime DateCreateSumary { get; set; } = DateTime.UtcNow;


    }
}