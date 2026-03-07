using Microsoft.AspNetCore.Mvc;
using SumaryYoutubeBackend.dbContext;
using SumaryYoutubeBackend.interfaces;
using SumaryYoutubeBackend.Extensions;
using Microsoft.EntityFrameworkCore;
using SumaryYoutubeBackend.Models;
using YoutubeExplode;
using YoutubeExplode.Common;

namespace SumaryYoutubeBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoSummaryController : ControllerBase
    {
        private readonly ITranscriptService _transcriptService;
        private readonly IGeminiService _geminiService;
        private readonly SumaryYoutubeDbContext _context;

        public VideoSummaryController(
            ITranscriptService transcriptService,
            IGeminiService geminiService,
            SumaryYoutubeDbContext context)
        {
            _transcriptService = transcriptService;
            _geminiService = geminiService;
            _context = context;
        }

        [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] VideoRequest url)
        {
            try
            {
                var videoId = url.VideoUrl;

                var existing = await _context.VideoSummaries.FirstOrDefaultAsync(v => v.CodeVideoId == videoId);
                if (existing != null) return Ok(existing);

                var transcript = await _transcriptService.GetStrinVideoAsync(videoId);

                var aiResponse = await _geminiService.GenerateSumaryAsync(transcript);

                var youtube = new YoutubeClient();
                var metadata = await youtube.Videos.GetAsync(videoId);

                var newsumary = new VideoSummary
                {
                    CodeVideoId = videoId,
                    Title = metadata.Title,
                    ThumbnaiUrl = metadata.Thumbnails.GetWithHighestResolution().Url,
                    Transcript = transcript,
                    TextGemini = aiResponse.Summary,
                    MindMap = aiResponse.MindMap,
                    DateCreateSumary = DateTime.UtcNow
                };

                _context.VideoSummaries.Add(newsumary);
                await _context.SaveChangesAsync();
                return Ok(newsumary);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Ocorreu um erro ao gerar o resumo do vídeo.",
                    detail = ex.Message
                });
            }
        }

    }
}