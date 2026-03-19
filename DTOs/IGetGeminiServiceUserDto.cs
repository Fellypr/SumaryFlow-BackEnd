using System.ComponentModel.DataAnnotations;
namespace SumaryYoutubeBackend.DTOs
{
    public class IGetGeminiServiceUserDto
    {
        public int IdUser { get; set; }
        [MaxLength(50, ErrorMessage = "O título não pode ter mais de 50 caracteres")]
        public string? Title { get; set; }

    }
}