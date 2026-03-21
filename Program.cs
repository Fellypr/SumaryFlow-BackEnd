using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SumaryYoutubeBackend.dbContext;
using SumaryYoutubeBackend.interfaces;
using SumaryYoutubeBackend.Interfaces;
using SumaryYoutubeBackend.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<ITranscriptService,TranscriptServices>();
builder.Services.AddHttpClient<IGeminiService, GeminiService>();
builder.Services.AddScoped<IJwtServices, JwtServices>();
builder.Services.AddScoped<IRegisterUserServices, RegisterUserServices>();
builder.Services.AddScoped<IAuthenticationServices, AuthenticationServices>();
builder.Services.AddHttpClient<IGetGeminiServiceUserAsync, GeminiService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"] ?? "CARACTERESPECIALPARAJWT1234567890";
        var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "SumaryYoutubeBackend";
        var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "SumaryYoutubeFrontend";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddDbContext<SumaryYoutubeDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
