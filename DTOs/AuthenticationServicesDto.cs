using System.ComponentModel.DataAnnotations;
namespace SumaryYoutubeBackend.DTOs
{
    public class AuthenticationServicesDto
    {
        [Required]
        [MaxLength(50, ErrorMessage = "o nome de usuario nao pode ser maior que 50 caracteres")]
        [MinLength(3, ErrorMessage = "o nome de usuario nao pode ser menor que 3 caracteres")]
        public string Username { get; set; }
        [Required]
        [MaxLength(10, ErrorMessage = "a senha nao pode ser maior que 10 caracteres")]
        [MinLength(3, ErrorMessage = "a senha nao pode ser menor que 3 caracteres")]
        public string Password { get; set; }
    }
}