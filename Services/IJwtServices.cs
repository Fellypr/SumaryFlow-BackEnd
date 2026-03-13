using SumaryYoutubeBackend.Models;
namespace SumaryYoutubeBackend.Services
{
    public interface IJwtServices
    {
        string GenerateTokenAuth(AuthUser user);
    }
}