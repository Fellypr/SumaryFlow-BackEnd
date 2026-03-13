using SumaryYoutubeBackend.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SumaryYoutubeBackend.Models
{
    public class AuthUser
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<VideoSummary> VideosUser { get; set; } = new List<VideoSummary>();
        
        [NotMapped]
        public string? Token { get; set; }
    }
}