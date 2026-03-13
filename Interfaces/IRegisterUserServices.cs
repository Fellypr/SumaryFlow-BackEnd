using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SumaryYoutubeBackend.DTOs;
using SumaryYoutubeBackend.Models;
namespace SumaryYoutubeBackend.Interfaces
{
    public interface IRegisterUserServices
    {
        Task<AuthUser> RegisterUserAsync(RegisterUserServicesDto registerUserServicesDto);
        
    }
}