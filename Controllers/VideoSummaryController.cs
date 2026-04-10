using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SumaryYoutubeBackend.dbContext;
using SumaryYoutubeBackend.interfaces;
using Microsoft.EntityFrameworkCore;
using SumaryYoutubeBackend.Models;
using YoutubeExplode;
using YoutubeExplode.Common;
using System.Security.Claims;
using SumaryYoutubeBackend.DTOs;
using Microsoft.Extensions.Logging;
using SumaryYoutubeBackend.Exceptions;

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
        private readonly ILogger<VideoSummaryController> _logger;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public VideoSummaryController(
            ITranscriptService transcriptService,
            IGeminiService geminiService,
            SumaryYoutubeDbContext context,
            IGetGeminiServiceUserAsync getGeminiServiceUserAsync,
            ILogger<VideoSummaryController> logger,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _transcriptService = transcriptService;
            _geminiService = geminiService;
            _context = context;
            _getGeminiServiceUserAsync = getGeminiServiceUserAsync;
            _logger = logger;
            _environment = environment;
            _configuration = configuration;
        }

        [HttpPost("summarize")]
        public async Task<IActionResult> Summarize([FromBody] VideoRequest url)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null) return Unauthorized("Usuário não autenticado");
                var userIdInt = int.Parse(userId);

                if (url == null || string.IsNullOrWhiteSpace(url.VideoUrl))
                {
                    return ApiError(StatusCodes.Status400BadRequest, "INVALID_REQUEST", "Informe a URL do vídeo do YouTube.");
                }

                var videoId = ExtractVideoId(url.VideoUrl);
                if (string.IsNullOrWhiteSpace(videoId))
                {
                    return ApiError(StatusCodes.Status400BadRequest, "INVALID_URL", "URL do vídeo inválida.");
                }

                var existing = await _context.VideoSummaries.FirstOrDefaultAsync(v => v.CodeVideoId == videoId && v.IdUser == userIdInt);
                if (existing != null) return Ok(existing);

                var youtube = new YoutubeClient();
                var metadata = await youtube.Videos.GetAsync(videoId);

                var maxMinutes = _configuration.GetValue<int?>("VideoRules:MaxMinutes") ?? 20;
                if (metadata.Duration.HasValue && metadata.Duration.Value.TotalMinutes > maxMinutes)
                {
                    return ApiError(
                        StatusCodes.Status422UnprocessableEntity,
                        "VIDEO_TOO_LONG",
                        $"O vídeo excede o limite de {maxMinutes} minutos.",
                        new
                        {
                            maxMinutes,
                            videoMinutes = Math.Round(metadata.Duration.Value.TotalMinutes, 2)
                        });
                }

                string transcript;
                try
                {
                    transcript = await _transcriptService.GetStrinVideoAsync(videoId);
                }
                catch (TranscriptServiceException tex)
                {
                    return ApiError(tex.StatusCode, tex.Code, tex.Message);
                }

                var aiResponse = await _geminiService.GenerateSumaryAsync(transcript);

                var newsumary = new VideoSummary
                {
                    CodeVideoId = videoId,
                    Title = metadata.Title,
                    ThumbnaiUrl = metadata.Thumbnails.GetWithHighestResolution().Url,
                    Transcript = transcript,
                    Duration = metadata.Duration ?? TimeSpan.Zero,
                    Vizualization = metadata.Engagement.ViewCount,
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
                return ServerError("Ocorreu um erro ao gerar o resumo do vídeo.", ex, nameof(Summarize));
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
                return ServerError("Ocorreu um erro ao buscar o resumo do vídeo.", ex, nameof(GetGeminiServiceUser));
            }
        }

        private IActionResult ServerError(string userMessage, Exception ex, string actionName)
        {
            _logger.LogError(ex, "Falha em {Action}", actionName);

            var innerErrors = new List<object>();
            for (var inner = ex.InnerException; inner != null; inner = inner.InnerException)
            {
                innerErrors.Add(new { type = inner.GetType().FullName, message = inner.Message });
            }

            return StatusCode(StatusCodes.Status500InternalServerError, new
            {
                message = userMessage,
                status = StatusCodes.Status500InternalServerError,
                traceId = HttpContext.TraceIdentifier,
                exceptionType = ex.GetType().FullName,
                detail = ex.Message,
                innerErrors,
                stackTrace = _environment.IsDevelopment() ? ex.StackTrace : null
            });
        }

        private IActionResult ApiError(int statusCode, string code, string message, object? extra = null)
        {
            if (extra == null)
            {
                return StatusCode(statusCode, new { code, message, status = statusCode, traceId = HttpContext.TraceIdentifier });
            }

            return StatusCode(statusCode, new { code, message, status = statusCode, traceId = HttpContext.TraceIdentifier, extra });
        }

        private static string ExtractVideoId(string videoUrlOrId)
        {
            if (string.IsNullOrWhiteSpace(videoUrlOrId))
            {
                return string.Empty;
            }

            var input = videoUrlOrId.Trim();
            if (!Uri.TryCreate(input, UriKind.Absolute, out var uri))
            {
                return input;
            }

            var host = uri.Host.ToLowerInvariant();

            if (host.Contains("youtu.be"))
            {
                return uri.AbsolutePath.Trim('/').Split('/').FirstOrDefault() ?? string.Empty;
            }

            if (host.Contains("youtube.com"))
            {
                if (uri.AbsolutePath.StartsWith("/watch", StringComparison.OrdinalIgnoreCase))
                {
                    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    return query["v"] ?? string.Empty;
                }

                if (uri.AbsolutePath.StartsWith("/shorts/", StringComparison.OrdinalIgnoreCase) ||
                    uri.AbsolutePath.StartsWith("/embed/", StringComparison.OrdinalIgnoreCase))
                {
                    var segments = uri.AbsolutePath.Trim('/').Split('/');
                    return segments.Length >= 2 ? segments[1] : string.Empty;
                }
            }

            return input;
        }

    }
}