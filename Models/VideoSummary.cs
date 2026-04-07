using System.ComponentModel.DataAnnotations;
using SumaryYoutubeBackend.Models;

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
        public TimeSpan Duration {get; set;}
        public long Vizualization {get; set;} 
        public string MindMap { get; set; }
        public DateTime DateCreateSumary { get; set; } = DateTime.UtcNow;
        public int IdUser { get; set; }
        public AuthUser AuthUser { get; set;}


    }
}