using Microsoft.AspNetCore.Mvc;
using SumaryYoutubeBackend.dbContext;
using SumaryYoutubeBackend.interfaces;
using Microsoft.EntityFrameworkCore;
using SumaryYoutubeBackend.Models;
using YoutubeExplode;
using YoutubeExplode.Common;
using System.Security.Claims;
using SumaryYoutubeBackend.DTOs;

namespace SumaryYoutubeBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoSummaryController : ControllerBase
    {
        private readonly ITranscriptService _transcriptService;
        private readonly IGeminiService _geminiService;
        private readonly SumaryYoutubeDbContext _context;
        private readonly IGetGeminiServiceUserAsync _getGeminiServiceUserAsync;
        public VideoSummaryController(
            ITranscriptService transcriptService,
            IGeminiService geminiService,
            SumaryYoutubeDbContext context,
            IGetGeminiServiceUserAsync getGeminiServiceUserAsync)
        {
            _transcriptService = transcriptService;
            _geminiService = geminiService;
            _context = context;
            _getGeminiServiceUserAsync = getGeminiServiceUserAsync;
        }

        [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] VideoRequest url)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized("Usuário não autenticado");
                var userIdInt = int.Parse(userId);

                var videoId = url.VideoUrl;

                var existing = await _context.VideoSummaries.FirstOrDefaultAsync(v => v.CodeVideoId == videoId && v.IdUser == userIdInt);
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
                    DateCreateSumary = DateTime.UtcNow,
                    IdUser = userIdInt
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
        [HttpGet("get-gemini-service-user")]
        public async Task<IActionResult> GetGeminiServiceUser([FromQuery] IGetGeminiServiceUserDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized("Usuário não autenticado");
                var userIdInt = int.Parse(userId);
                if (userIdInt != dto.IdUser) return Unauthorized("Usuário não autorizado");

                var geminiResult = await _getGeminiServiceUserAsync.GetGeminiServiceUserAsync(dto);
                return Ok(geminiResult);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    message = "Ocorreu um erro ao buscar o resumo do vídeo.",
                    detail = ex.Message
                });
            }
        }

    }
}