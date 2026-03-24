using Microsoft.AspNetCore.Mvc;
using SumaryYoutubeBackend.DTOs;
using SumaryYoutubeBackend.Interfaces;
using SumaryYoutubeBackend.Services;
using SumaryYoutubeBackend.dbContext;
using Microsoft.EntityFrameworkCore;

namespace SumaryYoutubeBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IRegisterUserServices _registerUserServices;
        private readonly IAuthenticationServices _authenticationServices;
        private readonly IJwtServices _jwtServices;
        private readonly SumaryYoutubeDbContext _context;

        public AuthController(
            IRegisterUserServices registerUserServices,
            IAuthenticationServices authenticationServices,
            IJwtServices jwtServices, SumaryYoutubeDbContext context)
        {
            _registerUserServices = registerUserServices;
            _authenticationServices = authenticationServices;
            _jwtServices = jwtServices;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserServicesDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Os campos username e password devem ser preenchidos.");
            }

            var userExist = await _context.AuthUsers.FirstOrDefaultAsync(v => v.Username == dto.UserName);

            if(userExist != null)
            {
                return Conflict($"Usuario com o nome de {dto.UserName} já existe");
            }


            try
            {
                var user = await _registerUserServices.RegisterUserAsync(dto);
                var token = _jwtServices.GenerateTokenAuth(user);

                return StatusCode(201, new
                {
                    token = token,
                    user = new
                    {
                        id = user.Id,
                        username = user.Username
                    },
                    message = $"Conta criada com sucesso .Seja Bem Vindo {user.Username}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Ocorreu um erro ao registrar o usuário.",
                    detail = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationServicesDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Os campos username e password devem ser preenchidos.");
            }

            try
            {
                var user = await _authenticationServices.Authenticate(dto);

                return Ok(new
                {
                    token = user.Token,
                    user = new
                    {
                        id = user.Id,
                        username = user.Username
                    },
                    message = $"Bem-vindo de volta, {user.Username}!"
                });
            }
            catch (Exception ex)
            {
                var statusCode = ex.Message switch
                {
                    "Usuario nao encontrado" => 404,
                    "Senha incorreta" => 401,
                    _ => 500
                };
                return StatusCode(statusCode, new
                {
                    message = ex.Message
                });
            }
        }
    }
}
