using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text;
using SumaryYoutubeBackend.dbContext;
using SumaryYoutubeBackend.interfaces;
using SumaryYoutubeBackend.Interfaces;
using SumaryYoutubeBackend.Services;
using YoutubeExplode;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<YoutubeClient>(_ =>
{
    var cookiesValue = builder.Configuration["YoutubeSettings:Cookies"];
    if (string.IsNullOrWhiteSpace(cookiesValue))
        return new YoutubeClient();

    var cookies = cookiesValue
        .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(cookiePart => cookiePart.Split('=', 2, StringSplitOptions.TrimEntries))
        .Where(parts => parts.Length == 2 && !string.IsNullOrWhiteSpace(parts[0]))
        .Select(parts => new Cookie(parts[0], parts[1], "/", ".youtube.com"))
        .ToList();

    return cookies.Count > 0 ? new YoutubeClient(cookies) : new YoutubeClient();
});
builder.Services.AddHttpClient<ITranscriptService,TranscriptServices>();
builder.Services.AddHttpClient<IGeminiService, GeminiService>();
builder.Services.AddScoped<IJwtServices, JwtServices>();
builder.Services.AddScoped<IRegisterUserServices, RegisterUserServices>();
builder.Services.AddScoped<IAuthenticationServices, AuthenticationServices>();
builder.Services.AddHttpClient<IGetGeminiServiceUserAsync, GeminiService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtKey = builder.Configuration["Jwt:Key"] 
            ?? throw new Exception("Jwt:Key não está configurada.");
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
        policy.WithOrigins(
            "http://localhost:3000",
            "https://sumary-flow-cliente.vercel.app",
            "https://sumary-flow-cliente-8ig65xl7y-fellype15kenned-4944s-projects.vercel.app"
            )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
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
