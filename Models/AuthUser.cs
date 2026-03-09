using SumaryYoutubeBackend.Models;
using System.Collections.Generic;
namespace SumaryYoutubeBackend.Models
{
    public class AuthUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public ICollection<VideoSummary> VideosUser { get; set; } = new List<VideoSummary>();



    }
}