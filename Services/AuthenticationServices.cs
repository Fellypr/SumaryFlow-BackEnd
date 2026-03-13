using SumaryYoutubeBackend.Interfaces;
using SumaryYoutubeBackend.DTOs;
using SumaryYoutubeBackend.Models;
using SumaryYoutubeBackend.dbContext;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
namespace SumaryYoutubeBackend.Services
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly SumaryYoutubeDbContext _dbContext;
        private readonly IJwtServices _jwtServices;
        public AuthenticationServices(SumaryYoutubeDbContext dbContext, IJwtServices jwtServices)
        {
            _dbContext = dbContext;
            _jwtServices = jwtServices;
        }
        public async Task<AuthUser> Authenticate(AuthenticationServicesDto authenticationServicesDto)
        {
            var userExists = await _dbContext.AuthUsers.FirstOrDefaultAsync(u => u.Username == authenticationServicesDto.Username);
            if (userExists == null)
            {
                throw new Exception("Usuario nao encontrado");
            }
            if (!BCrypt.Net.BCrypt.Verify(authenticationServicesDto.Password, userExists.Password))
            {
                throw new Exception("Senha incorreta");
            }

            var token = _jwtServices.GenerateTokenAuth(userExists);
            return new AuthUser
            {
                Id = userExists.Id,
                Username = userExists.Username,
                Token = token
            };
        }

    }
}