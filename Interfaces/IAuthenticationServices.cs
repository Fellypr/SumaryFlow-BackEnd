using SumaryYoutubeBackend.DTOs;
using SumaryYoutubeBackend.Models;
namespace SumaryYoutubeBackend.Interfaces
{
    public interface IAuthenticationServices
    {
        Task<AuthUser> Authenticate(AuthenticationServicesDto authenticationServicesDto);
    }
}