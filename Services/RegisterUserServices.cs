using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SumaryYoutubeBackend.dbContext;
using SumaryYoutubeBackend.DTOs;
using SumaryYoutubeBackend.Interfaces;
using SumaryYoutubeBackend.Models;

namespace SumaryYoutubeBackend.Services
{
    public class RegisterUserServices : IRegisterUserServices
    {
        private readonly SumaryYoutubeDbContext _dbContext;
        private readonly IJwtServices _jwtServices;
        public RegisterUserServices(SumaryYoutubeDbContext dbContext, IJwtServices jwtServices)
        {
            _dbContext = dbContext;
            _jwtServices = jwtServices;
        }

        public async Task<AuthUser> RegisterUserAsync(RegisterUserServicesDto registerUserServicesDto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerUserServicesDto.Password);
            var user = new AuthUser
            {
                Username = registerUserServicesDto.UserName,
                Password = hashedPassword
            };
            _dbContext.AuthUsers.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }
        
    }
}