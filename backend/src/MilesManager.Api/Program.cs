using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MilesManager.Api.Data;
using MilesManager.Api.Middleware;
using MilesManager.Api.Presentation;
using MilesManager.Application;
using MilesManager.Application.Context;
using MilesManager.Infrastructure;
using MilesManager.Infrastructure.Persistence;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation();

var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Missing JWT issuer.");
var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Missing JWT audience.");
var key = jwtSection["Key"] ?? throw new InvalidOperationException("Missing JWT signing key.");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,

            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy
        .WithOrigins("http://localhost:4200", "https://9cnmnz5x-4200.euw.devtunnels.ms")
        .AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUser, CurrentUser>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("frontend");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MilesManagerDbContext>();

    await dbContext.Database.MigrateAsync();
}

await SeedData.InitializeAsync(app.Services, CancellationToken.None);
await SeedDataAirports.InitializeAsync(app.Services, CancellationToken.None);

app.Run();
