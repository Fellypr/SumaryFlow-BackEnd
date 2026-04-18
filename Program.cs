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
builder.Services.AddHttpClient("YoutubeExplodeBrowserClient", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.TryAddWithoutValidation(
        "User-Agent",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    AllowAutoRedirect = true,
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
    UseCookies = false
});
builder.Services.AddScoped<YoutubeClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
    var httpClient = httpClientFactory.CreateClient("YoutubeExplodeBrowserClient");

    var cookieString = configuration["YoutubeSettings:Cookies"];
    if (string.IsNullOrWhiteSpace(cookieString))
    {
        cookieString = configuration["YOUTUBE_COOKIES"];
    }

    if (!string.IsNullOrWhiteSpace(cookieString))
    {
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Cookie", cookieString);
    }

    return new YoutubeClient(httpClient);
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
